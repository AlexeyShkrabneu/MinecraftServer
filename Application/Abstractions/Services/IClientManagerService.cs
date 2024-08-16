namespace Application.Abstractions.Services;

public interface IClientManagerService
{
    Task<IConnection> HandshakeAsync(TcpClient tcpClient, CancellationToken cancellationToken = default);
}
