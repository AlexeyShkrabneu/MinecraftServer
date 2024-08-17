namespace Infrastructure.Network.Package.Handlers;

public class LoginPackageHandler : ILoginPackageHandler
{
    public Task<bool> HandlePackageAsync(IConnection connection, IncomingPackageHeader packageHeader, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
