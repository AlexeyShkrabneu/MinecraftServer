namespace Domain.Models;

public class ServerStatus
{
    [JsonProperty("version", NullValueHandling = NullValueHandling.Ignore)]
    public Version Version { get; set; }

    [JsonProperty("players", NullValueHandling = NullValueHandling.Ignore)]
    public PlayersInfo Palyers { get; set; }

    [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
    public TextComponent Description { get; set; }

    [JsonProperty("favicon", NullValueHandling = NullValueHandling.Ignore)]
    public string IconBase64 { get; set; }

    [JsonProperty("enforcesSecureChat", NullValueHandling = NullValueHandling.Ignore)]
    public bool EnforcesSecureChat { get; set; }
}

public class Version
{
    [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
    public string Name { get; set; }

    [JsonProperty("protocol", NullValueHandling = NullValueHandling.Ignore)]
    public int Protocol { get; set; }
}

public class PlayersInfo
{
    [JsonProperty("max", NullValueHandling = NullValueHandling.Ignore)]
    public int MaxPlayers { get; set; }

    [JsonProperty("online", NullValueHandling = NullValueHandling.Ignore)]
    public int OnlinePlayers { get; set; }

    [JsonProperty("sample", NullValueHandling = NullValueHandling.Ignore)]
    public PlayerSample[] Players { get; set; }
}