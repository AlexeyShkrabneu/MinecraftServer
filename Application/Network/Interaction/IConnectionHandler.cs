namespace Application.Network.Interaction;

public interface IConnectionHandler
{
    Task HandleAsync(IConnection connection, CancellationToken cancellationToken = default);
}
