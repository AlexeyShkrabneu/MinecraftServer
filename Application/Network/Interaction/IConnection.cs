namespace Application.Network.Interaction;

public interface IConnection : IDisposable
{
    bool Active { get; }
    bool DataAvailable { get; }
    int ProtocolVersion { get; }
    ConnectionState State  { get; }
    IConnectionStream Stream { get; }
    IPlayerProfile PlayerProfile { get; }

    Task<IncomingPackageHeader> ReadIncomingPackageHeaderAsync(CancellationToken cancellationToken = default);
    void SetState(ConnectionState connectionState);
    void SetVerifyToken(byte[] verifyTokenBytes);
    bool ValidateVerifyToken(byte[] verifyTokenBytes);
    void UseEncryption(byte[] sharedKey);
    void SetPlayerProfile(IPlayerProfile player);
}
