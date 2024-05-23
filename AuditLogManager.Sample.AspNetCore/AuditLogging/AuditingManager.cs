using AuditLogManager.Sample.AspNetCore.Utilities;

namespace AuditLogManager.Sample.AspNetCore.AuditLogging;

public class AuditingManager(AmbientContext ambientContext) : IAuditingManager
{
    private const string AuditLogKey = "CurrentAuditLog";

    public AuditLog? Current
    {
        get => ambientContext.Get<AuditLog>(AuditLogKey);
        set => ambientContext.Set(AuditLogKey, value);
    }
}