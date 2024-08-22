namespace Infrastructure.Network.Package.ServerBound.Login;

public class EncryptionResponseServerBoundPackage(
    ServerOptions serverOptions,
    ServerEncryption serverEncryption,
    IMojangAuthService mojangAuthService)
        : ServerBoundPackage(ProtocolDefinition.EncryptionResponsePackageId)
{
    public async override Task<ClientBoundPackage> HandleAsync(IConnection connection, CancellationToken cancellationToken = default)
    {
        var sharedSecret = await connection.Stream.ReadByteArrayAsync(cancellationToken);
        var validationToken = await connection.Stream.ReadByteArrayAsync(cancellationToken);

        var decodedSharedSecret = serverEncryption.RSA.Decrypt(sharedSecret, RSAEncryptionPadding.Pkcs1);
        var decodedValidationToken = serverEncryption.RSA.Decrypt(validationToken, RSAEncryptionPadding.Pkcs1);

        if (!connection.ValidateVerifyToken(decodedValidationToken))
        {
            connection.Dispose();
            return new NoActionNeededClientBoundPackage();
        }

        connection.UseEncryption(decodedSharedSecret);

        IPlayerProfile playerProfile = serverOptions.OnlineMode
            ? await mojangAuthService.GetAuthenticatedPlayerProfileAsync(connection.PlayerProfile.Username, decodedSharedSecret, cancellationToken)
            : await mojangAuthService.GetPlayerProfileAsync(connection.PlayerProfile.Username, connection.PlayerProfile.Id, cancellationToken);

        if (serverOptions.OnlineMode && playerProfile is null)
        {
           await connection.DisconnectAsync(
                DefaultTextComponents.ServerOplineModeUnathorizadPlayer(),
                cancellationToken);

            return new NoActionNeededClientBoundPackage();
        }

        connection.SetPlayerProfile(playerProfile);

        return new LoginSuccessClientBoundPackage();
    }
}
