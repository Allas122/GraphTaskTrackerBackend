using FluentValidation;
using GraphTaskTrackerBackend.Api.Models;

namespace GraphTaskTrackerBackend.Api.Validators;

public class CreateGraphRequestValidator : AbstractValidator<CreateGraphRequest>
{
    public CreateGraphRequestValidator()
    {
        RuleFor(u=>u.Name)
            .NotNull().NotEmpty().WithMessage("Name is required")
            .Length(10,50).WithMessage("Name must be between 10 and 50 characters long");
        RuleFor(u => u.Description)
            .MaximumLength(2500).WithMessage("Description cannot exceed 2500 characters");
    }
}