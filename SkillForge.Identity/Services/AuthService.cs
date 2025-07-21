using Microsoft.AspNetCore.Identity;
using SkillForge.Identity.Configuration;
using SkillForge.Identity.DTOs;
using SkillForge.Identity.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;




namespace SkillForge.Identity.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtSettings _jwtSettings;
        public AuthService(UserManager<ApplicationUser> userManager, IOptions<JwtSettings> jwtSettings)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<AuthResultDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                return new AuthResultDto { Success = false, Errors = ["Invalid credentials"] };
            }
            var token =await GenerateJwtToken(user);
            return new AuthResultDto { Success = true, Token = token };
        }

        public async Task<AuthResultDto> RegisterAsync(RegisterDto registerDto)
        {
            var user = new ApplicationUser {
            UserName=registerDto.UserName,
                Email = registerDto.Email,
                Name = registerDto.Name,
                Surname = registerDto.Surname
            };

            var result =await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded) {
                return new AuthResultDto
                {
                    Success = false,
                    Errors = result.Errors.Select(e => e.Description).ToList()
                };
            }

            var token = await GenerateJwtToken(user);
            return new AuthResultDto { Success = true, Token = token };
      
        }
        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            var roleClaims = roles.Select(r => new Claim(ClaimTypes.Role, r)).ToList();

            var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Email, user.Email)
        };

            claims.AddRange(userClaims);
            claims.AddRange(roleClaims);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresInMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
