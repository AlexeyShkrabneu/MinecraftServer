namespace Server;

public class MinecraftServer : IDisposable
{
    private readonly ILogger _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConnectionHandler _connectionHandler;

    private readonly Thread _listenerThread;
    private readonly TcpListener _serverListener;
    private readonly ConfigureServerOptions _serverOptions;
    private readonly CancellationTokenSource _cancellationTokenSource;
    public bool IsRunning { get; private set; }

    internal MinecraftServer(ConfigureServerOptions serverOptions, IServiceProvider serviceProvider)
    {
        _serverOptions = serverOptions;
        _serviceProvider = serviceProvider;

        _cancellationTokenSource = new();

        _logger = serviceProvider.GetRequiredService<ILogger>();
        _connectionHandler = serviceProvider.GetRequiredService<IConnectionHandler>();

        _serverListener = new TcpListener(_serverOptions.Ip, _serverOptions.Port);
        _listenerThread = new Thread(async () => await StartTcpListenerAsync(_cancellationTokenSource.Token));
    }

    public static MinecraftServerBuilder CreateBuilder()
    {
        return new MinecraftServerBuilder();
    }

    public void Start()
    {
        try
        {
            _logger.Information("Trying to start the server");

            if (IsRunning)
            {
                _logger.Information("Server is already running!");
                return;
            }

            _serverListener.Start();
            _listenerThread.Start();
            IsRunning = true;


            if (_serverOptions.DisplayInLocalNetwork)
            {
                _ = Task.Run(async () => await ViewInLocalNetworkAsync(_cancellationTokenSource.Token));
            }

            _logger.Information("Server has been started!");
        }
        catch(Exception ex)
        {
            _logger.Fatal(ex, "Exception while starting the server:");
            Stop();
        }
    }

    public void Stop()
    {
        try
        {
            _logger.Information("Trying to stop the server");

            if (!IsRunning)
            {
                _logger.Information("Server is not running!");
            }

            _serverListener.Stop();
            _cancellationTokenSource.Cancel();

            _logger.Information("The server is stoped!");
        }
        catch(Exception ex)
        {
            _logger.Fatal(ex, "Exception while stopping a server");
        }
        finally
        {
            Dispose();
            IsRunning = _listenerThread.IsAlive;
        }
    }

    public void Dispose()
    {
        _serverListener.Dispose();
        _cancellationTokenSource.Dispose();
    }

    private async Task StartTcpListenerAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.Information("Waiting for client connections...");
            do
            {
                var tcpClient = await _serverListener.AcceptTcpClientAsync(cancellationToken);
                var connection = await Connection.HandshakeAsync(tcpClient);

                if (connection is null)
                {
                    continue;
                }

                _ = Task.Run(async () => await _connectionHandler.HandleAsync(connection, cancellationToken));
            } while (IsRunning);
        }
        catch (OperationCanceledException) { /* Ignored */ }
    }

    private async Task ViewInLocalNetworkAsync(CancellationToken cancellationToken = default)
    {
        const int brodcastPort = 4445;
        const string brodcastAddress = "224.0.2.60";
        var brodcastIPEndPoint = new IPEndPoint(IPAddress.Parse(brodcastAddress), brodcastPort);

        using var udpSender = new UdpClient();
        var package = $"[MOTD]§a{_serverOptions.Name}[/MOTD][AD]{_serverOptions.Port}[/AD]";
        var packageInBytes = Encoding.UTF8.GetBytes(package);

        do
        {
            await udpSender.SendAsync(packageInBytes, brodcastIPEndPoint);
            await Task.Delay(1000);
        } while (IsRunning);

        udpSender.Close();
        udpSender.Dispose();
    }
}
