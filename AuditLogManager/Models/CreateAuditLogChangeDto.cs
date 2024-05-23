using Newtonsoft.Json;

namespace AuditLogManager.Models;

public record CreateAuditLogChangeDto
{
    public string? TableName { get; set; }
    public AuditLogType Type { get; set; }
    public string? EntityId { get; set; } 
    public string? OldValues { get; set; } 
    public string? NewValues { get; set; }
    public List<string> ChangedColumns { get; set; } = [];

    public AuditLogChange ToAudit(IGuidGenerator guidGenerator)
    {
        return new AuditLogChange
        {
            Id = guidGenerator.Create(),
            TableName = TableName,
            EntityId = EntityId,
            OldValues = OldValues,
            NewValues = NewValues,
            AffectedColumns = ChangedColumns.Count == 0 ? null : JsonConvert.SerializeObject(ChangedColumns),
            Type = Type,
        };
    }
}