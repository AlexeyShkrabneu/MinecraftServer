namespace Domain.Models.MojangAuth;

public class MojangPlayerSessionProfileInfoResponse
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("properties")]
    public PlayerProperty[] Properties { get; set; }

    [JsonProperty("errorMessage")]
    public string ErrorMessage { get; set; }
}