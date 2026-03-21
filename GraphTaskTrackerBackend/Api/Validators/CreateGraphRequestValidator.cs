using FluentValidation;
using GraphTaskTrackerBackend.Api.Models;

namespace GraphTaskTrackerBackend.Application.Validators;

public class CreateGraphRequestValidator : AbstractValidator<CreateGraphRequest>
{
    public CreateGraphRequestValidator()
    {
        RuleFor(u=>u.Name)
            .NotNull().NotEmpty().WithMessage("Name is required")
            .Length(10,50).WithMessage("Name must be between 10 and 50 characters long");
    }
}