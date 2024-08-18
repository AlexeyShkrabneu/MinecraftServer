namespace Domain.Models.MojangAuth;

public class PlayerProperty
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("value")]
    public string Value { get; set; }

    [JsonProperty("signature")]
    public string Signature { get; set; }

    public bool IsSigned => !string.IsNullOrWhiteSpace(Signature);
}
