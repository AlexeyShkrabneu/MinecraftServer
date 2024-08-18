namespace Infrastructure.Network.Package.Handlers;

public class LoginPackageHandler(
    ServerOptions serverOptions,
    IPlayerManager playerManager,
    ServerEncryption serverEncryption) 
        : ILoginPackageHandler
{
    private readonly List<ServerBoundPackage> _loginsPackages = new()
    {
        new LoginStartServerBoundPackage(serverOptions, playerManager, serverEncryption),
        new EncryptionResponseServerBoundPackage(serverOptions, serverEncryption)
    };

    public async Task<bool> HandlePackageAsync(IConnection connection, IncomingPackageHeader packageHeader, CancellationToken cancellationToken = default)
    {
        var loginPackage = _loginsPackages.FirstOrDefault(package => package.Id.Equals(packageHeader.PackageId));
        if (loginPackage is null)
        {
            return false;
        }

        var clientBoundPackage = await loginPackage.HandleAsync(connection, cancellationToken);
        if (clientBoundPackage is null)
        {
            return false;
        }

        return await clientBoundPackage.RespondAsync(connection, cancellationToken);
    }
}
