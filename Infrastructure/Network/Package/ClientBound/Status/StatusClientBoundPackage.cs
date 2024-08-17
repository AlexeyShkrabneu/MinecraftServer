namespace Infrastructure.Network.Package.ClientBound.Status;

public class StatusClientBoundPackage
    (int packageId, ServerStatus serverStatus) 
        : ClientBoundPackage(packageId)
{
    public async override Task<bool> RespondAsync(IConnection connection, CancellationToken cancellationToken = default)
    {
        var serverStatusJson = JsonConvert.SerializeObject(serverStatus);

        await connection.Stream
            .WriteVarInt(Id)
            .WriteString(serverStatusJson)
            .SendAsync (cancellationToken);

        return true;
    }
}
