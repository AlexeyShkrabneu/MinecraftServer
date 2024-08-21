namespace Application.Abstractions.Services;

public interface IMojangAuthService
{
    Task<IPlayerProfile> GetPlayerProfileAsync(string username, Guid playerId, CancellationToken cancellationToken = default);
    Task<IPlayerProfile> GetAuthenticatedPlayerProfileAsync(string username, byte[] sharedSecret, CancellationToken cancellationToken = default);
}
