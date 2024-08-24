namespace Domain.Constants;

public static class ServerConstants
{
    public const int ProtocolVersion = 767;
    public const string VersionName = "1.21";
    public const ushort DefaultPort = 25565;
    public const int DefaultMaxPlayersCount = 10;
    public const int DefaultMaxPlayerUserNameLength = 16;
    public const string DefaultHots = "localhost";
    public const string DefaultName = "Minecraft Server";
    public const bool DefaultEnforcesSecureChat = false;
    public const bool DefaultOnlineMode = false;
    public const bool DefautlUseEncryption = true;
    public const bool DefaultDisplayInLocalNetwork = true;
    public const bool DefaultIsHardcore = false;
    public const int DefaultViewDistance = 8;
    public const int DefaultSimulationDistance = 8;
    public const bool DefaultReducedDebugInfo = true;
    public const bool DefaultEnableRespawnScreen = true;
    public static readonly TextComponent DefaultDescription = new()
    {
        Type = TextComponentType.Text,
        Text = "Locale Minecraft Server",
        Color = "#7eff47"
    };
}
