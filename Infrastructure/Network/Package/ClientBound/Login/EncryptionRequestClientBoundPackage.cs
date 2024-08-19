namespace Infrastructure.Network.Package.ClientBound.Login;

public class EncryptionRequestClientBoundPackage(
    byte[] publicKey,
    byte[] verifyToken,
    bool shouldAuthenticate)
        : ClientBoundPackage(ProtocolDefinition.EncryptionRequestPackageId)
{
    public async override Task<bool> RespondAsync(IConnection connection, CancellationToken cancellationToken = default)
    {
        await connection.Stream
            .WriteVarInt(Id)
            .WriteString(ProtocolDefinition.ServerId)
            .WriteByteArrayWithLength(publicKey)
            .WriteByteArrayWithLength(verifyToken)
            .WriteBool(shouldAuthenticate)
            .SendAsync(cancellationToken);

        return true;
    }
}
