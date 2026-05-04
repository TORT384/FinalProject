namespace FinalProject.Models;

public class Statistics
{
    public int TotalGames { get; private set; }

    public int Wins { get; private set; }

    public TimeSpan BestTime { get; private set; } = TimeSpan.Zero;

    public TimeSpan AverageCompletionTime { get; private set; } = TimeSpan.Zero;

    public static Statistics CreateSnapshot(int totalGames, int wins, TimeSpan bestTime, TimeSpan averageCompletionTime)
    {
        if (totalGames < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(totalGames), "Total games cannot be negative.");
        }

        if (wins < 0 || wins > totalGames)
        {
            throw new ArgumentOutOfRangeException(nameof(wins), "Wins must be between 0 and total games.");
        }

        if (bestTime < TimeSpan.Zero || averageCompletionTime < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(bestTime), "Time values cannot be negative.");
        }

        return new Statistics
        {
            TotalGames = totalGames,
            Wins = wins,
            BestTime = bestTime,
            AverageCompletionTime = averageCompletionTime
        };
    }

    public void RegisterGameResult(bool isWin, TimeSpan completionTime)
    {
        if (completionTime < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(completionTime), "Completion time cannot be negative.");
        }

        var previousWins = Wins;

        TotalGames++;
        if (isWin)
        {
            Wins++;
            UpdateBestTime(completionTime);
            RecalculateAverageTime(previousWins, completionTime);
        }
    }

    private void UpdateBestTime(TimeSpan completionTime)
    {
        if (BestTime == TimeSpan.Zero || completionTime < BestTime)
        {
            BestTime = completionTime;
        }
    }

    private void RecalculateAverageTime(int previousWins, TimeSpan completionTime)
    {
        if (previousWins == 0)
        {
            AverageCompletionTime = completionTime;
            return;
        }

        var totalTicks = (AverageCompletionTime.Ticks * previousWins) + completionTime.Ticks;
        AverageCompletionTime = TimeSpan.FromTicks(totalTicks / Wins);
    }
}
