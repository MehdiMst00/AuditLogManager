namespace AuditLogManager.Models;

public class AuditLogChange
{
    #region Relations

    public Guid AuditLogId { get; set; }
    public AuditLog? AuditLog { get; set; }

    #endregion

    #region Ctor

    public AuditLogChange()
    {
        
    }

    #endregion

    #region Properties

    public Guid Id { get; set; }
    public string? TableName { get; set; }
    public AuditLogType Type { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? AffectedColumns { get; set; }
    public string? EntityId { get; set; }

    #endregion
}