using FluentValidation;

namespace Application.Feature.Roles.Commands.Create
{
    public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
    {
        public CreateRoleCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Role name is required.")
                .MaximumLength(100).WithMessage("Role name must not exceed 100 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Role description is required.")
                .MaximumLength(500).WithMessage("Role description must not exceed 500 characters.");
        }
    }
}