using SkillForge.Application.Common.Models.Results;
using SkillForge.Application.DataTransferObjects;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace SkillForge.Application.Common.Interfaces
{
    public interface IUserManagementService
    {
        Task<PaginatedResult<UserDto>> GetAllUsersAsync(int page, int pageSize, string? search = null, string? sortBy = null, string? sortDirection = null, CancellationToken cancellationToken = default);
        Task<SkillForge.Shared.Results.Result<UserDto>> GetUserByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<SkillForge.Shared.Results.Result> UpdateUserAsync(string id, string userName, string email, string name, string surname, bool emailConfirmed, bool lockoutEnabled, List<string> roles, CancellationToken cancellationToken = default);
        Task<SkillForge.Shared.Results.Result> DeleteUserAsync(string id, CancellationToken cancellationToken = default);
    }
} 