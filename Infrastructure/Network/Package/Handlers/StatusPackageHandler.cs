namespace Infrastructure.Network.Package.Handlers;

public class StatusPackageHandler(
    ServerOptions serverOptions,
    IPlayerManager playerManager) 
        : IStatusPackageHandler
{
    private List<ServerBoundPackage> _statusPackages = new()
    {
        new StatusServerBoundPackage(ProtocolDefinition.StatusPackageId, serverOptions, playerManager),
        new PingServerBoundPackage(ProtocolDefinition.PingPackageId)
    };

    public async Task<bool> HandlePackageAsync(IConnection connection, IncomingPackageHeader packageHeader, CancellationToken cancellationToken = default)
    {
        var statusPackage = _statusPackages.FirstOrDefault(package => package.Id.Equals(packageHeader.PackageId));
        if (statusPackage is null) 
        {
            return false;
        }

        var clientBoundPackage = await statusPackage.HandleAsync(connection, cancellationToken);
        if (clientBoundPackage is null) 
        {
            return false;
        }

        return await clientBoundPackage.RespondAsync(connection, cancellationToken);
    }
}
