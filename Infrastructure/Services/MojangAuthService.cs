namespace Infrastructure.Services;

public class MojangAuthService(
    ServerEncryption serverEncryption)
        : IMojangAuthService
{
    private const string _mojangSessionApi = "https://sessionserver.mojang.com/";

    public async Task<IPlayerProfile> GetMojangPlayerProfileAsync(
        string username, Guid playerId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(username))
        {
            return null;
        }

        var playerUUID = playerId.ToString().Replace("-", "");
        var profileInfoRoute = $"/session/minecraft/profile/{playerUUID}?unsigned=false";
        using var httpClient = new HttpClient { BaseAddress = new Uri(_mojangSessionApi) };
        
        var playerInfoResponse = await httpClient.GetAsync(profileInfoRoute, cancellationToken);
        var playerInfoJson = await playerInfoResponse.Content.ReadAsStringAsync(cancellationToken);
        var playerInfo = JsonConvert.DeserializeObject<MojangPlayerProfileResponse>(playerInfoJson);

        if (playerInfoResponse.StatusCode is HttpStatusCode.OK && !string.IsNullOrWhiteSpace(playerInfoJson))
        {
            return new PlayerProfile(playerId, playerInfo.Name, playerInfo.Properties, true);
        }

        return new PlayerProfile(Guid.NewGuid(), username, [], false);
    }

    public async Task<bool> IsAuthenticatedAsync(string username, byte[] sharedSecret, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(username) || sharedSecret.Length != 128)
        {
            return false;
        }

        var serverHash = GenerateServerHash(sharedSecret);
        var usernameUrlEncoded = HttpUtility.UrlEncode(username, Encoding.UTF8);
        var hasJoinedRoute = $"/session/minecraft/hasJoined?username={usernameUrlEncoded}&serverId={serverHash}";
        using var httpClient = new HttpClient { BaseAddress = new Uri(_mojangSessionApi) };

        var playerJoinedResponse = await httpClient.GetAsync(hasJoinedRoute, cancellationToken);
        var playerJoinedJson = await playerJoinedResponse.Content.ReadAsStringAsync(cancellationToken);

        return playerJoinedResponse.StatusCode is HttpStatusCode.OK 
            && !string.IsNullOrWhiteSpace(playerJoinedJson);
    }

    private string GenerateServerHash(byte[] sharedSecretBytes)
    {
        using var sha1 = SHA1.Create();

        var serverIdBytes = Encoding.Latin1.GetBytes(ProtocolDefinition.ServerId);
        var serverPublicKeyBytes = serverEncryption.PublicKey;

        using var ms = new MemoryStream();

        ms.Write(serverIdBytes, 0, serverIdBytes.Length);
        ms.Write(sharedSecretBytes, 0, sharedSecretBytes.Length);
        ms.Write(serverPublicKeyBytes, 0, serverPublicKeyBytes.Length);

        var hashBytes = sha1.ComputeHash(ms);

        Array.Reverse(hashBytes);
        var b = new BigInteger(hashBytes);

        return b < 0
            ? "-" + (-b).ToString("x").TrimStart('0')
            : b.ToString("x").TrimStart('0');
    }
}
