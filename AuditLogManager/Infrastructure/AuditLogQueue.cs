namespace AuditLogManager.Infrastructure;

public class AuditLogQueue
{
    private readonly object _lock = new();
    private readonly Queue<AuditLog> _queue = new();

    public void Enqueue(AuditLog auditLog)
    {
        lock (_lock)
        {
            _queue.Enqueue(auditLog);
        }
    }

    public AuditLog Dequeue()
    {
        lock (_lock)
        {
            return _queue.Dequeue();
        }
    }

    public bool IsEmpty()
    {
        lock (_lock)
        {
            return _queue.Count == 0;
        }
    }
}