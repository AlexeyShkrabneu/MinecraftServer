using System.Net.Sockets;

namespace Application.Abstractions.Packages;

public class InboundPackage
    (int id, byte[] content)
{
    public readonly int Id = id;
    public readonly byte[] Content = content;

    private int _offset = 0;

    public byte ReadByte()
    {
        var b = Content[_offset];
        _offset += 1;
        return b;
    }

    public byte[] Read(int length)
    {
        var data = new byte[length];
        Array.Copy(Content, _offset, data, 0, length);
        _offset += length;
        return data;
    }

    public int ReadVarInt()
    {
        var value = 0;
        var size = 0;
        int b;
        while (((b = ReadByte()) & 0x80) == 0x80)
        {
            value |= (b & 0x7F) << (size++ * 7);
            if (size > 5)
            {
                throw new IOException("VarInt is too long!");
            }
        }
        return value | ((b & 0x7F) << (size * 7));
    }

    public ushort ReadUShort()
    {
        var ushortBuff = Read(2);
        Array.Reverse(ushortBuff);

        return BitConverter.ToUInt16(ushortBuff, 0);
    }

    public string ReadString()
    {
        var length = ReadVarInt();
        var data = Read(length);
        return Encoding.UTF8.GetString(data);
    }
}
