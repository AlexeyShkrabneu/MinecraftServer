namespace Infrastructure.Packages.Inbound;

public class HandshakeInPackage
{
    public readonly ushort ServerPort;
    public readonly string ServerAddress;

    public readonly int ProtocollVersion;
    public readonly NextState NextState;

    private HandshakeInPackage(ushort port, string address, int pVersion, NextState nxtState) 
    {
        ServerPort = port;
        ServerAddress = address;
        ProtocollVersion = pVersion;
        NextState = nxtState;
    }

    public static HandshakeInPackage Parse(InboundPackage inboundPackage)
    {
        if(inboundPackage == null || inboundPackage.Id != 0x00)
        {
            return null;
        }

        var pVersion = inboundPackage.ReadVarInt();
        var address = inboundPackage.ReadString();
        var port = inboundPackage.ReadUShort();
        var nxtState = (NextState)inboundPackage.ReadVarInt();

        return new HandshakeInPackage(port, address, pVersion, nxtState);
    }
}
