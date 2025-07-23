using FluentValidation;

namespace SkillForge.Application.Features.ProjectTags.Commands.Update
{
    public class UpdateProjectTagCommandValidator : AbstractValidator<UpdateProjectTagCommand>
    {
        public UpdateProjectTagCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Invalid tag ID.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Tag name is required.")
                .MaximumLength(50)
                .WithMessage("Tag name cannot exceed 50 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(200)
                .WithMessage("Description cannot exceed 200 characters.")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.ProjectId)
                .NotEmpty()
                .WithMessage("Project ID is required.");
        }
    }
} 