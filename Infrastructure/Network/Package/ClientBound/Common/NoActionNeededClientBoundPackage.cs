namespace Infrastructure.Network.Package.ClientBound.Common;

internal class NoActionNeededClientBoundPackage()
    : ClientBoundPackage(int.MinValue)
{
    public override Task<bool> RespondAsync(IConnection connection, CancellationToken cancellationToken = default) 
        => Task.FromResult(true);
}
