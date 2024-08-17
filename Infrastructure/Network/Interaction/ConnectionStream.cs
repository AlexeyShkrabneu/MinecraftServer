namespace Infrastructure.Network.Interaction;

public class ConnectionStream(NetworkStream _networkStream) : IConnectionStream, IDisposable
{
    public bool DataAvailable => _networkStream.DataAvailable;
    private readonly List<byte> _responseBuffer = new();

    #region Read
    public async Task<byte> ReadByteAsync(CancellationToken cancellationToken = default)
    {
        var buffer = new byte[1];
        await _networkStream.ReadAsync(buffer, 0, 1, cancellationToken);

        return buffer[0];
    }

    public async Task<byte[]> ReadBytesAsync(int length, CancellationToken cancellationToken = default)
    {
        var buffer = new byte[length];
        await _networkStream.ReadAsync(buffer, 0, length, cancellationToken);

        return buffer;
    }

    public async Task<int> ReadVarIntAsync(CancellationToken cancellationToken = default)
    {
        var value = 0;
        var size = 0;
        int b;

        while (((b = await ReadByteAsync(cancellationToken)) & 0x80) == 0x80)
        {
            value |= (b & 0x7F) << (size++ * 7);
            if (size > 5)
            {
                throw new IOException("VarInt too long.");
            }
        }

        return value | ((b & 0x7F) << (size * 7));
    }

    public async Task<string> ReadStringAsync(CancellationToken cancellationToken = default)
    {
        var length = await ReadVarIntAsync(cancellationToken);
        var stringBytes = await ReadBytesAsync(length, cancellationToken);

        return Encoding.UTF8.GetString(stringBytes);
    }

    public async Task<ushort> ReadUShortAsync(CancellationToken cancellationToken = default)
    {
        var ushortBytes = await ReadBytesAsync(2, cancellationToken);

        Array.Reverse(ushortBytes);

        return BitConverter.ToUInt16(ushortBytes);
    }

    public async Task<long> ReadLongAsync(CancellationToken cancellationToken = default)
    {
        var longBytes = await ReadBytesAsync(8);

        Array.Reverse(longBytes);

        return BitConverter.ToInt64(longBytes);
    }
    #endregion

    #region Write
    public IConnectionStream WriteVarInt(int value)
    {
        do
        {
            byte temp = (byte)(value & 0b01111111);
            value >>>= 7;
            if (value != 0)
            {
                temp |= 0b10000000;
            }

            _responseBuffer.Add(temp);
        } while (value != 0);

        return this;
    }

    public IConnectionStream WriteLong(long value)
    {
        _responseBuffer.AddRange(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(value)));

        return this;
    }

    public IConnectionStream WriteString(string value)
    {
        var encodedValue = Encoding.UTF8.GetBytes(value);

        WriteVarInt(encodedValue.Length);
        _responseBuffer.AddRange(encodedValue);

        return this;
    }

    public IConnectionStream WriteBytes(byte[] value)
    {
        _responseBuffer.AddRange(value);

        return this;
    }

    public async Task SendAsync(CancellationToken cancellationToken = default)
    {
        if (_responseBuffer.Count == 0)
        {
            return;
        }

        var dataToSend = _responseBuffer.ToArray();
        _responseBuffer.Clear();

        WriteVarInt(dataToSend.Length);
        WriteBytes(dataToSend);

        await _networkStream.WriteAsync(_responseBuffer.ToArray(), cancellationToken);
        await _networkStream.FlushAsync(cancellationToken);

        _responseBuffer.Clear();
    }
    #endregion

    public void Close() => _networkStream.Close();
    public void Dispose() => _networkStream.Dispose();
}
