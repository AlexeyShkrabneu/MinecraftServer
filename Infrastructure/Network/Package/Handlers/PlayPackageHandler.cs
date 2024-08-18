namespace Infrastructure.Network.Package.Handlers;

class PlayPackageHandler : BasePackageHandler, IPlayPackageHandler
{
    internal override List<ServerBoundPackage> _packages { get; } =
    [
    ];
}