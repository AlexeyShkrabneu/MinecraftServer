namespace Infrastructure.Network;

public class Connection : IConnection, IDisposable
{
    private readonly TcpClient _tcpConnection;
    private readonly NetworkStream _connectionStream;
    
    public bool Connected => CheckConnection();
    public bool DataAvailable => _connectionStream.DataAvailable;

    public Connection(TcpClient tcpClient)
    {
        _tcpConnection = tcpClient;
        _connectionStream = tcpClient.GetStream();
    }

    public async Task<InboundPackage> ReadInboundPackageAsync(CancellationToken cancellationToken)
    {
        if (!DataAvailable)
        {
            return null;
        }

        var packageLength = ReadVarInt();

        if(packageLength == 0)
        {
            return null;
        }

        var packageIdLength = 0;
        var packageId = ReadVarInt(out packageIdLength);
        var contentLength = packageLength - packageIdLength;

        if(contentLength < 0)
        {
            return null;
        }

        var content = contentLength > 0
            ? await ReadBytesAsync(packageLength - packageIdLength, cancellationToken)
            : Array.Empty<byte>();

        return new InboundPackage(packageId, content);
    }

    public async Task<bool> SendPackageAsync(IOutboundPackage package, CancellationToken cancellationToken)
    {
        if (!Connected)
        {
            return false;
        }

        var dataBuffer = package.GetOutboundBytes();
        await _connectionStream.WriteAsync(dataBuffer, cancellationToken);

        return true;
    }

    public void Dispose()
    {
        _tcpConnection.Close();
        _tcpConnection.Dispose();

        _connectionStream.Close();
        _connectionStream.Dispose();
    }

    private int ReadVarInt() => ReadVarInt(out var _);
    private int ReadVarInt(out int size) 
    {
        size = 0;
        var value = 0;
        int b;

        while (((b = _connectionStream.ReadByte()) & 0x80) == 0x80)
        {
            value |= (b & 0x7F) << (size++ * 7);
            if (size > 5)
            {
                throw new IOException("VarInt too long.");
            }
        }

        return value | ((b & 0x7F) << (size * 7));
    }

    private async Task<byte[]> ReadBytesAsync(int length, CancellationToken cancellationToken)
    {
        var buffer = new byte[length];
        await _connectionStream.ReadAsync(buffer, 0, length);

        return buffer;
    }

    private bool CheckConnection()
    {
        try
        {
            if (_tcpConnection != null && _tcpConnection.Client != null && _tcpConnection.Client.Connected)
            {
                if (_tcpConnection.Client.Poll(0, SelectMode.SelectRead))
                {
                    var buff = new byte[1];
                    return _tcpConnection.Client.Receive(buff, SocketFlags.Peek) != 0;
                }

                return true;
            }
        }
        catch { /* Ignorred */ }

        return false;
    }
}
