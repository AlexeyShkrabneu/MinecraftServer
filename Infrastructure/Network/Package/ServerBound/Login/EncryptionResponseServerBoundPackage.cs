namespace Infrastructure.Network.Package.ServerBound.Login;

public class EncryptionResponseServerBoundPackage(
    ServerOptions serverOptions, 
    ServerEncryption serverEncryption)
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
            return new LoginDisconnectCleintBoundPackage(
                DefaultTextComponents.EncryptionIssuesWhileLogin("Invalid decrypted verification token."));
        }

        connection.UseEncryption(decodedSharedSecret);

        if (serverOptions.OnlineMode)
        {
            //ToDo Mojang Auth;
        }

        return new LoginSuccessClientBoundPackage();
    }
}
