namespace Application.Network.Interaction;

public interface IConnection : IDisposable
{
    bool Active { get; }
    bool DataAvailable { get; }
    int ProtocolVersion { get; }
    ConnectionState State  { get; }
    IConnectionStream Stream { get; }
    IPlayer Player { get; }

    Task<IncomingPackageHeader> ReadIncomingPackageHeaderAsync(CancellationToken cancellationToken = default);
    void ChangeState(ConnectionState connectionState);
    void SetVerifyToken(byte[] verifyTokenBytes);
    bool ValidateVerifyToken(byte[] verifyTokenBytes);
    void UseEncryption(byte[] sharedKey);
    void SetPlayer(IPlayer player);
}
