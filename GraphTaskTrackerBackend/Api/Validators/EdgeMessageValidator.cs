using FluentValidation;
using GraphTaskTrackerBackend.Api.Models;

namespace GraphTaskTrackerBackend.Api.Validators;

public class EdgeMessageValidator : AbstractValidator<EdgeMessage>
{
    public EdgeMessageValidator()
    {
        RuleFor(x => x.FromNodeId)
            .NotEmpty().WithMessage("Source Node Id (FromNodeId) is required.");

        RuleFor(x => x.ToNodeId)
            .NotEmpty().WithMessage("Destination Node Id (ToNodeId) is required.")
            .NotEqual(x => x.FromNodeId).WithMessage("Self-loops are not allowed (FromNodeId and ToNodeId must be different).");

        RuleFor(x => x.Weight)
            .GreaterThanOrEqualTo(0).WithMessage("Edge weight cannot be negative.");
    }
}