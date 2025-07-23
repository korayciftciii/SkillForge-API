using FluentValidation;

namespace SkillForge.Application.Features.ProjectMembers.Commands.Update
{
    public class UpdateProjectMemberCommandValidator : AbstractValidator<UpdateProjectMemberCommand>
    {
        public UpdateProjectMemberCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Invalid member ID.");

            RuleFor(x => x.ProjectId)
                .NotEmpty()
                .WithMessage("Project ID is required.");

            RuleFor(x => x.Role)
                .IsInEnum()
                .WithMessage("Invalid role specified.");
        }
    }
} 