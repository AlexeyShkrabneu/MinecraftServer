namespace Infrastructure.Services;

public class ClientManagerService
    (Serilog.ILogger logger)
        : IClientManagerService
{
    public async Task<IConnection> HandshakeAsync(TcpClient tcpClient, CancellationToken cancellationToken = default)
    {
        tcpClient.SendTimeout = 1000;
        tcpClient.ReceiveTimeout = 1000;

        var connection = new Connection(tcpClient);

        var inboundPackage = await connection.ReadInboundPackageAsync(cancellationToken);
        
        if (inboundPackage is null)
        {
            return null;
        }

        var handshake = HandshakeInPackage.Parse(inboundPackage);
        logger.Information("Client connected");
        return null;
    }
}