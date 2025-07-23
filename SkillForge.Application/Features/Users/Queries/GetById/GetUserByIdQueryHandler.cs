using MediatR;
using SkillForge.Application.DataTransferObjects;
using SkillForge.Application.Common.Interfaces;
using SkillForge.Shared.Results;
using System.Threading;
using System.Threading.Tasks;

namespace SkillForge.Application.Features.Users.Queries.GetById
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
    {
        private readonly IUserManagementService _userManagementService;

        public GetUserByIdQueryHandler(IUserManagementService userManagementService)
        {
            _userManagementService = userManagementService;
        }

        public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            return await _userManagementService.GetUserByIdAsync(request.Id, cancellationToken);
        }
    }
} 