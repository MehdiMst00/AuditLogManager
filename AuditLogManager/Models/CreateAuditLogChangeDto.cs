using Newtonsoft.Json;

namespace AuditLogManager.Models;

public record CreateAuditLogChangeDto
{
    public string? TableName { get; set; }
    public AuditLogType Type { get; set; }
    public string? KeyValues { get; }
    public string? OldValues { get; }
    public string? NewValues { get; }
    public List<string> ChangedColumns { get; } = [];

    public AuditLogChange ToAudit(IGuidGenerator guidGenerator)
    {
        return new AuditLogChange
        {
            Id = guidGenerator.Create(),
            TableName = TableName,
            PrimaryKey = KeyValues,
            OldValues = OldValues,
            NewValues = NewValues,
            AffectedColumns = ChangedColumns.Count == 0 ? null : JsonConvert.SerializeObject(ChangedColumns),
            Type = Type,
        };
    }
}