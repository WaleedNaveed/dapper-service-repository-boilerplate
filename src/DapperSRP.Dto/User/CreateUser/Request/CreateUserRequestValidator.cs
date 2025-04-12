using FluentValidation;

namespace DapperSRP.Dto.User.CreateUser.Request
{
    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.")
                .MinimumLength(3)
                .WithMessage("Name must be at least 3 characters.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Invalid email format.");

            RuleFor(x => x.Role)
                .GreaterThan(0)
                .WithMessage("Invalid role selected");
        }
    }
}
