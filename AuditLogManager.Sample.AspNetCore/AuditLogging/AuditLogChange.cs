namespace AuditLogManager.Sample.AspNetCore.AuditLogging;

public class AuditLogChange
{
    public string? TableName { get; set; }
    public AuditLogType Type { get; set; }
    public string? EntityId { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public List<string> ChangedColumns { get; set; } = [];
}