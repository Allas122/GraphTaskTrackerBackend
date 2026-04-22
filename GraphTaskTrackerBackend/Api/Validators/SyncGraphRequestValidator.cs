using GraphTaskTrackerBackend.Api.Models;

namespace GraphTaskTrackerBackend.Api.Validators;

using FluentValidation;
using System.Linq;

public class SyncGraphRequestValidator : AbstractValidator<SyncGraphRequest>
{
    public SyncGraphRequestValidator()
    {
        RuleFor(x => x.GraphId)
            .NotEmpty().WithMessage("GraphId is required.");
        
        RuleForEach(x => x.Nodes).SetValidator(new CreateNodeMessageValidator());
        RuleForEach(x => x.Edges).SetValidator(new EdgeMessageValidator());
        
        RuleFor(x => x.Nodes)
            .Must(nodes => nodes == null || nodes.Select(n => n.Id).Distinct().Count() == nodes.Count)
            .WithMessage("Duplicate Node IDs detected in the request.");

        
        RuleFor(x => x.Edges)
            .Must(edges => edges == null || edges
                .Select(e => new { e.FromNodeId, e.ToNodeId })
                .Distinct().Count() == edges.Count)
            .WithMessage("Duplicate edges detected (multiple edges with the same From and To IDs).");
        
        RuleFor(x => x)
            .Custom((request, context) =>
            {
                if (request.Nodes == null || request.Edges == null) return;

                var nodeIds = request.Nodes.Select(n => n.Id).ToHashSet();

                foreach (var edge in request.Edges)
                {
                    if (!nodeIds.Contains(edge.FromNodeId))
                    {
                        context.AddFailure(nameof(request.Edges), 
                            $"Edge refers to a non-existent FromNodeId: {edge.FromNodeId}");
                    }
                    if (!nodeIds.Contains(edge.ToNodeId))
                    {
                        context.AddFailure(nameof(request.Edges), 
                            $"Edge refers to a non-existent ToNodeId: {edge.ToNodeId}");
                    }
                }
            });
    }
}