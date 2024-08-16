namespace Application.Network.Interaction;

public interface IConnection : IDisposable
{
    bool Active { get; }
    int ProtocolVersion { get; }
    ConnectionState State  { get; }
}
