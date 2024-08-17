namespace Infrastructure.Network.Package.ClientBound.Login;

public class LoginDisconnectCleintBoundPackage
    (TextComponent reason, int packageId)
        : ClientBoundPackage(packageId)
{
    public override Task<bool> RespondAsync(IConnection connection, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
