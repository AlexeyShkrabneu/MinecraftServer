namespace Infrastructure.Network.Package.Handlers;

public class StatusPackageHandler(
    ServerOptions serverOptions,
    IPlayerManager playerManager) 
        : BasePackageHandler, IStatusPackageHandler
{
    internal override List<ServerBoundPackage> _packages { get; } =
    [
        new StatusServerBoundPackage(serverOptions, playerManager),
        new PingServerBoundPackage()
    ];
}
