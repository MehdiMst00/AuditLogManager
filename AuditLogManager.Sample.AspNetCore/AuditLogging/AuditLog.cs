namespace AuditLogManager.Sample.AspNetCore.AuditLogging;

public class AuditLog
{
    public AuditLog()
    {
        AuditLogChanges = [];
    }

    public string? UserId { get; set; }
    public string? Header { get; set; }
    public string? IpAddress { get; set; }
    public string? HttpMethod { get; set; }
    public string? Area { get; set; }
    public string? ControllerName { get; set; }
    public string? ActionName { get; set; }
    public string? BrowserInfo { get; set; }
    public string? Url { get; set; }
    public string? Exceptions { get; set; }
    public int? HttpStatusCode { get; set; }
    public string? Arguments { get; set; }
    public int ExecutionDuration { get; set; }
    public List<AuditLogChange> AuditLogChanges { get; set; }
}
