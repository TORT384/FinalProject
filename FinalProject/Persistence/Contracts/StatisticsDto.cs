namespace FinalProject.Persistence.Contracts;

public sealed class StatisticsDto
{
    public int TotalGames { get; set; }

    public int Wins { get; set; }

    public long BestTimeSeconds { get; set; }

    public long AverageTimeSeconds { get; set; }
}
