namespace Domain.Models.Text;

public class ClickEvent
{
    [JsonProperty("action")]
    public ClickEventActionType Action { get; set; }

    [JsonProperty("value")]
    public string Value { get; set; }
}
