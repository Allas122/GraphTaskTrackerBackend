using GraphTaskTrackerBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GraphTaskTrackerBackend.Infrastructure.DataBase.EntityTypeConfiguration;

public class GraphEntityTypeConfiguration : IEntityTypeConfiguration<Graph>
{
    public void Configure(EntityTypeBuilder<Graph> builder)
    {
        builder
            .HasIndex(e => e.Name)
            .HasMethod("gin")
            .HasOperators("gin_trgm_ops");
        builder
            .HasIndex(e => e.Description)
            .HasMethod("gin")
            .HasOperators("gin_trgm_ops");
    }
}