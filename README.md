# Audit Log Manager

## AuditLogManager Project
- Background worker to commit logs to database (SQL Server) using `AuditLogQueue`
- AuditLogController to enqueue log with HTTP POST method (You can also add gRPC)
- Worker setting in appsettings.json:
```json
  "Worker": {
    "DelayInMilliseconds": 2000,
    "CommitCount": 200
  }
```

## AuditLogManager.Sample.AspNetCore Project
- `AuditLogFilterAttribute` for create log object and POST to `AuditLogManager` project (You can combine AuditLogManager and AuditLogManager.Sample.AspNetCore project for removing remote call)
- SaveChangesAsync override in `AppDbContext` for getting entity change values:
```c#
var auditLog = _auditingManager.Current;
List<AuditLogChangeEntityEntry>? entityChangeList = null;
if (auditLog != null)
{
    entityChangeList = AuditLogChangesHelper.CreateAuditLogChangeEntry(ChangeTracker.Entries().ToList());
}

try
{
    int result = await base.SaveChangesAsync(cancellationToken);

    if (entityChangeList != null)
    {
        AuditLogChangesHelper.UpdateAuditLogChangeEntry(entityChangeList);
        auditLog!.AuditLogChanges.AddRange(entityChangeList.Select(e => e.ToEntity()).ToList());
    }

    return result;
}
finally
{
    ChangeTracker.AutoDetectChangesEnabled = true;
}
```
- Add `AuditLogFilterAttribute` globally or use per each controller class or action method:
```c#
// Program.cs
builder.Services.AddControllers(options =>
{
    options.Filters.Add<AuditLogFilterAttribute>();
});

// OR

// TodoesController.cs
[ServiceFilter(typeof(AuditLogFilterAttribute))]
public class TodoesController : ControllerBase
...
```
- Use `DisableAuditLogFilter` for disable auditing 
```c#
// GET: api/Todoes/5
[HttpGet("{id}")]
[DisableAuditLogFilter]
public async Task<ActionResult<Todo>> GetTodo(int id)
```

## AuditLogManager.AppHost (Aspire)
- Run project using [Aspire](https://learn.microsoft.com/en-us/dotnet/aspire/get-started/aspire-overview) :)
