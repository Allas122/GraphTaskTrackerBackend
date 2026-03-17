using GraphTaskTrackerBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GraphTaskTrackerBackend.Infrastructure.DataBase;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Graph> Graphs { get; set; }
    public DbSet<Node> Nodes { get; set; }
    public DbSet<Edge> Edges { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);
    }
}