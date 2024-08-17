namespace Domain.Options;

public class ServerOptions
{
    #region Default values
    protected const ushort DefaultPort = 25565;
    protected const int DefaultMaxPlayersCount = 10;
    protected const int DefaultProtocolVersion = 767;
    protected const string DefaultHots = "localhost";
    protected const string DefaultName = "Minecraft Server";
    protected const string DefaultVersionName = "1.21";
    protected const bool DefaultEnforcesSecureChat = false;
    protected const bool DefaultDisplayInLocalNetwork = true;
    protected static readonly TextComponent DefaultDescription = new()
    {
        Type = TextComponentType.Text,
        Text = "Locale Minecraft Server",
        Color = "#7eff47"
    };

    #endregion

    #region Game options
    public string Host { get; set; } = DefaultHots;
    public string Name { get; set; } = DefaultName;
    public string IconBase64 { get; protected set; } = null;
    public TextComponent Description { get; set; } = DefaultDescription;
    public string VersionName { get; set; } = DefaultVersionName;
    public int ProtocolVersion { get; set; } = DefaultProtocolVersion;
    public int MaxPlayersCount { get; set; } = DefaultMaxPlayersCount;
    public bool EnforcesSecureChat { get; set; } = DefaultEnforcesSecureChat;
    public bool DisplayInLocalNetwork { get; set; } = DefaultDisplayInLocalNetwork;
    #endregion
}
