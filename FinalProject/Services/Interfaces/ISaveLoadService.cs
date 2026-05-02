using FinalProject.Models;

namespace FinalProject.Services.Interfaces;

public interface ISaveLoadService
{
    Task SaveSessionAsync(GameSession session, CancellationToken cancellationToken = default);

    Task<GameSession?> LoadSessionAsync(CancellationToken cancellationToken = default);
}
