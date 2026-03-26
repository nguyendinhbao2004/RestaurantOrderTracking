using FluentValidation;

namespace Application.Feature.Auth.Command.Register.RegisterChef
{
    public class RegisterChefValidator : AbstractValidator<RegisterChefCommand>
    {
        public RegisterChefValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username is required.")
                .MaximumLength(50).WithMessage("Username must not exceed 50 characters.");
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(100).WithMessage("Full name must not exceed 100 characters.");
            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone number is required.")
                .MaximumLength(15).WithMessage("Phone number must not exceed 15 characters.");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
                .MaximumLength(100).WithMessage("Password must not exceed 100 characters.");
            RuleFor(x => x.Specialty)
                .IsInEnum().WithMessage("Specialty must be a valid expertise for chefs.");
            RuleFor(x => x.SkillLevel)
                .NotEmpty().WithMessage("Skill level is required.")
                .MaximumLength(50).WithMessage("Skill level must not exceed 50 characters.");
        }
    }
}