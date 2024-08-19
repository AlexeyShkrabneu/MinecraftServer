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
            var authenticated = await UserAuthenticated(connection.Player.Username, sharedSecret, cancellationToken);
            
            if (authenticated)
            {

            }

        }

        return new LoginSuccessClientBoundPackage();
    }

    private async Task<bool> UserAuthenticated(string userName, byte[] sharedKey, CancellationToken cancellationToken = default)
    {
        const string mojangSessionServerApi = "https://sessionserver.mojang.com/";
        const string playerHasJoinedInfo = "/session/minecraft/hasJoined?username={0}&serverId={1}";

        var serverHash = GenerateServerHash(ProtocolDefinition.ServerId, sharedKey);

        using var client = new HttpClient
        {
            BaseAddress = new Uri(mojangSessionServerApi)
        };
        var playerAuthenticatedResponse = await client
            .GetAsync(string.Format(playerHasJoinedInfo, userName, serverHash), cancellationToken);


        return false;
    }

    private string GenerateServerHash(string serverId, byte[] sharedSecretBytes)
    {
        using var sha1 = SHA1.Create();

        var serverIdBytes = Encoding.ASCII.GetBytes(serverId);
        var serverPublicKeyBytes = serverEncryption.PublicKeyDERFormat;

        using var ms = new MemoryStream();
        
        ms.Write(serverIdBytes, 0, serverIdBytes.Length);
        ms.Write(sharedSecretBytes, 0, sharedSecretBytes.Length);
        ms.Write(serverPublicKeyBytes, 0, serverPublicKeyBytes.Length);

        var hashBytes = sha1.ComputeHash(ms);
        string hashHexString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

        Console.WriteLine($"SHA1 Hash: {hashHexString}");

        return hashHexString;
    }
}
