namespace Infrastructure.Network.Interaction;

public class ConnectionHandler : IConnectionHandler
{
    public async Task HandleAsync(IConnection connection, CancellationToken cancellationToken = default)
    {
        while (connection.Active && !cancellationToken.IsCancellationRequested) 
        {
            if (connection.DataAvailable)
            {
                var packageHeader = await connection.ReadIncomingPackageHeaderAsync(cancellationToken);
                
                connection.Dispose();
            }
        }

        connection.Dispose();
    }
}
