namespace Application.Network.Interaction;

public interface IPlayerManager
{
    PlayerSample[] OnlinePlayers { get; }

    bool IsPlayerOnline(string userName);
}
