namespace Infrastructure.Network.Package.ServerBound;

public abstract class ServerBoundPackage(int packageId)
{
    public readonly int Id = packageId;

    public abstract Task<ClientBoundPackage> HandleAsync(IConnection connection, CancellationToken cancellationToken = default);
}
