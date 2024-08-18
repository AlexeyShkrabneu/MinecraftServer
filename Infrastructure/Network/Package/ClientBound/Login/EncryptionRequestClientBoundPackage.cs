namespace Infrastructure.Network.Package.ClientBound.Login;

public class EncryptionRequestClientBoundPackage(
    byte[] publicKey,
    bool shouldAuthenticate)
        : ClientBoundPackage(ProtocolDefinition.EncryptionRequestPackageId)
{
    Random random = new();

    public async override Task<bool> RespondAsync(IConnection connection, CancellationToken cancellationToken = default)
    {
        var verifyTokenBytes = new byte[4];
        random.NextBytes(verifyTokenBytes);

        connection.SetVerifyToken(verifyTokenBytes);

        await connection.Stream
            .WriteVarInt(Id)
            .WriteString("")
            .WriteVarInt(publicKey.Length)
            .WriteBytes(publicKey)
            .WriteVarInt(verifyTokenBytes.Length)
            .WriteBytes(verifyTokenBytes)
            .WriteBool(shouldAuthenticate)
            .SendAsync(cancellationToken);

        return true;
    }
}
