namespace Domain.Models.MojangAuth;

public class MojangPlayerProfileInfoResponse
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("errorMessage")]
    public string ErrorMessage { get; set; }
}
