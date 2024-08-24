namespace Infrastructure.Network.Package.ServerBound.Status;

public class StatusServerBoundPackage(
    ServerOptions serverOptions,
    IPlayerManager playerManager) 
        : ServerBoundPackage(ProtocolDefinition.StatusPackageId)
{
    public override Task<ClientBoundPackage> HandleAsync(IConnection connection, CancellationToken cancellationToken = default)
    {
        var activePlayers = playerManager.OnlinePlayers;
        var serverStatus = new ServerStatus
        {
            Version = new()
            {
                Name = serverOptions.Name,
                Protocol = ServerConstants.ProtocolVersion
            },
            Palyers = new()
            {
                Players = activePlayers,
                MaxPlayers = serverOptions.MaxPlayersCount,
                OnlinePlayers = playerManager.OnlinePlayersCount
            },
            IconBase64 = serverOptions.IconBase64,
            Description = serverOptions.Description,
            EnforcesSecureChat = serverOptions.EnforcesSecureChat,
        };

        return Task.FromResult<ClientBoundPackage>(new StatusClientBoundPackage(Id, serverStatus));
    }
}
