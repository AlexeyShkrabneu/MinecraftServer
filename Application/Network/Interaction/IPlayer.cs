namespace Application.Network.Interaction;

public interface IPlayer
{
    public Guid Id { get; }
    public string Username { get; }

    public PlayerProperty[] Properties { get; }
}
