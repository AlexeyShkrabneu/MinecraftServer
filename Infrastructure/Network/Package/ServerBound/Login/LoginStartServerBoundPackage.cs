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
            await connection.DisconnectAsync(
                DefaultTextComponents.UsernameInvalidLength(playerName, serverOptions.MaxPlayerUserNameLength), 
                cancellationToken);

            return new NoActionNeededClientBoundPackage();
        }

        if(connection.ProtocolVersion > serverOptions.ProtocolVersion)
        {
            await connection.DisconnectAsync(
                DefaultTextComponents.ServerVersionIsOutdated(serverOptions.VersionName),
                cancellationToken);

            return new NoActionNeededClientBoundPackage();
        }

        if(connection.ProtocolVersion < serverOptions.ProtocolVersion)
        {
            await connection.DisconnectAsync(
                DefaultTextComponents.ServerVersionIsModern(serverOptions.VersionName),
                cancellationToken);

            return new NoActionNeededClientBoundPackage();
        }

        if (playerManager.OnlinePlayersCount == serverOptions.MaxPlayersCount)
        {
            await connection.DisconnectAsync(
                DefaultTextComponents.ServerMaxPlayersOnline(serverOptions.MaxPlayersCount),
                cancellationToken);

            return new NoActionNeededClientBoundPackage();
        }

        if (playerManager.IsPlayerOnline(playerName))
        {
            await connection.DisconnectAsync(
                DefaultTextComponents.PlayerIsAlreadyOnline(playerName),
                cancellationToken);

            return new NoActionNeededClientBoundPackage();
        }

        connection.SetPlayerProfile(new PlayerProfile(playerId, playerName, [], false));

        if (serverOptions.OnlineMode || serverOptions.UseEncryption)
        {
            var verificationToken = new byte[4];
            serverOptions.Random.NextBytes(verificationToken);

            connection.SetVerifyToken(verificationToken);

            return new EncryptionRequestClientBoundPackage(
                serverEncryption.PublicKeyDERFormat, verificationToken, serverOptions.OnlineMode);
        }

        var profile = await mojangAuthService.GetPlayerProfileAsync(playerName, playerId, cancellationToken);
        connection.SetPlayerProfile(profile);

        return new LoginSuccessClientBoundPackage();
    }
}
