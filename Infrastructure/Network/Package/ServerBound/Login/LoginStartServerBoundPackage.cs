namespace Infrastructure.Network.Package.ServerBound.Login;

public class LoginStartServerBoundPackage
    (int packageId)
        : ServerBoundPackage(packageId)
{
    public async override Task<ClientBoundPackage> HandleAsync(IConnection connection, CancellationToken cancellationToken = default)
    {
        var userName = await connection.Stream.ReadStringAsync(cancellationToken);
        var id = await connection.Stream.ReadStringAsync(cancellationToken);

        if (userName.Length > 16)
        {

        }

        throw new NotImplementedException();
    }
}
