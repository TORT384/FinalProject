using System.IO;
using System.Text.Json;
using FinalProject.Models;
using FinalProject.Persistence.Contracts;
using FinalProject.Services.Interfaces;

namespace FinalProject.Services;

public class StatisticsService : IStatisticsService
{
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        WriteIndented = true
    };

    private readonly string _statisticsFilePath;

    public StatisticsService(string? statisticsFilePath = null)
    {
        _statisticsFilePath = statisticsFilePath ?? Path.Combine(AppContext.BaseDirectory, "Persistence", "statistics.json");
    }

    public async Task<Statistics> GetStatisticsAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_statisticsFilePath))
        {
            return new Statistics();
        }

        await using var stream = File.OpenRead(_statisticsFilePath);
        var dto = await JsonSerializer.DeserializeAsync<StatisticsDto>(stream, _serializerOptions, cancellationToken);
        return dto is null ? new Statistics() : FromDto(dto);
    }

    public async Task RegisterResultAsync(bool isWin, TimeSpan completionTime, CancellationToken cancellationToken = default)
    {
        var statistics = await GetStatisticsAsync(cancellationToken);
        statistics.RegisterGameResult(isWin, completionTime);
        await SaveAsync(statistics, cancellationToken);
    }

    private async Task SaveAsync(Statistics statistics, CancellationToken cancellationToken)
    {
        var directoryPath = Path.GetDirectoryName(_statisticsFilePath);
        if (!string.IsNullOrWhiteSpace(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        var dto = ToDto(statistics);
        await using var stream = File.Create(_statisticsFilePath);
        await JsonSerializer.SerializeAsync(stream, dto, _serializerOptions, cancellationToken);
    }

    private static StatisticsDto ToDto(Statistics statistics)
    {
        return new StatisticsDto
        {
            TotalGames = statistics.TotalGames,
            Wins = statistics.Wins,
            BestTimeSeconds = (long)statistics.BestTime.TotalSeconds,
            AverageTimeSeconds = (long)statistics.AverageCompletionTime.TotalSeconds
        };
    }

    private static Statistics FromDto(StatisticsDto dto)
    {
        var totalGames = Math.Max(0, dto.TotalGames);
        var wins = Math.Clamp(dto.Wins, 0, totalGames);

        return Statistics.CreateSnapshot(
            totalGames: totalGames,
            wins: wins,
            bestTime: TimeSpan.FromSeconds(Math.Max(0, dto.BestTimeSeconds)),
            averageCompletionTime: TimeSpan.FromSeconds(Math.Max(0, dto.AverageTimeSeconds)));
    }
}
