namespace Application.Abstractions.Services;

public interface IMojangAuthService
{
    Task<IPlayerProfile> GetMojangPlayerProfileAsync(string username, Guid playerId, CancellationToken cancellationToken = default);
    Task<bool> IsAuthenticatedAsync(string username, byte[] sharedSecret, CancellationToken cancellationToken = default);
}
