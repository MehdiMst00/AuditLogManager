using AuditLogManager.Sample.AspNetCore.Utilities;

namespace AuditLogManager.Sample.AspNetCore.AuditLogging;

public static class AuditLoggingServiceCollectionExtension
{
    public static IServiceCollection AddAuditLog(this IServiceCollection services)
    {
        services.AddSingleton<AmbientContext>();
        services.AddScoped<AuditLogFilterAttribute>();
        services.AddSingleton<IAuditingManager, AuditingManager>();
        return services;
    }
}
