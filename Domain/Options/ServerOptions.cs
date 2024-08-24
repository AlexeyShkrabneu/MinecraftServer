namespace Domain.Options;

public class ServerOptions
{
    #region Game options
    public string Host { get; set; } = ServerConstants.DefaultHots;
    public string Name { get; set; } = ServerConstants.DefaultName;
    public TextComponent Description { get; set; } = ServerConstants.DefaultDescription;
    public int MaxPlayersCount { get; set; } = ServerConstants.DefaultMaxPlayersCount;
    public bool EnforcesSecureChat { get; set; } = ServerConstants.DefaultEnforcesSecureChat;
    public bool DisplayInLocalNetwork { get; set; } = ServerConstants.DefaultDisplayInLocalNetwork;
    public int MaxPlayerUserNameLength { get; set; } = ServerConstants.DefaultMaxPlayerUserNameLength;
    public bool OnlineMode { get; set; } = ServerConstants.DefaultOnlineMode;
    public bool UseEncryption { get; set; } = ServerConstants.DefautlUseEncryption;
    public bool IsHardcore { get; set; } = ServerConstants.DefaultIsHardcore;
    public int ViewDistance { get; set; } = ServerConstants.DefaultViewDistance;
    public int SimulationDistance { get; set; } = ServerConstants.DefaultSimulationDistance;
    public bool ReducedDebugInfo { get; set; } = ServerConstants.DefaultReducedDebugInfo;
    public bool EnableRespawnScreen { get; set; } = ServerConstants.DefaultEnableRespawnScreen;
    public string IconBase64 { get; protected set; } = null;
    #endregion

    public Random Random = new();
}
