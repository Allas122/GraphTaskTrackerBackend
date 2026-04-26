using FluentValidation;
using GraphTaskTrackerBackend.Api.Models;

namespace GraphTaskTrackerBackend.Api.Validators;

public class CreateGraphRequestValidator : AbstractValidator<CreateGraphRequest>
{
    public CreateGraphRequestValidator()
    {
        RuleFor(u=>u.Name)
            .NotNull().NotEmpty().WithMessage("Name is required")
            .Length(1,1500).WithMessage("Name must be between 1 and 1500 characters long");
        RuleFor(u => u.Description)
            .MaximumLength(2500).WithMessage("Description cannot exceed 2500 characters");
    }
}