using AuditLogManager.Sample.AspNetCore.AuditLogging;
using AuditLogManager.Sample.AspNetCore.Models;
using Microsoft.EntityFrameworkCore;

namespace AuditLogManager.Sample.AspNetCore.Data;

public class AppDbContext : DbContext
{
    private readonly IAuditingManager _auditingManager;

    public AppDbContext(DbContextOptions<AppDbContext> options,
        IAuditingManager auditingManager) : base(options)
    {
        _auditingManager = auditingManager;
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
    }

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
        var auditLog = _auditingManager.Current;
        List<AuditLogChangeEntityEntry>? entityChangeList = null;
        if (auditLog != null)
        {
            entityChangeList = AuditLogChangesHelper.CreateAuditLogChangeEntry(ChangeTracker.Entries().ToList());
        }

        try
        {
            int result = await base.SaveChangesAsync(cancellationToken);

            if (entityChangeList != null)
            {
                AuditLogChangesHelper.UpdateAuditLogChangeEntry(entityChangeList);
                auditLog!.AuditLogChanges.AddRange(entityChangeList.Select(e => e.ToEntity()).ToList());
            }

            return result;
        }
        finally
        {
            ChangeTracker.AutoDetectChangesEnabled = true;
        }
    }

    public override int SaveChanges()
    {
        return SaveChangesAsync().GetAwaiter().GetResult();
    }
}
