namespace Application.Network.Interaction;

public interface IPlayerManager
{
    int OnlinePlayersCount { get; }
    IEnumerable<PlayerSample> OnlinePlayers { get; }

    bool IsPlayerOnline(string userName);
    void AddPlayer(IConnection connection);

    void RemovePlayer(Guid uiid);
    void RemovePlayer(string uiid);
    void RemovePlayer(IConnection connection);
}
