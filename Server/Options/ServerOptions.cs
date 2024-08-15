namespace Server.Options;

public class ServerOptions
{
    #region Default values
    private const ushort DefaultPort = 25565;
    private const int DefaultMaxPlayersCount = 10;
    private const int DefaultProtocolVersion = 767;
    private const string DefaultHots = "localhost";
    private const string DefaultName = "Minecraft Server";
    private const string DefaultDescription = "§k§3Locale §2Minecraft §aServer";
    private const string DefaultVersionName = "1.21";
    #endregion

    #region Game settings
    public string Host { get; set; } = DefaultHots;
    public string Name { get; set; } = DefaultName;
    public string Description { get; set; } = DefaultDescription;
    public string VersionName { get; set; } = DefaultVersionName;
    public int ProtocolVersion { get; set; } = DefaultProtocolVersion;
    public int MaxPlayersCount { get; set; } = DefaultMaxPlayersCount;

    internal string IconBase64 { get; set; } = null;
    #endregion

    #region Infrastructure Settings
    public bool IsDevelopment { get; set; }
    public ushort Port { get; set; } = DefaultPort;
    public string DbConnectionString { get; set; }
    public IPAddress Ip { get; set; } = IPAddress.Any;
    #endregion

    internal ServerOptions() { }
}
