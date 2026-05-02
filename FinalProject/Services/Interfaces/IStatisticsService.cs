using FinalProject.Models;

namespace FinalProject.Services.Interfaces;

public interface IStatisticsService
{
    Task<Statistics> GetStatisticsAsync(CancellationToken cancellationToken = default);

    Task RegisterResultAsync(bool isWin, TimeSpan completionTime, CancellationToken cancellationToken = default);
}
