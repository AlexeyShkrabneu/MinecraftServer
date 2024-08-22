namespace Infrastructure.Network.Interaction;

public class PlayerProfile(
    Guid id,
    string userName,
    PlayerProperty[] properties,
    bool existingMojangAccount) : IPlayerProfile
{
    public Guid Id { get; } = id;
    public string Username { get; } = userName;
    public bool ExistsInMojang { get; } = existingMojangAccount;
    public PlayerProperty[] Properties { get; } = properties;
    public string IdString { get; } = id.ToString().Replace("-", "");
}
