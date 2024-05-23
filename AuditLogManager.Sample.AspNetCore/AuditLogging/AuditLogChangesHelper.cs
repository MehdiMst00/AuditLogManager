using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AuditLogManager.Sample.AspNetCore.AuditLogging;

public static class AuditLogChangesHelper
{
    public static List<AuditLogChangeEntityEntry> CreateAuditLogChangeEntry(List<EntityEntry> entityEntries)
    {
        var entityChanges = new List<AuditLogChangeEntityEntry>();

        foreach (var entry in entityEntries
            .Where(e => e.State == EntityState.Modified ||
                        e.State == EntityState.Deleted ||
                        e.State == EntityState.Added))
        {
            var auditLogChnages = new AuditLogChangeEntityEntry
            {
                EntityEntry = entry,
                EntityEntryState = entry.State,
                TableName = entry.Entity.GetType().Name
            };

            foreach (var property in entry.Properties)
            {
                string propertyName = property.Metadata.Name;
                if (property.Metadata.IsPrimaryKey())
                {
                    if (property.EntityEntry.IsKeySet)
                        auditLogChnages.EntityId = property.CurrentValue?.ToString();
                    continue;
                }

                if (auditLogChnages.EntityEntryState == EntityState.Added)
                {
                    auditLogChnages.Type = AuditLogType.Create;
                    auditLogChnages.NewValues[propertyName] = property.CurrentValue;
                }
                else if (entry.State == EntityState.Modified || HasChangedOwnedEntities(entry))
                {
                    if (property.IsModified)
                    {
                        auditLogChnages.ChangedColumns.Add(propertyName);
                        auditLogChnages.Type = AuditLogType.Update;
                        auditLogChnages.OldValues[propertyName] = property.OriginalValue;
                        auditLogChnages.NewValues[propertyName] = property.CurrentValue;
                    }
                }
                else if (entry.State == EntityState.Deleted)
                {
                    auditLogChnages.Type = AuditLogType.Delete;
                    auditLogChnages.OldValues[propertyName] = property.OriginalValue;
                }
            }

            entityChanges.Add(auditLogChnages);
        }

        return entityChanges;
    }

    public static void UpdateAuditLogChangeEntry(List<AuditLogChangeEntityEntry> entityEntries)
    {
        foreach (var entry in entityEntries)
        {
            foreach (var property in entry.EntityEntry.Properties)
            {
                // Update entity id
                string propertyName = property.Metadata.Name;
                if (property.Metadata.IsPrimaryKey())
                {
                    entry.EntityId = property.CurrentValue?.ToString();
                    continue;
                }
            }
        }
    }

    private static bool HasChangedOwnedEntities(EntityEntry entry) =>
        entry.References.Any(r =>
            r.TargetEntry != null &&
            r.TargetEntry.Metadata.IsOwned() &&
           (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
}
