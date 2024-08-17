namespace Application.Network.Interaction;

public interface IConnection : IDisposable
{
    bool Active { get; }
    bool DataAvailable { get; }
    int ProtocolVersion { get; }
    ConnectionState State  { get; }
    IConnectionStream Stream { get; }

    Task<IncomingPackageHeader> ReadIncomingPackageHeaderAsync(CancellationToken cancellationToken = default);
}
