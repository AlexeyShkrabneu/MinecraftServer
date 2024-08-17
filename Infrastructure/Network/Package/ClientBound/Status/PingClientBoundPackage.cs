namespace Infrastructure.Network.Package.ClientBound.Status;

public class PingClientBoundPackage
    (int packageId, long time)
        : ClientBoundPackage(packageId)
{
    public async override Task<bool> RespondAsync(IConnection connection, CancellationToken cancellationToken = default)
    {
        await connection.Stream
            .WriteVarInt(Id)
            .WriteLong(time)
            .SendAsync(cancellationToken);

        return true;
    }
}
