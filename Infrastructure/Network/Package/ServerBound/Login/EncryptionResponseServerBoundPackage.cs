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

        if (serverOptions.OnlineMode)
        {
            var authenticated = await mojangAuthService
                .IsAuthenticatedAsync(connection.PlayerProfile.Username, decodedSharedSecret);
            
            if (!authenticated)
            {
                return new LoginDisconnectCleintBoundPackage(
                    DefaultTextComponents.ServerOplineModeUnathorizadPlayer());
            }
        }

        return new LoginSuccessClientBoundPackage();
    }
}
