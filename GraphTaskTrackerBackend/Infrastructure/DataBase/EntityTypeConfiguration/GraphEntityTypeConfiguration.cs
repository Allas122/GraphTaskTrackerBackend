using GraphTaskTrackerBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GraphTaskTrackerBackend.Infrastructure.DataBase.EntityTypeConfiguration;

public class GraphEntityTypeConfiguration : IEntityTypeConfiguration<Node>
{
    public void Configure(EntityTypeBuilder<Node> builder)
    {
        builder.HasMany<Node>()
            .WithMany()
            .UsingEntity<Edge>(
                l => l.HasOne(e => e.ToNode)
                    .WithMany()
                    .HasForeignKey(e => e.ToNodeId)
                    .OnDelete(DeleteBehavior.Restrict),
                r => r.HasOne(e => e.FromNode)
                    .WithMany()
                    .HasForeignKey(e => e.FromNodeId)
                    .OnDelete(DeleteBehavior.Restrict)
            );
    }
}