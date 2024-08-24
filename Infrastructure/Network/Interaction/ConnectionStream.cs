namespace Infrastructure.Network.Interaction;

public class ConnectionStream(NetworkStream _networkStream) : IConnectionStream, IDisposable
{
    public bool DataAvailable => _networkStream.DataAvailable;

    private readonly List<byte> _responseBuffer = new();
    private readonly Dictionary<int, byte[]> _responseProperties = new();

    #region Encryption
    private bool _isEncryption { get; set; }
    private BufferedBlockCipher _decryptor { get; set; }
    private BufferedBlockCipher _encryptor { get; set; }

    public void UseEncryption(byte[] sharedKey)
    {
        _encryptor = new BufferedBlockCipher(new CfbBlockCipher(new AesEngine(), 8));
        _encryptor.Init(true, new ParametersWithIV(new KeyParameter(sharedKey), sharedKey, 0, 16));

        _decryptor = new BufferedBlockCipher(new CfbBlockCipher(new AesEngine(), 8));
        _decryptor.Init(false, new ParametersWithIV(new KeyParameter(sharedKey), sharedKey, 0, 16));

        _isEncryption = true;
    }
    #endregion

    #region Read
    public async Task<byte> ReadByteAsync(CancellationToken cancellationToken = default)
    {
        var buffer = new byte[1];
        await _networkStream.ReadAsync(buffer, 0, 1, cancellationToken);

        if (_isEncryption)
        {
            buffer = DecryptBytes(buffer);
        }

        return buffer[0];
    }

    public async Task<byte[]> ReadBytesAsync(int length, CancellationToken cancellationToken = default)
    {
        var buffer = new byte[length];
        await _networkStream.ReadAsync(buffer, 0, length, cancellationToken);

        if (_isEncryption)
        {
            buffer = DecryptBytes(buffer);
        }

        return buffer;
    }

    public async Task<Guid> ReadGuidAsync(CancellationToken cancellation = default)
    {
        var bytes = await ReadBytesAsync(16, cancellation);
        return new Guid(bytes, true);
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
        var stringBytes = await ReadByteArrayAsync(cancellationToken);

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

    public async Task<bool> ReadBool(CancellationToken cancellationToken = default)
    {
        var boolByte = await ReadByteAsync(cancellationToken);
        return boolByte is 0x01;
    }

    public async Task<byte[]> ReadByteArrayAsync(CancellationToken cancellationToken = default)
    {
        var length = await ReadVarIntAsync(cancellationToken);
        return await ReadBytesAsync(length, cancellationToken);
    }
    #endregion

    #region Write
    public IConnectionStream WriteBytes(params byte[] value)
    {
        _responseBuffer.AddRange(value);

        return this;
    }

    public IConnectionStream WriteLong(long value) => WriteBytes(ToBytes(value));
    public IConnectionStream WriteGuid(Guid value) => WriteBytes(ToBytes(value));
    public IConnectionStream WriteBool(bool value) => WriteBytes(ToBytes(value));
    public IConnectionStream WriteString(string value) => WriteBytes(ToBytes(value));
    public IConnectionStream WriteVarInt(int value) => WriteBytes(ToBytesVarInt(value));
    public IConnectionStream WriteInt(int value) => WriteBytes(ToBytes(value));
    
    public IConnectionStream WriteByteArrayWithLength(byte[] value)
    {
        WriteVarInt(value.Length);
        WriteBytes(value);

        return this;
    }

    public IConnectionStream WriteProperties(PlayerProperty[] properties)
    {
        var propsLength = ToBytesVarInt(properties.Length);
        var buffer = new List<byte>(propsLength);

        foreach(var prop in properties)
        {
            buffer.AddRange(ToBytes(prop.Name));
            buffer.AddRange(ToBytes(prop.Value));
            
            buffer.Add(ToBytes(prop.IsSigned));

            if (prop.IsSigned)
            {
                buffer.AddRange(ToBytes(prop.Signature));
            }
        }

        WriteBytes(buffer.ToArray());

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

        dataToSend = _responseBuffer.ToArray();

        if (_isEncryption)
        {
            dataToSend = EncryptBytes(dataToSend);
        }

        await _networkStream.WriteAsync(dataToSend, cancellationToken);
        await _networkStream.FlushAsync(cancellationToken);

        _responseBuffer.Clear();
    }
    #endregion

    public void Close() => _networkStream?.Close();
    public void Dispose() => _networkStream?.Dispose();

    private byte[] EncryptBytes(byte[] plainBytes) => _encryptor.ProcessBytes(plainBytes);
    private byte[] DecryptBytes(byte[] cipherBytes) => _decryptor.ProcessBytes(cipherBytes);

    private byte[] ToBytesVarInt(int value)
    {
        var buffer = new List<byte>();
        do
        {
            byte temp = (byte)(value & 0b01111111);
            value >>>= 7;
            if (value != 0)
            {
                temp |= 0b10000000;
            }

            buffer.Add(temp);
        } while (value != 0);

        return buffer.ToArray();
    }

    private byte[] ToBytes(int value)
    {
        return BitConverter.GetBytes(value);
    }

    private byte[] ToBytes(long value)
    {
        return BitConverter.GetBytes(IPAddress.HostToNetworkOrder(value));
    }

    private byte[] ToBytes(string value)
    {
        var buffer = new List<byte>(); 

        var encodedValue = Encoding.UTF8.GetBytes(value);
        buffer.AddRange(ToBytesVarInt(encodedValue.Length));
        buffer.AddRange(encodedValue);

        return buffer.ToArray();
    }

    private byte ToBytes(bool value)
    {
        return value ? (byte)0x01 : (byte)0x00;
    }

    private byte[] ToBytes(Guid value)
    {
        return value.ToByteArray(true);
    }
}
