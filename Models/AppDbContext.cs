using Microsoft.EntityFrameworkCore;

namespace ProjectManagement.Models;

public class AppDbContext : DbContext {
    public DbSet<Project> Projects { get; set; }
    public DbSet<DesignObject> DesignObjects { get; set; }
    public DbSet<DocumentationSet> DocumentationSets { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<Lookup> Lookups { get; set; }

    protected override void OnConfiguring(
        DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlite("Data Source=database.db");
    }
}
