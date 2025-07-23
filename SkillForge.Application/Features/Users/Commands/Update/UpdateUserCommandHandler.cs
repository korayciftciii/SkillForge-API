using MediatR;
using SkillForge.Application.Common.Interfaces;
using SkillForge.Shared.Results;
using System.Threading;
using System.Threading.Tasks;

namespace SkillForge.Application.Features.Users.Commands.Update
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result>
    {
        private readonly IUserManagementService _userManagementService;

        public UpdateUserCommandHandler(IUserManagementService userManagementService)
        {
            _userManagementService = userManagementService;
        }

        public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            return await _userManagementService.UpdateUserAsync(
                request.Id,
                request.UserName,
                request.Email,
                request.Name,
                request.Surname,
                request.EmailConfirmed,
                request.LockoutEnabled,
                request.Roles,
                cancellationToken);
        }
    }
} 