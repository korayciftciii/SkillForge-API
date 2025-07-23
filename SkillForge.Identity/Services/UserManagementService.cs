using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SkillForge.Application.Common.Models.Results;
using SkillForge.Application.DataTransferObjects;
using SkillForge.Identity.Models;
using SkillForge.Shared.Results;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SkillForge.Identity.Services
{
    public class UserManagementService : SkillForge.Application.Common.Interfaces.IUserManagementService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public UserManagementService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<PaginatedResult<UserDto>> GetAllUsersAsync(int page, int pageSize, string? search = null, string? sortBy = null, string? sortDirection = null, CancellationToken cancellationToken = default)
        {
            var query = _userManager.Users.AsQueryable();

            // Search functionality
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u =>
                    u.UserName.ToLower().Contains(search.ToLower()) ||
                    u.Email.ToLower().Contains(search.ToLower()) ||
                    u.Name.ToLower().Contains(search.ToLower()) ||
                    u.Surname.ToLower().Contains(search.ToLower()));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            // Apply sorting
            bool desc = sortDirection?.ToLower() == "desc";
            query = sortBy?.ToLower() switch
            {
                "username" => desc ? query.OrderByDescending(u => u.UserName) : query.OrderBy(u => u.UserName),
                "email" => desc ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
                "name" => desc ? query.OrderByDescending(u => u.Name) : query.OrderBy(u => u.Name),
                "surname" => desc ? query.OrderByDescending(u => u.Surname) : query.OrderBy(u => u.Surname),
                _ => desc ? query.OrderByDescending(u => u.UserName) : query.OrderBy(u => u.UserName)
            };

            var users = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                userDtos.Add(new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    Name = user.Name,
                    Surname = user.Surname,
                    EmailConfirmed = user.EmailConfirmed,
                    LockoutEnabled = user.LockoutEnabled,
                    LockoutEnd = user.LockoutEnd,
                    AccessFailedCount = user.AccessFailedCount,
                    Roles = roles.ToList()
                });
            }

            return new PaginatedResult<UserDto>(userDtos, totalCount, page, pageSize);
        }

        public async Task<SkillForge.Shared.Results.Result<UserDto>> GetUserByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return SkillForge.Shared.Results.Result<UserDto>.Fail("User not found.");

            var roles = await _userManager.GetRolesAsync(user);

            var userDto = new UserDto
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Name = user.Name,
                Surname = user.Surname,
                EmailConfirmed = user.EmailConfirmed,
                LockoutEnabled = user.LockoutEnabled,
                LockoutEnd = user.LockoutEnd,
                AccessFailedCount = user.AccessFailedCount,
                Roles = roles.ToList()
            };

            return SkillForge.Shared.Results.Result<UserDto>.Ok(userDto);
        }

        public async Task<SkillForge.Shared.Results.Result> UpdateUserAsync(string id, string userName, string email, string name, string surname, bool emailConfirmed, bool lockoutEnabled, List<string> roles, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return SkillForge.Shared.Results.Result.Fail("User not found.");

            // Check if email is already taken by another user
            if (user.Email != email)
            {
                var existingUser = await _userManager.FindByEmailAsync(email);
                if (existingUser != null && existingUser.Id != id)
                    return SkillForge.Shared.Results.Result.Fail("Email is already taken by another user.");
            }

            // Check if username is already taken by another user
            if (user.UserName != userName)
            {
                var existingUser = await _userManager.FindByNameAsync(userName);
                if (existingUser != null && existingUser.Id != id)
                    return SkillForge.Shared.Results.Result.Fail("Username is already taken by another user.");
            }

            // Update user properties
            user.UserName = userName;
            user.Email = email;
            user.Name = name;
            user.Surname = surname;
            user.EmailConfirmed = emailConfirmed;
            user.LockoutEnabled = lockoutEnabled;
            user.NormalizedUserName = userName.ToUpper();
            user.NormalizedEmail = email.ToUpper();

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
                return SkillForge.Shared.Results.Result.Fail(updateResult.Errors.Select(e => e.Description).ToArray());

            // Update user roles
            var currentRoles = await _userManager.GetRolesAsync(user);
            var rolesToRemove = currentRoles.Except(roles);
            var rolesToAdd = roles.Except(currentRoles);

            // Remove roles
            if (rolesToRemove.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                if (!removeResult.Succeeded)
                    return SkillForge.Shared.Results.Result.Fail(removeResult.Errors.Select(e => e.Description).ToArray());
            }

            // Add roles
            if (rolesToAdd.Any())
            {
                // Verify all roles exist
                foreach (var role in rolesToAdd)
                {
                    if (!await _roleManager.RoleExistsAsync(role))
                        return SkillForge.Shared.Results.Result.Fail($"Role '{role}' does not exist.");
                }

                var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
                if (!addResult.Succeeded)
                    return SkillForge.Shared.Results.Result.Fail(addResult.Errors.Select(e => e.Description).ToArray());
            }

            return SkillForge.Shared.Results.Result.Ok("User updated successfully.");
        }

        public async Task<SkillForge.Shared.Results.Result> DeleteUserAsync(string id, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return SkillForge.Shared.Results.Result.Fail("User not found.");

            var deleteResult = await _userManager.DeleteAsync(user);

            if (!deleteResult.Succeeded)
                return SkillForge.Shared.Results.Result.Fail(deleteResult.Errors.Select(e => e.Description).ToArray());

            return SkillForge.Shared.Results.Result.Ok("User deleted successfully.");
        }
    }
} 