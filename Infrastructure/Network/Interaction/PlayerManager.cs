namespace Infrastructure.Network.Interaction;

public class PlayerManager : IPlayerManager
{
    public PlayerSample[] OnlinePlayers => Array.Empty<PlayerSample>();

    public bool IsPlayerOnline(string userName)
    {
        return false;
    }
}
