namespace Infrastructure.Network.Package.Handlers;

public class LoginPackageHandler(
    ServerOptions serverOptions,
    ServerEncryption serverEncryption,
    IPlayerManager playerManager,
    IMojangAuthService mojangAuthService) 
        : BasePackageHandler, ILoginPackageHandler
{
    internal override List<ServerBoundPackage> _packages { get; } =
    [
        new LoginStartServerBoundPackage(serverOptions, serverEncryption, playerManager, mojangAuthService),
        new EncryptionResponseServerBoundPackage(serverOptions, serverEncryption, mojangAuthService),
        new LoginAcknowledgedServerBoundPackage(),
    ];
}
