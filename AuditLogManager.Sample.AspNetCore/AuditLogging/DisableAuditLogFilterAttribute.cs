namespace AuditLogManager.Sample.AspNetCore.AuditLogging;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class DisableAuditLogFilterAttribute : Attribute
{
}
