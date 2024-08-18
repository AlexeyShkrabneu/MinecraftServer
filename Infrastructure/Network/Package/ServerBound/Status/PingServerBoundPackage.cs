namespace Infrastructure.Network.Package.ServerBound.Status;

public class PingServerBoundPackage()
    : ServerBoundPackage(ProtocolDefinition.PingPackageId)
{
    public async override Task<ClientBoundPackage> HandleAsync(IConnection connection, CancellationToken cancellationToken = default)
    {
        var pingTime = await connection.Stream.ReadLongAsync(cancellationToken);

        return new PingClientBoundPackage(Id, pingTime);
    }
}
