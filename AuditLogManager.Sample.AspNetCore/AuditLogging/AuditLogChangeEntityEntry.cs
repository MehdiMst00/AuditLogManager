using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;

namespace AuditLogManager.Sample.AspNetCore.AuditLogging;

public class AuditLogChangeEntityEntry
{
    public string? TableName { get; set; }
    public AuditLogType Type { get; set; }
    public string? EntityId { get; set; }
    public Dictionary<string, object?> OldValues { get; set; } = [];
    public Dictionary<string, object?> NewValues { get; set; } = [];
    public List<string> ChangedColumns { get; set; } = [];
    public EntityEntry EntityEntry { get; set; } = default!;
    public EntityState EntityEntryState { get; set; }

    public AuditLogChange ToEntity()
    {
        return new AuditLogChange
        {
            EntityId = EntityId,
            Type = Type,
            TableName = TableName,
            NewValues = NewValues.Count != 0 ? JsonConvert.SerializeObject(NewValues) : null,
            OldValues = OldValues.Count != 0 ? JsonConvert.SerializeObject(OldValues) : null,
            ChangedColumns = ChangedColumns,
        };
    }
}