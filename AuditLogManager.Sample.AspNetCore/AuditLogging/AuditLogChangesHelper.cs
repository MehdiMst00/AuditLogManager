using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AuditLogManager.Sample.AspNetCore.AuditLogging;

public static class AuditLogChangesHelper
{
    public static List<AuditLogChange> GetAuditLogChanges(List<EntityEntry> entityEntries)
    {
        var entityChanges = new List<AuditLogChange>();

        foreach (var entry in entityEntries
            .Where(e => e.State == EntityState.Modified ||
                        e.State == EntityState.Deleted ||
                        e.State == EntityState.Added))
        {
            var auditLogChnages = new AuditLogChange
            {
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

                if (entry.State == EntityState.Added)
                {
                    auditLogChnages.Type = AuditLogType.Create;
                    auditLogChnages.SetNewValues(propertyName, property.CurrentValue);
                }
                else if (entry.State == EntityState.Modified || HasChangedOwnedEntities(entry))
                {
                    if (property.IsModified)
                    {
                        auditLogChnages.ChangedColumns.Add(propertyName);
                        auditLogChnages.Type = AuditLogType.Update;
                        auditLogChnages.SetOldValues(propertyName, property.OriginalValue);
                        auditLogChnages.SetNewValues(propertyName, property.CurrentValue);
                    }
                }
                else if (entry.State == EntityState.Deleted)
                {
                    auditLogChnages.Type = AuditLogType.Delete;
                    auditLogChnages.SetOldValues(propertyName, property.OriginalValue);
                }
            }

            entityChanges.Add(auditLogChnages);
        }

        return entityChanges;
    }

    private static bool HasChangedOwnedEntities(EntityEntry entry) =>
        entry.References.Any(r =>
            r.TargetEntry != null &&
            r.TargetEntry.Metadata.IsOwned() &&
           (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
}
