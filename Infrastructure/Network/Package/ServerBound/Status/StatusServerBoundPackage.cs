namespace Infrastructure.Network.Package.ServerBound.Status;

public class StatusServerBoundPackage(
    int packageId,
    ServerOptions serverOptions,
    IPlayerManager playerManager) 
        : ServerBoundPackage(packageId)
{
    public override Task<ClientBoundPackage> HandleAsync(IConnection connection, CancellationToken cancellationToken = default)
    {
        var activePlayers = playerManager.OnlinePlayers;
        var serverStatus = new ServerStatus
        {
            Version = new()
            {
                Name = serverOptions.Name,
                Protocol = serverOptions.ProtocolVersion
            },
            Palyers = new()
            {
                Players = activePlayers,
                OnlinePlayers = activePlayers.Length,
                MaxPlayers = serverOptions.MaxPlayersCount
            },
            IconBase64 = serverOptions.IconBase64,
            Description = serverOptions.Description,
            EnforcesSecureChat = serverOptions.EnforcesSecureChat,
        };

        return Task.FromResult<ClientBoundPackage>(new StatusClientBoundPackage(Id, serverStatus));
    }
}
