namespace AuditLogManager.Guids;

public static class SequentialGuidGeneratorServiceCollection
{
    public static IServiceCollection AddGuidenerator(this IServiceCollection services)
    {
        services.Configure<SequentialGuidGeneratorOptions>(options =>
        {
            options.DefaultSequentialGuidType = SequentialGuidType.SequentialAtEnd;
        });

        services.AddTransient<IGuidGenerator, SequentialGuidGenerator>();
        return services;
    }
}
