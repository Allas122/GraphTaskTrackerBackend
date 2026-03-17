using GraphTaskTrackerBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GraphTaskTrackerBackend.Infrastructure.DataBase.EntityTypeConfiguration;

public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasIndex(e => e.Name).IsUnique();
    }
}