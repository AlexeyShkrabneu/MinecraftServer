namespace Application.Network.Paskage.Handlers;

public interface IPackageHandler
{
    Task<bool> HandlePackageAsync(IConnection connection, IncomingPackageHeader packageHeader, CancellationToken cancellationToken = default);
}
