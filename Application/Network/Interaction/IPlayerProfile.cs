namespace Application.Network.Interaction;

public interface IPlayerProfile
{
    Guid Id { get; }
    string IdString { get; }
    string Username { get; }
    bool ExistsInMojang { get; }
    PlayerProperty[] Properties { get; }
}
