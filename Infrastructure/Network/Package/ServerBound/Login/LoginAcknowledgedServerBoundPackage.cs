namespace Infrastructure.Network.Package.ServerBound.Login;

internal class LoginAcknowledgedServerBoundPackage()
    : ServerBoundPackage(ProtocolDefinition.LoginAcknowledgedPackageId)
{
    public override Task<ClientBoundPackage> HandleAsync(IConnection connection, CancellationToken cancellationToken = default)
    {
        connection.ChangeState(ConnectionState.Play);

        return Task.FromResult<ClientBoundPackage>(new NoActionNeededClientBoundPackage());
    }
}
