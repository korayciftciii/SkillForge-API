using MediatR;
using SkillForge.Application.Common.Models.Results;
using SkillForge.Application.DataTransferObjects;
using SkillForge.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SkillForge.Application.Features.Users.Queries.GetAll
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, PaginatedResult<UserDto>>
    {
        private readonly IUserManagementService _userManagementService;

        public GetAllUsersQueryHandler(IUserManagementService userManagementService)
        {
            _userManagementService = userManagementService;
        }

        public async Task<PaginatedResult<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            return await _userManagementService.GetAllUsersAsync(
                request.Filter.Page,
                request.Filter.PageSize,
                request.Filter.Search,
                request.Filter.SortBy,
                request.Filter.SortDirection,
                cancellationToken);
        }
    }
} 