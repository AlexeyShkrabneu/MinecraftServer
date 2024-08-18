namespace Infrastructure.Network.Package.ServerBound.Login;

public class LoginStartServerBoundPackage(
    ServerOptions serverOptions,
    IPlayerManager playerManager,
    ServerEncryption serverEncryption)
        : ServerBoundPackage(ProtocolDefinition.LoginStart)
{
    public async override Task<ClientBoundPackage> HandleAsync(IConnection connection, CancellationToken cancellationToken = default)
    {
        connection.ChangeState(ConnectionState.Login);

        var userName = await connection.Stream.ReadStringAsync(cancellationToken);
        var id = await connection.Stream.ReadStringAsync(cancellationToken);

        if (userName.Length > serverOptions.MaxPlayerUserNameLength)
        {
            return new LoginDisconnectCleintBoundPackage(
                DefaultTextComponents.UsernameInvalidLength(userName, serverOptions.MaxPlayerUserNameLength));
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

        if (playerManager.IsPlayerOnline(userName))
        {
            return new LoginDisconnectCleintBoundPackage(
                DefaultTextComponents.PlayerIsAlreadyOnline(userName));
        }

        if(serverOptions.OnlineMode || serverOptions.UseEncryption)
        {
            var publicKeyParameters = serverEncryption.RSA.ExportParameters(false);
            var publicKey = serverEncryption.RSA.ExportRSAPublicKey();

            var publicKeyDer = EncodePublicKeyToAsn1Der(publicKey, publicKeyParameters);
            var myKey = Convert.ToBase64String(publicKeyDer);

            return new EncryptionRequestClientBoundPackage(publicKeyDer, serverOptions.OnlineMode);
        }
        
        throw new NotImplementedException();
    }

    private byte[] EncodePublicKeyToAsn1Der(byte[] publicKey, RSAParameters rsaParameters)
    {
        var writer = new AsnWriter(AsnEncodingRules.DER);
        writer.PushSequence(Asn1Tag.Sequence);
            writer.PushSequence();
                writer.WriteObjectIdentifier("1.2.840.113549.1.1.1");
                writer.WriteNull();
            writer.PopSequence();    
            writer.WriteBitString(publicKey);
        writer.PopSequence();

        return writer.Encode();
    }
}
