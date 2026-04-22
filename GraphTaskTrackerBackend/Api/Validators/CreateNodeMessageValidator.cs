using FluentValidation;
using GraphTaskTrackerBackend.Api.Models;

namespace GraphTaskTrackerBackend.Api.Validators;

public class CreateNodeMessageValidator : AbstractValidator<CreateNodeMessage>
{
    public CreateNodeMessageValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Node Id is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Node name is required.")
            .MaximumLength(200).WithMessage("Node name must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");
    }
}