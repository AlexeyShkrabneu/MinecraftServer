namespace Infrastructure.Network.Package.ClientBound;

public abstract class ClientBoundPackage(int packageId)
{
    public readonly int Id = packageId;

    public abstract Task<bool> RespondAsync(IConnection connection, CancellationToken cancellationToken = default);
}
