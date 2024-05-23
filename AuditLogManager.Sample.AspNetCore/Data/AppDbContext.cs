using AuditLogManager.Sample.AspNetCore.AuditLogging;
using AuditLogManager.Sample.AspNetCore.Models;
using Microsoft.EntityFrameworkCore;

namespace AuditLogManager.Sample.AspNetCore.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options, 
    IAuditingManager auditingManager) : DbContext(options)
{
    public DbSet<Todo> Todo => Set<Todo>();
    public DbSet<Contributor> Contributors => Set<Contributor>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contributor>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedNever();
        });

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var auditLog = auditingManager.Current;
        List<AuditLogChange>? entityChangeList = null;
        if (auditLog != null)
        {
            entityChangeList = AuditLogChangesHelper.GetAuditLogChanges(ChangeTracker.Entries().ToList());
        }

        int result = await base.SaveChangesAsync(cancellationToken);

        if (entityChangeList != null)
        {
            auditLog!.AuditLogChanges.AddRange(entityChangeList);
        }

        return result;
    }

    public override int SaveChanges()
    {
        return SaveChangesAsync().GetAwaiter().GetResult();
    }
}
