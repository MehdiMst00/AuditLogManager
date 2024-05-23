using Newtonsoft.Json;

namespace AuditLogManager.Sample.AspNetCore.AuditLogging;

public class AuditLogChange
{
    public string? TableName { get; set; }
    public AuditLogType Type { get; set; }

    public string? EntityId { get; set; }

    private Dictionary<string, object?> _oldValues = [];
    public string? OldValues => _oldValues.Count != 0 ? JsonConvert.SerializeObject(_oldValues) : null;
    public void SetOldValues(string key, object? obj)
    {
        _oldValues[key] = obj;
    }

    private Dictionary<string, object?> _newValues = [];
    public string? NewValues => _newValues.Count != 0 ? JsonConvert.SerializeObject(_newValues) : null;
    public void SetNewValues(string key, object? obj)
    {
        _newValues[key] = obj;
    }

    public List<string> ChangedColumns { get; set; } = [];
}