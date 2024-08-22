namespace Infrastructure.Network.Package.ServerBound.Login;

internal class LoginAcknowledgedServerBoundPackage(IPlayerManager playerManager)
    : ServerBoundPackage(ProtocolDefinition.LoginAcknowledgedPackageId)
{
    public override Task<ClientBoundPackage> HandleAsync(IConnection connection, CancellationToken cancellationToken = default)
    {
        connection.SetState(ConnectionState.Play);
        playerManager.AddPlayer(connection);

        return Task.FromResult<ClientBoundPackage>(new NoActionNeededClientBoundPackage());
    }
}
