namespace AuditLogManager.Sample.AspNetCore.Models;

public record Todo
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public bool IsDone { get; set; }

    public DateTime CreateDate { get; set; }
}