namespace AuditLogManager.Infrastructure;

public class AuditLogProcessor(ILogger<AuditLogProcessor> logger,
    IConfiguration configuration,
    IServiceProvider serviceProvider,
    AuditLogQueue auditLogQueue) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var delayInMilliseconds = configuration.GetValue<int>("Worker:DelayInMilliseconds");

            if (!auditLogQueue.IsEmpty())
            {
                var commitCount = configuration.GetValue<int>("Worker:CommitCount");
                var auditLogs = new HashSet<AuditLog>();
                int count = 0;

                while (!auditLogQueue.IsEmpty() && count < commitCount)
                {
                    auditLogs.Add(auditLogQueue.Dequeue());
                    count++;
                }

                try
                {
                    using var scope = serviceProvider.CreateScope();
                    var provider = scope.ServiceProvider;
                    using var dbContext = provider.GetRequiredService<AppDbContext>();

                    await dbContext.BulkInsertAsync(auditLogs, bulkConfig =>
                    {
                        bulkConfig.IncludeGraph = true;
                    }, cancellationToken: stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to save audit logs.");
                }
            }

            await Task.Delay(delayInMilliseconds, stoppingToken);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (!auditLogQueue.IsEmpty())
        {
            var auditLogs = new HashSet<AuditLog>();
            while (!auditLogQueue.IsEmpty())
            {
                auditLogs.Add(auditLogQueue.Dequeue());
            }

            try
            {
                using var scope = serviceProvider.CreateScope();
                var provider = scope.ServiceProvider;
                using var dbContext = provider.GetRequiredService<AppDbContext>();

                await dbContext.BulkInsertAsync(auditLogs, bulkConfig =>
                {
                    bulkConfig.IncludeGraph = true;
                }, cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to save audit logs.");
            }
        }

        await base.StopAsync(cancellationToken);
    }
}