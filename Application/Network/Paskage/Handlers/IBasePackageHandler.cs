namespace Application.Network.Paskage.Handlers;

public interface IBasePackageHandler
{
    Task<bool> HandlePackageAsync(IConnection connection, IncomingPackageHeader packageHeader, CancellationToken cancellationToken = default);
}
