namespace Infrastructure.Network.Package.Handlers;

public class LoginPackageHandler(
    ServerOptions serverOptions,
    IPlayerManager playerManager,
    ServerEncryption serverEncryption) 
        : BasePackageHandler, ILoginPackageHandler
{
    internal override List<ServerBoundPackage> _packages { get; } =
    [
        new LoginStartServerBoundPackage(serverOptions, playerManager, serverEncryption),
        new EncryptionResponseServerBoundPackage(serverOptions, serverEncryption),
        new LoginAcknowledgedServerBoundPackage(),
    ];
}
