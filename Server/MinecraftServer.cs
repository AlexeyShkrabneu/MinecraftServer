namespace Server;

public class MinecraftServer : IDisposable
{
    private readonly ILogger _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IClientManagerService _clientService;

    private readonly Thread _listenerThread;
    private readonly TcpListener _serverListener;
    private readonly ServerOptions _serverOptions;
    private readonly CancellationTokenSource _cancellationTokenSource;
    public bool IsRunning { get; private set; }

    internal MinecraftServer(ServerOptions serverOptions, IServiceProvider serviceProvider)
    {
        _serverOptions = serverOptions;
        _serviceProvider = serviceProvider;

        _cancellationTokenSource = new();

        _logger = serviceProvider.GetRequiredService<ILogger>();
        _clientService = serviceProvider.GetRequiredService<IClientManagerService>();

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
        _logger.Information("Waiting for client connections...");

        do
        {
            var tcpClient = await _serverListener.AcceptTcpClientAsync(cancellationToken);
            var connection = await _clientService.HandshakeAsync(tcpClient, cancellationToken);

            _logger.Information("New client has been connected");

        } while (IsRunning);
    }
}
