namespace Infrastructure.Network.Interaction;

public class ConnectionHandler(
    ILogger logger,
    ILoginPackageHandler loginHandler,
    IStatusPackageHandler statusHandler,
    IPlayPackageHandler playPackageHandler,
    IConfigurationPackageHandler configurationHandler) : IConnectionHandler
{
    public async Task HandleAsync(IConnection connection, CancellationToken cancellationToken = default)
    {
        try
        {
            while (connection.Active && !cancellationToken.IsCancellationRequested)
            {
                if (connection.DataAvailable)
                {
                    var packageHeader = await connection.ReadIncomingPackageHeaderAsync(cancellationToken);

                    IBasePackageHandler packageHandler = connection.State switch
                    {
                        ConnectionState.Login => loginHandler,
                        ConnectionState.Status => statusHandler,
                        ConnectionState.Play => playPackageHandler,
                        ConnectionState.Configuration => configurationHandler,

                        _ => throw new Exception("Unreachable exception")
                    };

                    var handeled = await packageHandler.HandlePackageAsync(connection, packageHeader, cancellationToken);

                    if (!handeled && packageHeader.Length > 0)
                    {
                        _ = await connection.Stream.ReadBytesAsync(packageHeader.Length, cancellationToken);

                        logger.Warning(
                            "The received client package could not be processed.\r\n   [PackageId: '{0}', PackageLength: '{1}', ConnectionState: '{2}']\n",
                                "0x" + packageHeader.PackageId.ToString("X2"), packageHeader.Length, connection.State.ToString());
                    }
                }
            }
        }
        catch (Exception ex) 
        {
            logger.Error(ex, "Error occurred while handling  connection. Exception:");
        }
        finally 
        { 
            connection.Dispose(); 
        }
    }
}
