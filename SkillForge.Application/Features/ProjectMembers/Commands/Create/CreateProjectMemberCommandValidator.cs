using FluentValidation;

namespace SkillForge.Application.Features.ProjectMembers.Commands.Create
{
    public class CreateProjectMemberCommandValidator : AbstractValidator<CreateProjectMemberCommand>
    {
        public CreateProjectMemberCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("User ID is required.");

            RuleFor(x => x.ProjectId)
                .NotEmpty()
                .WithMessage("Project ID is required.");

            RuleFor(x => x.Role)
                .IsInEnum()
                .WithMessage("Invalid role specified.");
        }
    }
} 