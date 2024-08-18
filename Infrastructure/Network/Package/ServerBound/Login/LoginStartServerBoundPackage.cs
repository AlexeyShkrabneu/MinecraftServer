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
        var id = await connection.Stream.ReadUUIDAsync(cancellationToken);

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

        if (playerManager.OnlinePlayers.Length == serverOptions.MaxPlayersCount)
        {
            return new LoginDisconnectCleintBoundPackage(
                DefaultTextComponents.ServerMaxPlayersOnline(serverOptions.MaxPlayersCount));
        }

        if (playerManager.IsPlayerOnline(userName))
        {
            return new LoginDisconnectCleintBoundPackage(
                DefaultTextComponents.PlayerIsAlreadyOnline(userName));
        }

        var player = await GetPlayerProfileAsync(userName, cancellationToken);

        connection.SetPlayer(player);

        if (serverOptions.OnlineMode || serverOptions.UseEncryption)
        {
            var publicKeyParameters = serverEncryption.RSA.ExportParameters(false);
            var publicKey = serverEncryption.RSA.ExportRSAPublicKey();

            var publicKeyDer = EncodePublicKeyToAsn1Der(publicKey, publicKeyParameters);
            
            var verificationToken = new byte[4];
            serverOptions.Random.NextBytes(verificationToken);

            connection.SetVerifyToken(verificationToken);

            return new EncryptionRequestClientBoundPackage(publicKeyDer, verificationToken, serverOptions.OnlineMode);
        }

        return new LoginSuccessClientBoundPackage();


        throw new NotImplementedException();
    }

    private byte[] EncodePublicKeyToAsn1Der(byte[] publicKey, RSAParameters rsaParameters)
    {
        var writer = new AsnWriter(AsnEncodingRules.DER);
        writer.PushSequence();
            writer.PushSequence();
                writer.WriteObjectIdentifier("1.2.840.113549.1.1.1");
                writer.WriteNull();
            writer.PopSequence();    
            writer.WriteBitString(publicKey);
        writer.PopSequence();

        return writer.Encode();
    }

    private async Task<IPlayer> GetPlayerProfileAsync(string username, CancellationToken cancellationToken = default)
    {
        const string mojangApi = "https://api.mojang.com/";
        const string profileInfo = "/users/profiles/minecraft/{0}";

        const string mojangSessionServerApi = "https://sessionserver.mojang.com/";
        const string sessionProfileInfo = "/session/minecraft/profile/{0}?unsigned=false";

        var mojangApiClient = new HttpClient { BaseAddress = new Uri(mojangApi) };
        var mojangSessionServerApiClient = new HttpClient { BaseAddress = new Uri(mojangSessionServerApi) };

        try
        {
            var playerMojangProfileInfoResponse = await mojangApiClient
                .GetAsync(string.Format(profileInfo, username), cancellationToken);

            var playerProfileInfoJson = await playerMojangProfileInfoResponse.Content.ReadAsStringAsync(cancellationToken);
            var playerProfileInfo = JsonConvert.DeserializeObject<MojangPlayerProfileInfoResponse>(playerProfileInfoJson);

            if (!playerMojangProfileInfoResponse.IsSuccessStatusCode || !string.IsNullOrWhiteSpace(playerProfileInfo.ErrorMessage))
            {
                var message = playerProfileInfo.ErrorMessage ?? "Player profile was not found at Mojang.";
                throw new Exception(message);
            }
            
            var userId = new Guid(playerProfileInfo.Id);

            var playerSessionProfileInfoResponse = await mojangSessionServerApiClient
                .GetAsync(string.Format(sessionProfileInfo, playerProfileInfo.Id), cancellationToken);

            var playerSessionProfileInfoJson = await playerSessionProfileInfoResponse.Content.ReadAsStringAsync(cancellationToken);
            var playerSessionProfileInfo = JsonConvert.DeserializeObject<MojangPlayerSessionProfileInfoResponse>(playerSessionProfileInfoJson);

            if (!playerSessionProfileInfoResponse.IsSuccessStatusCode || !string.IsNullOrWhiteSpace(playerSessionProfileInfo.ErrorMessage))
            {
                var message = playerSessionProfileInfo.ErrorMessage ?? "Player profile was not found at Mojang.";
                throw new Exception(message);
            }

            return new Player(userId, playerSessionProfileInfo.Name, playerSessionProfileInfo.Properties);
        } 
        catch
        {
            return new Player(Guid.NewGuid(), username, Array.Empty<PlayerProperty>());
        }
        finally
        {
            mojangApiClient.Dispose();
            mojangSessionServerApiClient.Dispose();
        }
    }
}
