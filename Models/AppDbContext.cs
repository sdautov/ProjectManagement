using Microsoft.EntityFrameworkCore;

namespace ProjectManagement.Models;

public partial class AppDbContext : DbContext {
    public AppDbContext() { }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public virtual DbSet<Contractor> Contractors { get; set; }
    public virtual DbSet<DesignObject> DesignObjects { get; set; }
    public virtual DbSet<Document> Documents { get; set; }
    public virtual DbSet<DocumentType> DocumentTypes { get; set; }
    public virtual DbSet<DocumentationSet> DocumentationSets { get; set; }
    public virtual DbSet<Mark> Marks { get; set; }
    public virtual DbSet<Project> Projects { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlite("Data Source=database.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<DesignObject>(entity => {
            entity.HasOne(d => d.Contractor).WithMany(p => p.DesignObjects).HasForeignKey(d => d.ContractorId);
            entity.HasOne(d => d.ParentObject).WithMany(p => p.InverseParentObject)
                .HasForeignKey(d => d.ParentObjectId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(d => d.Project).WithMany(p => p.DesignObjects).HasForeignKey(d => d.ProjectId);
        });
        modelBuilder.Entity<Document>(entity => {
            entity.HasIndex(e => new { e.DocumentationSetId, e.DocumentTypeId, e.Number }, "idx_documents_unique").IsUnique();
            entity.HasOne(d => d.DocumentType).WithMany(p => p.Documents).HasForeignKey(d => d.DocumentTypeId);
            entity.HasOne(d => d.DocumentationSet).WithMany(p => p.Documents).HasForeignKey(d => d.DocumentationSetId);
        });
        modelBuilder.Entity<DocumentationSet>(entity => {
            entity.HasIndex(e => new { e.DesignObjectId, e.MarkId, e.Number }, "idx_documentation_sets_unique").IsUnique();
            entity.HasOne(d => d.DesignObject).WithMany(p => p.DocumentationSets).HasForeignKey(d => d.DesignObjectId);
            entity.HasOne(d => d.Mark).WithMany(p => p.DocumentationSets).HasForeignKey(d => d.MarkId);
        });
        modelBuilder.Entity<Project>(entity => { entity.HasOne(d => d.Contractor).WithMany(p => p.Projects).HasForeignKey(d => d.ContractorId); });
    }
}
