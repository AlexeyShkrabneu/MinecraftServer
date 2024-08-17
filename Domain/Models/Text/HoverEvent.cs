namespace Domain.Models.Text;

public class HoverEvent
{
    [JsonProperty("action")]
    public HoverEventActionType Action { get; set; }

    [JsonProperty("contents")]
    public string Contents { get; set; }
}
