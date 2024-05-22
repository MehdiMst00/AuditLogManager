namespace AuditLogManager;

public static class ConfigureServices
{
    public static void AddServices(this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddControllers();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddGuidenerator();
        services.AddSingleton<AuditLogQueue>();
        services.AddHostedService<AuditLogProcessor>();

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("Default"));
        });
    }
}