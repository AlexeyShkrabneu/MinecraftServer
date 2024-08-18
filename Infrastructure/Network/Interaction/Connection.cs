namespace Infrastructure.Network.Interaction;

public class Connection : IConnection
{
    public bool Active => CheckConnection();
    public bool DataAvailable => _connectionStream.DataAvailable;
    public IConnectionStream Stream => _connectionStream;
    public int ProtocolVersion { get; }
    public IPlayer Player { get; private set; } 
    public ConnectionState State { get; private set; }

    private byte[] _verifyToken { get; set; }

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

    public static async Task<IConnection> HandshakeAsync(TcpClient client, CancellationToken cancellationToken = default)
    {
        client.SendTimeout = 3000;
        client.ReceiveTimeout = 3000;

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

        client.SendTimeout = 10000;
        client.ReceiveTimeout = 10000;

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

    public async Task<IncomingPackageHeader> ReadIncomingPackageHeaderAsync(CancellationToken cancellationToken = default)
    {
        var length = await _connectionStream.ReadVarIntAsync(cancellationToken);
        var id = await _connectionStream.ReadVarIntAsync(cancellationToken);

        return new IncomingPackageHeader(id, length);
    }

    public bool ValidateVerifyToken(byte[] verifyTokenBytes)
    {
        if (_verifyToken is null || verifyTokenBytes is null 
            || _verifyToken.Length != verifyTokenBytes.Length) 
        {
            return false;
        }

        return verifyTokenBytes.SequenceEqual(_verifyToken);
    }

    public void ChangeState(ConnectionState connectionState) => State = connectionState;
    public void SetVerifyToken(byte[] verifyTokenBytes) => _verifyToken = verifyTokenBytes;
    public void UseEncryption(byte[] sharedKey) => _connectionStream.UseEncryption(sharedKey);
    public void SetPlayer(IPlayer player) => Player = player;

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
