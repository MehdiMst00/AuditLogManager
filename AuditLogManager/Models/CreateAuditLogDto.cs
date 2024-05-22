namespace AuditLogManager.Models;

public record CreateAuditLogDto
{
    public CreateAuditLogDto()
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
    public List<CreateAuditLogChangeDto> AuditLogChanges { get; set; }

    public AuditLog ToAudit(IGuidGenerator guidGenerator)
    {
        return new AuditLog
        {
            Id = guidGenerator.Create(),
            UserId = UserId,
            ExecutionTime = DateTime.UtcNow,
            BrowserInfo = BrowserInfo,
            HttpMethod = HttpMethod,
            Url = Url,
            HttpStatusCode = HttpStatusCode,
            Exceptions = Exceptions,
            Area = Area,
            ControllerName = ControllerName,
            ActionName = ActionName,
            IpAddress = IpAddress,
            Header = Header,
            Arguments = Arguments,
            AuditLogChanges = AuditLogChanges.Select(a => a.ToAudit(guidGenerator)).ToList()
        };
    }
}
