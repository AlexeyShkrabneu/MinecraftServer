namespace Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services, string dbConnectionString, bool isDevelopment)
    {
        services.AddDbContext<IServerDbContext, ServerDbContext>(options =>
        {
            options.UseNpgsql(dbConnectionString);
            options.UseLazyLoadingProxies();

            if (isDevelopment)
            {
                options.EnableDetailedErrors();
            }
        });

        return services;
    }
}
