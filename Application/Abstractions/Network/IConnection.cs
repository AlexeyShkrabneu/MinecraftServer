namespace Application.Abstractions.Network;

public interface IConnection
{
    bool Connected { get; }

    Task<InboundPackage> ReadInboundPackageAsync(CancellationToken cancellationToken);
    Task<bool> SendPackageAsync(IOutboundPackage package, CancellationToken cancellationToken);
}
