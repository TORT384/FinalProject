using FinalProject.Models;

namespace FinalProject.Tests.Models;

public class StatisticsTests
{
    [Fact]
    public void RegisterGameResult_UpdatesWinsBestTimeAndAverage()
    {
        var statistics = new Statistics();

        statistics.RegisterGameResult(isWin: true, TimeSpan.FromSeconds(120));
        statistics.RegisterGameResult(isWin: true, TimeSpan.FromSeconds(180));

        Assert.Equal(2, statistics.TotalGames);
        Assert.Equal(2, statistics.Wins);
        Assert.Equal(TimeSpan.FromSeconds(120), statistics.BestTime);
        Assert.Equal(TimeSpan.FromSeconds(150), statistics.AverageCompletionTime);
    }

    [Fact]
    public void RegisterGameResult_DoesNotChangeBestAndAverage_OnLoss()
    {
        var statistics = Statistics.CreateSnapshot(
            totalGames: 2,
            wins: 1,
            bestTime: TimeSpan.FromSeconds(130),
            averageCompletionTime: TimeSpan.FromSeconds(130));

        statistics.RegisterGameResult(isWin: false, TimeSpan.Zero);

        Assert.Equal(3, statistics.TotalGames);
        Assert.Equal(1, statistics.Wins);
        Assert.Equal(TimeSpan.FromSeconds(130), statistics.BestTime);
        Assert.Equal(TimeSpan.FromSeconds(130), statistics.AverageCompletionTime);
    }
}
