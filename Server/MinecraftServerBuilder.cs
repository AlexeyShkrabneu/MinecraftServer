namespace Server;

public class MinecraftServerBuilder
{
    private const string DefaultLogFilePath = "Logs/log_.txt";
    private const string DefaultLogOutputTemplate = "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{Message:lj}{NewLine}{Exception}";

    private readonly ConfigureServerOptions _serverOptions = new();
    private readonly IServiceCollection _services = new ServiceCollection();

    private LoggerConfiguration _loggerConfiguration = null;

    internal MinecraftServerBuilder() { }

    public MinecraftServerBuilder ConfigureServer(Action<ConfigureServerOptions> serverOptions = null)
    {
        if(serverOptions is null)
        {
            throw new ArgumentNullException(nameof(serverOptions));
        }

        serverOptions(_serverOptions);

        return this;
    }

    public MinecraftServerBuilder UseIcon(string iconFilePath = "favicon.png")
    {
        var iconFileInBytes = ReadFileContentInBytes(iconFilePath);

        ValidateServerIconFormatAndSize(iconFileInBytes);

        _serverOptions.UseIcon($"data:image/png;base64,{Convert.ToBase64String(iconFileInBytes)}");

        return this;
    }

    public MinecraftServerBuilder ConfigureLogger(Action<LoggerConfiguration> loggerOptions = null)
    {
        if (loggerOptions is null)
        {
            throw new ArgumentNullException(nameof(loggerOptions));
        }

        _loggerConfiguration = new();

        loggerOptions(_loggerConfiguration);

        return this;
    }

    public MinecraftServer Build()
    {
        if (_loggerConfiguration is null)
        {
            _loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.File(
                    path: DefaultLogFilePath,
                    restrictedToMinimumLevel: LogEventLevel.Warning,
                    outputTemplate: DefaultLogOutputTemplate,
                    rollingInterval: RollingInterval.Day,
                    flushToDiskInterval: TimeSpan.FromSeconds(20),
                    encoding: Encoding.UTF8)
                .WriteTo.Console(
                    restrictedToMinimumLevel: LogEventLevel.Information,
                    outputTemplate: DefaultLogOutputTemplate,
                    formatProvider: new DateTimeFormatInfo(),
                    levelSwitch: null,
                    standardErrorFromLevel: LogEventLevel.Error,
                    theme: AnsiConsoleTheme.Sixteen
                );
        }

        _services.AddSingleton<ILogger>(_loggerConfiguration.CreateLogger());
        _services.AddSingleton<ServerOptions>(_serverOptions);

        //_services.AddPersistence(_serverOptions.DbConnectionString, _serverOptions.IsDevelopment);
        _services.AddInfrastructure();

        return new MinecraftServer(_serverOptions, _services.BuildServiceProvider());
    }

    private byte[] ReadFileContentInBytes(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File was not found: {filePath}");
        }

        using var fs = new FileStream(filePath, FileMode.Open);
        var imageBuffer = new byte[fs.Length];

        fs.Read(imageBuffer, 0, imageBuffer.Length);

        return imageBuffer;
    }

    private void ValidateServerIconFormatAndSize(byte[] iconInBytes)
    {
        if (iconInBytes.Length < 33)
        {
            throw new ArgumentException("Server icon is too small");
        }

        var pngImageFormatHeader = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0xD, 0xA, 0x1A, 0xA };
        var iconImageHeader = iconInBytes.Take(8);

        if (!iconImageHeader.SequenceEqual(pngImageFormatHeader))
        {
            throw new ArgumentException("Server icon should be in 'PNG' format");
        }

        for(int index = 8; index < iconInBytes.Length;)
        {
            int chunkLength = BitConverter.ToInt32(iconInBytes, index);
            index += 4;

            string chunkType = System.Text.Encoding.ASCII.GetString(iconInBytes, index, 4);
            index += 4;

            if (chunkType != "IHDR")
            {
                index += chunkLength + 4;
                continue;
            }

            var widthInBytes = iconInBytes.Skip(index).Take(4).Reverse().ToArray();
            var heightInBytes = iconInBytes.Skip(index).Take(4).Reverse().ToArray();

            var width = BitConverter.ToInt32(widthInBytes);
            var height = BitConverter.ToInt32(heightInBytes);

            if (width != 64 || height != 64)
            {
                throw new ArgumentException("Server icon should be 64x64 in size");
            }
            
            return;
        }

        throw new ArgumentException("Unable to verify image size.");
    }
}
