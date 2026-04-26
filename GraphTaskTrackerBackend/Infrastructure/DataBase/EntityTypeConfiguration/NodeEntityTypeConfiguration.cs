using GraphTaskTrackerBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GraphTaskTrackerBackend.Infrastructure.DataBase.EntityTypeConfiguration;

public class NodeEntityTypeConfiguration : IEntityTypeConfiguration<Node>
{
    public void Configure(EntityTypeBuilder<Node> builder)
    {
        builder.ToTable("Nodes");
        builder.HasKey(n => n.Id);
        
        builder.HasMany(n => n.Nodes)
            .WithMany()
            .UsingEntity<Edge>(
                l => l.HasOne(e => e.ToNode)
                    .WithMany()
                    .HasForeignKey(e => e.ToNodeId)
                    .OnDelete(DeleteBehavior.Cascade),
                r => r.HasOne(e => e.FromNode)
                    .WithMany()
                    .HasForeignKey(e => e.FromNodeId)
                    .OnDelete(DeleteBehavior.Cascade),
                j => 
                { 
                    j.ToTable("Edges");
                    j.HasKey(e => new { e.FromNodeId, e.ToNodeId }); 
                }
            );
        
        builder.HasOne(n => n.Graph)
            .WithMany(g => g.Nodes)
            .HasForeignKey(n => n.GraphId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(n => n.Author)
            .WithMany(u => u.AuthorNodes)
            .HasForeignKey(n => n.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Assigned)
            .WithMany(u => u.AssignedNodes)
            .UsingEntity<AssignedUser>(
                "AssignedUserNodes",
                l => l.HasOne(au => au.User)
                    .WithMany()
                    .HasForeignKey(au => au.UserId)
                    .OnDelete(DeleteBehavior.Cascade),
                r => r.HasOne(au => au.Node)
                    .WithMany()
                    .HasForeignKey(au => au.NodeId)
                    .OnDelete(DeleteBehavior.Cascade),
                j => 
                {
                    j.HasKey(au => new { au.NodeId, au.UserId }); // Составной ключ
                }
                );
    }
}