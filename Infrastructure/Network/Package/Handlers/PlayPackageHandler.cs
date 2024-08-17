namespace Infrastructure.Network.Package.Handlers;

class PlayPackageHandler : IPlayPackageHandler
{
    public Task<bool> HandlePackageAsync(IConnection connection, IncomingPackageHeader packageHeader, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}