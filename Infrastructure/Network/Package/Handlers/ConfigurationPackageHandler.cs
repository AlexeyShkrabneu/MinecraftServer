namespace Infrastructure.Network.Package.Handlers;

public class ConfigurationPackageHandler : IConfigurationPackageHandler
{
    public Task<bool> HandlePackageAsync(IConnection connection, IncomingPackageHeader packageHeader, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
