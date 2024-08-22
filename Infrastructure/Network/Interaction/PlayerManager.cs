namespace Infrastructure.Network.Interaction;

public class PlayerManager : IPlayerManager
{
    public IEnumerable<PlayerSample> OnlinePlayers =>
         _connections.Select(connection => new PlayerSample()
         {
            Id = connection.PlayerProfile.Id.ToString(),
            Name = connection.PlayerProfile.Username,
         });
    public int OnlinePlayersCount { get; private set; } = 0;

    private readonly List<IConnection> _connections = new();

    public bool IsPlayerOnline(string userName)
    {
        return _connections.Any(player => 
            player.PlayerProfile.Username.Equals(userName, StringComparison.InvariantCultureIgnoreCase));
    }

    public void AddPlayer(IConnection connection)
    {
        OnlinePlayersCount++;
        _connections.Add(connection);
    }

    public void RemovePlayer(string uiid)
    {
        var connection = _connections.FirstOrDefault(c => 
            c.PlayerProfile.IdString.Equals(uiid, StringComparison.InvariantCultureIgnoreCase));

        RemovePlayer(connection);
    }

    public void RemovePlayer(Guid uiid)
    {
        var connection = _connections.FirstOrDefault(c =>
            c.PlayerProfile.Id.Equals(uiid));

        RemovePlayer(connection);
    }

    public void RemovePlayer(IConnection connection)
    {
        if (_connections.Remove(connection))
        {
            OnlinePlayersCount--;
        }
    }
}
