namespace Infrastructure.Network.Package.ServerBound.Login;

public class LoginStartServerBoundPackage(
    ServerOptions serverOptions,
    ServerEncryption serverEncryption,
    IPlayerManager playerManager,
    IMojangAuthService mojangAuthService)
        : ServerBoundPackage(ProtocolDefinition.LoginStart)
{
    public async override Task<ClientBoundPackage> HandleAsync(IConnection connection, CancellationToken cancellationToken = default)
    {
        connection.SetState(ConnectionState.Login);

        var playerName = await connection.Stream.ReadStringAsync(cancellationToken);
        var playerId = await connection.Stream.ReadGuidAsync(cancellationToken);

        if (playerName.Length > serverOptions.MaxPlayerUserNameLength)
        {
            return new LoginDisconnectCleintBoundPackage(
                DefaultTextComponents.UsernameInvalidLength(playerName, serverOptions.MaxPlayerUserNameLength));
        }

        if(connection.ProtocolVersion > serverOptions.ProtocolVersion)
        {
            return new LoginDisconnectCleintBoundPackage(
                DefaultTextComponents.ServerVersionIsOutdated(serverOptions.VersionName));
        }

        if(connection.ProtocolVersion < serverOptions.ProtocolVersion)
        {
            return new LoginDisconnectCleintBoundPackage(
                DefaultTextComponents.ServerVersionIsModern(serverOptions.VersionName));
        }

        if (playerManager.OnlinePlayers.Length == serverOptions.MaxPlayersCount)
        {
            return new LoginDisconnectCleintBoundPackage(
                DefaultTextComponents.ServerMaxPlayersOnline(serverOptions.MaxPlayersCount));
        }

        if (playerManager.IsPlayerOnline(playerName))
        {
            return new LoginDisconnectCleintBoundPackage(
                DefaultTextComponents.PlayerIsAlreadyOnline(playerName));
        }

        var playerProfile = await mojangAuthService.GetMojangPlayerProfileAsync(playerName, playerId, cancellationToken);

        if (playerProfile is null || !playerProfile.ExistsInMojang && serverOptions.OnlineMode) 
        {
            return new LoginDisconnectCleintBoundPackage(
                DefaultTextComponents.ServerOplineModeUnathorizadPlayer());
        }

        connection.SetPlayerProfile(playerProfile);

        if (serverOptions.OnlineMode || serverOptions.UseEncryption)
        {
            var verificationToken = new byte[4];
            serverOptions.Random.NextBytes(verificationToken);

            connection.SetVerifyToken(verificationToken);

            return new EncryptionRequestClientBoundPackage(
                serverEncryption.PublicKeyDERFormat, verificationToken, serverOptions.OnlineMode);
        }

        return new LoginSuccessClientBoundPackage();
    }
}
