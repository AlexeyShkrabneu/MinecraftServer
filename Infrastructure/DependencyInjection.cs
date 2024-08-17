namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        services.AddHandlers();

        services.AddManagers();

        return services;
    }

    private static void AddHandlers(this IServiceCollection services)
    {
        services.AddSingleton<IConnectionHandler, ConnectionHandler>();

        services.AddSingleton<IPlayPackageHandler, PlayPackageHandler>();
        services.AddSingleton<ILoginPackageHandler, LoginPackageHandler>();
        services.AddSingleton<IStatusPackageHandler, StatusPackageHandler>();
        services.AddSingleton<IConfigurationPackageHandler, ConfigurationPackageHandler>();
    }

    private static void AddManagers(this IServiceCollection services)
    {
        services.AddSingleton<IPlayerManager, PlayerManager>();
    }
}
