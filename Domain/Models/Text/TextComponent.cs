namespace Domain.Models.Text;

public class TextComponent
{
    [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
    public TextComponentType Type { get; set; } = TextComponentType.Text;

    [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
    public string Text { get; set; }

    [JsonProperty("bold", NullValueHandling = NullValueHandling.Ignore)]
    public bool? Bold { get; set; }

    [JsonProperty("italic", NullValueHandling = NullValueHandling.Ignore)]
    public bool? Italic { get; set; }

    [JsonProperty("underlined", NullValueHandling = NullValueHandling.Ignore)]
    public bool? underlined { get; set; }

    [JsonProperty("strikethrough", NullValueHandling = NullValueHandling.Ignore)]
    public bool? strikethrough { get; set; }

    [JsonProperty("obfuscated", NullValueHandling = NullValueHandling.Ignore)]
    public bool? Obfuscated { get; set; }

    [JsonProperty("font", NullValueHandling = NullValueHandling.Ignore)]
    public FontType? Font { get; set; }

    [JsonProperty("insertion", NullValueHandling = NullValueHandling.Ignore)]
    public string Insertion { get; set; }

    [JsonProperty("clickEvent", NullValueHandling = NullValueHandling.Ignore)]
    public ClickEvent ClickEvent { get; set; }

    [JsonProperty("hoverEvent", NullValueHandling = NullValueHandling.Ignore)]
    public HoverEvent HoverEvent { get; set; }

    [JsonProperty("extra", NullValueHandling = NullValueHandling.Ignore)]
    public TextComponent[] Exta { get; set; }

    [JsonProperty("with", NullValueHandling = NullValueHandling.Ignore)]
    public TextComponent[] With { get; set; }

    [JsonProperty("color", NullValueHandling = NullValueHandling.Ignore)]
    public string Color { get; set; } = "white";
}
