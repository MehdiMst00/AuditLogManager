namespace AuditLogManager.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<AuditLogChange> AuditLogChanges => Set<AuditLogChange>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditLog>(e =>
        {
            e.ToTable("AuditLogs", "log");

            e.HasKey(e => e.Id);
            e.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<AuditLogChange>(e =>
        {
            e.ToTable("AuditLogChanges", "log");

            e.HasKey(e => e.Id);
            e.Property(e => e.Id).ValueGeneratedNever();
        });

        base.OnModelCreating(modelBuilder);
    }
}