namespace Infrastructure.Network.Interaction;

public class Connection : IConnection
{
    public bool Active => CheckConnection();
    public int ProtocolVersion { get; }
    public ConnectionState State { get; private set; }

    private readonly TcpClient _tcpClient;
    private readonly ConnectionStream _connectionStream;

    private Connection(
        TcpClient client,
        ConnectionStream stream,
        ConnectionState state,
        int pVersion)
    {
        _tcpClient = client;
        _connectionStream = stream;

        State = state;
        ProtocolVersion = pVersion;
    }

    public static async Task<Connection> HandshakeAsync(TcpClient client, CancellationToken cancellationToken = default)
    {
        var connectionStream = new ConnectionStream(client.GetStream());

        var packageLength = await connectionStream.ReadVarIntAsync(cancellationToken);
        var packageId = await connectionStream.ReadVarIntAsync(cancellationToken);

        if(packageId != ProtocolDefinition.HandshakePackageId)
        {
            return disposeAndDefault();
        }

        var pVersion = await connectionStream.ReadVarIntAsync(cancellationToken);
        var host = await connectionStream.ReadStringAsync(cancellationToken);
        var port = await connectionStream.ReadUShortAsync(cancellationToken);
        var nextState = await connectionStream.ReadVarIntAsync(cancellationToken);

        if (!Enum.IsDefined(typeof(ConnectionState), nextState))
        {
            return disposeAndDefault();
        }

        return new Connection(client, connectionStream, (ConnectionState)nextState, pVersion);

        Connection disposeAndDefault()
        {
            connectionStream.Close();
            connectionStream.Dispose();

            client.Close();
            client.Dispose();

            return null;
        }
    }

    public void Dispose()
    {
        _connectionStream.Close();
        _connectionStream.Dispose();

        _tcpClient.Close();
        _tcpClient.Dispose();
    }

    private bool CheckConnection()
    {
        try
        {
            if (_tcpClient != null && _tcpClient.Client != null && _tcpClient.Client.Connected)
            {
                if (_tcpClient.Client.Poll(0, SelectMode.SelectRead))
                {
                    var buff = new byte[1];
                    return _tcpClient.Client.Receive(buff, SocketFlags.Peek) != 0;
                }

                return true;
            }
        }
        catch { /* Ignorred */ }

        return false;
    }
}
