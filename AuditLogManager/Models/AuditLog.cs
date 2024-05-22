namespace AuditLogManager.Models;

public record AuditLog
{
    #region Relations

    public ICollection<AuditLogChange> AuditLogChanges { get; set; }

    #endregion

    #region Ctor

    public AuditLog()
    {
        AuditLogChanges ??= [];
    }

    #endregion

    #region Properties
    
    public Guid Id { get; set; }
    public string? UserId { get; set; }
    public string? IpAddress { get; set; }
    public string? HttpMethod { get; set; }
    public string? Area { get; set; }
    public string? ControllerName { get; set; }
    public string? ActionName { get; set; }
    public string? BrowserInfo { get; set; }
    public string? Url { get; set; }
    public string? Exceptions { get; set; }
    public string? Header { get; set; }
    public int? HttpStatusCode { get; set; }
    public string? Arguments { get; set; }
    public DateTime ExecutionTime { get; set; }

    #endregion
}