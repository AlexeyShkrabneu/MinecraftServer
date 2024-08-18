namespace Infrastructure.Network.Package.Handlers;

public abstract class BasePackageHandler : IBasePackageHandler
{
    internal abstract List<ServerBoundPackage> _packages { get; }

    public virtual async Task<bool> HandlePackageAsync(
        IConnection connection, IncomingPackageHeader packageHeader, CancellationToken cancellationToken = default)
    {
        var package = _packages.FirstOrDefault(package => package.Id.Equals(packageHeader.PackageId));
        if (package is null)
        {
            return false;
        }

        var clientBoundPackage = await package.HandleAsync(connection, cancellationToken);
        if (clientBoundPackage is null)
        {
            return false;
        }

        return await clientBoundPackage.RespondAsync(connection, cancellationToken);
    }
}
