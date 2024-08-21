namespace Infrastructure.Services;

public class MojangAuthService(
    ServerEncryption serverEncryption)
        : IMojangAuthService
{
    private const string _mojangSessionApi = "https://sessionserver.mojang.com/";

    public async Task<IPlayerProfile> GetPlayerProfileAsync(
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

    public async Task<IPlayerProfile> GetAuthenticatedPlayerProfileAsync(
        string username, byte[] sharedSecret, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(username) || sharedSecret.Length != 16)
        {
            return null;
        }

        var serverHash = GenerateServerHash(sharedSecret);
        var usernameUrlEncoded = HttpUtility.UrlEncode(username, Encoding.UTF8);
        var hasJoinedRoute = $"/session/minecraft/hasJoined?username={usernameUrlEncoded}&serverId={serverHash}";
        using var httpClient = new HttpClient { BaseAddress = new Uri(_mojangSessionApi) };

        var playerJoinedResponse = await httpClient.GetAsync(hasJoinedRoute, cancellationToken);
        var playerJoinedJson = await playerJoinedResponse.Content.ReadAsStringAsync(cancellationToken);

        if(playerJoinedResponse.StatusCode is not HttpStatusCode.OK ||
           string.IsNullOrWhiteSpace(playerJoinedJson))
        {
            return null;
        }

        var mojangProfile = JsonConvert.DeserializeObject<MojangPlayerProfileResponse>(playerJoinedJson);
        return new PlayerProfile(new Guid(mojangProfile.Id), mojangProfile.Name, mojangProfile.Properties, true);
    }

    private string GenerateServerHash(byte[] sharedSecretBytes)
    {
        var serverId = Encoding.Latin1.GetBytes(ProtocolDefinition.ServerId);
        var publicKey = serverEncryption.PublicKeyDERFormat;

        using var sha = SHA1.Create();

        sha.TransformBlock(serverId, 0, serverId.Length, serverId, 0);
        sha.TransformBlock(sharedSecretBytes, 0, sharedSecretBytes.Length, sharedSecretBytes, 0);
        sha.TransformFinalBlock(publicKey, 0, publicKey.Length);

        var hashNumber = new BigInteger(sha.Hash, false, true);
        
        return hashNumber < 0
            ? "-" + (-hashNumber).ToString("x").TrimStart('0')
            : hashNumber.ToString("x").TrimStart('0');
    }
}
