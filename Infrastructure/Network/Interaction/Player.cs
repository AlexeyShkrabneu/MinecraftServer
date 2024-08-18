namespace Infrastructure.Network.Interaction;

public class Player
    (Guid id, string userName, PlayerProperty[] properties) : IPlayer
{
    public Guid Id { get; } = id;

    public string Username { get; } = userName;

    public PlayerProperty[] Properties { get; } = properties;
}
