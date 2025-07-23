using Microsoft.AspNetCore.Identity;
using SkillForge.Identity.Configuration;
using SkillForge.Identity.DTOs;
using SkillForge.Identity.Models;
using SkillForge.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using SkillForge.Infrastructure.Persistence;




namespace SkillForge.Identity.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtSettings _jwtSettings;
        private readonly AppDbContext _context;

        public AuthService(
            UserManager<ApplicationUser> userManager, 
            IOptions<JwtSettings> jwtSettings,
            AppDbContext context)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
            _context = context;
        }

        public async Task<AuthResultDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                return new AuthResultDto { Success = false, Errors = ["Invalid credentials"] };
            }
            var authResult = await GenerateJwtTokenAsync(user);
            return authResult;
        }

        public async Task<AuthResultDto> RegisterAsync(RegisterDto registerDto)
        {
            var user = new ApplicationUser {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                Name = registerDto.Name,
                Surname = registerDto.Surname,
                EmailConfirmed = true // Auto-confirm email for simplicity
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded) {
                return new AuthResultDto
                {
                    Success = false,
                    Errors = result.Errors.Select(e => e.Description).ToList()
                };
            }

            // Automatically assign User role to newly registered users
            var addToRoleResult = await _userManager.AddToRoleAsync(user, ApplicationRole.User);
            if (!addToRoleResult.Succeeded)
            {
                // Log the error but don't fail the registration
                // The user is created successfully, just without the role
                // In production, you might want to handle this differently
            }

            var authResult = await GenerateJwtTokenAsync(user);
            return authResult;
        }

        public async Task<AuthResultDto> RefreshTokenAsync(RefreshTokenDto refreshTokenDto)
        {
            return await VerifyAndGenerateTokenAsync(refreshTokenDto);
        }

        public async Task<bool> RevokeTokenAsync(string refreshToken)
        {
            var token = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == refreshToken);
            if (token == null)
                return false;

            token.IsRevoked = true;
            await _context.SaveChangesAsync();
            
            return true;
        }

        public async Task<AuthResultDto> VerifyAndGenerateTokenAsync(RefreshTokenDto tokenRequest)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            try
            {
                // Validate the JWT token
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false, // We don't care about the expiry when validating, as it's expected to be expired
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidAudience = _jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key))
                };

                // Validate and extract claims from expired token
                var principal = jwtTokenHandler.ValidateToken(
                    tokenRequest.Token,
                    tokenValidationParameters,
                    out var validatedToken);

                // Validate security algorithm
                if (validatedToken is not JwtSecurityToken jwtSecurityToken ||
                    !jwtSecurityToken.Header.Alg.Equals(
                        SecurityAlgorithms.HmacSha256,
                        StringComparison.InvariantCultureIgnoreCase))
                {
                    return new AuthResultDto
                    {
                        Success = false,
                        Errors = ["Invalid token"]
                    };
                }

                // Check if refresh token exists
                var storedRefreshToken = await _context.RefreshTokens
                    .FirstOrDefaultAsync(rt => rt.Token == tokenRequest.RefreshToken);

                if (storedRefreshToken == null)
                {
                    return new AuthResultDto
                    {
                        Success = false,
                        Errors = ["Invalid refresh token"]
                    };
                }

                // Check if refresh token is used or revoked
                if (storedRefreshToken.IsUsed)
                {
                    return new AuthResultDto
                    {
                        Success = false,
                        Errors = ["Refresh token has been used"]
                    };
                }

                if (storedRefreshToken.IsRevoked)
                {
                    return new AuthResultDto
                    {
                        Success = false,
                        Errors = ["Refresh token has been revoked"]
                    };
                }

                // Check if refresh token has expired
                if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
                {
                    return new AuthResultDto
                    {
                        Success = false,
                        Errors = ["Refresh token has expired"]
                    };
                }

                // Get JWT ID from token
                var jti = principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
                
                // Check if the JWT ID matches
                if (storedRefreshToken.JwtId != jti)
                {
                    return new AuthResultDto
                    {
                        Success = false,
                        Errors = ["Token mismatch"]
                    };
                }

                // Mark current refresh token as used
                storedRefreshToken.IsUsed = true;
                _context.RefreshTokens.Update(storedRefreshToken);
                await _context.SaveChangesAsync();

                // Get the user to generate new tokens
                var userId = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return new AuthResultDto
                    {
                        Success = false,
                        Errors = ["User not found"]
                    };
                }

                // Generate new JWT and refresh token
                var authResult = await GenerateJwtTokenAsync(user);
                return authResult;
            }
            catch (Exception ex)
            {
                return new AuthResultDto
                {
                    Success = false,
                    Errors = ["Server error: " + ex.Message]
                };
            }
        }

        private async Task<AuthResultDto> GenerateJwtTokenAsync(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            var roleClaims = roles.Select(r => new Claim(ClaimTypes.Role, r)).ToList();

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Name, user.UserName)
            };

            claims.AddRange(userClaims);
            claims.AddRange(roleClaims);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Calculate token expiration
            var tokenExpiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresInMinutes);
            
            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: tokenExpiration,
                signingCredentials: creds
            );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
            
            // Generate refresh token
            var refreshToken = new RefreshToken
            {
                Token = GenerateRandomString(35),
                JwtId = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value,
                UserId = user.Id,
                ExpiryDate = DateTime.UtcNow.AddDays(30),
                IsRevoked = false,
                IsUsed = false
            };

            // Save refresh token to database
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return new AuthResultDto
            {
                Success = true,
                Token = jwtToken,
                RefreshToken = refreshToken.Token,
                TokenExpiration = tokenExpiration
            };
        }

        private static string GenerateRandomString(int length)
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[length];
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes)
                .Replace("/", "_")
                .Replace("+", "-")
                .Replace("=", "")
                .Substring(0, length);
        }
    }
}
