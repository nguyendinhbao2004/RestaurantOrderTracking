using FluentValidation;

namespace Application.Feature.Auth.Command.Register.RegisterWaiter
{
    public class RegisterWaiterValidator : AbstractValidator<RegisterWaiterCommand>
    {
        public RegisterWaiterValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage("UserName is required");
            RuleFor(x => x.FullName).NotEmpty().WithMessage("FullName is required");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long");
            RuleFor(x => x.Phone).NotEmpty().WithMessage("Phone is required")
                .Matches(@"^\d{10}$").WithMessage("Phone must be a valid 10-digit number");
            RuleFor(x => x.AreaId).NotEmpty().WithMessage("AreaId is required");
        }
    }
}