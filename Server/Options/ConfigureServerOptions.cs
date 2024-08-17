namespace Server.Options;

public class ConfigureServerOptions : ServerOptions
{
    #region Infrastructure Settings
    public ushort Port { get; set; } = DefaultPort;
    public IPAddress Ip { get; set; } = IPAddress.Any;
    public bool IsDevelopment { get; set; }
    public string DbConnectionString { get; set; }
    #endregion

    internal void UseIcon(string base64String) => IconBase64 = base64String;

    internal ConfigureServerOptions() { }
}
