namespace AuditLogManager.Sample.AspNetCore.AuditLogging;

public interface IAuditingManager
{
    public AuditLog? Current { get; set; }
}