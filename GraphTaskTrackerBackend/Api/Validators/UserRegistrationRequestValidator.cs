using FluentValidation;
using GraphTaskTrackerBackend.Api.Models;

namespace GraphTaskTrackerBackend.Api.Validators;

public class UserRegistrationRequestValidator : AbstractValidator<VerifyUserRequest>
{
    public UserRegistrationRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Username is required.")
            .Length(3, 50).WithMessage("Username must be between 3 and 50 characters.")
            .Matches(@"^[a-zA-Z0-9_\-]+$")
            .WithMessage("Username can only contain letters, numbers, underscores, or hyphens.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .MaximumLength(100).WithMessage("Password must not exceed 100 characters.")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one digit.")
            .Matches(@"[\!\?\*\.\@\#\$\%\^]")
            .WithMessage("Password must contain at least one special character (!?*.@#$%^).");
    }
}