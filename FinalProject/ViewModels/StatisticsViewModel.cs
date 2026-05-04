using System.Windows.Input;
using FinalProject.Commands;
using FinalProject.Services;
using FinalProject.Services.Interfaces;

namespace FinalProject.ViewModels;

public class StatisticsViewModel : BaseViewModel
{
    private readonly IStatisticsService _statisticsService;
    private int _totalGames;
    private int _wins;
    private string _winRate = "0%";
    private string _bestTime = "--:--";
    private string _averageTime = "--:--";
    private string _statusMessage = "Завантаження статистики...";

    public StatisticsViewModel() : this(new StatisticsService())
    {
    }

    public StatisticsViewModel(IStatisticsService statisticsService)
    {
        _statisticsService = statisticsService ?? throw new ArgumentNullException(nameof(statisticsService));
        RefreshCommand = new RelayCommand(_ => _ = LoadAsync());
        _ = LoadAsync();
    }

    public string Title => "Статистика гравця";

    public ICommand RefreshCommand { get; }

    public int TotalGames
    {
        get => _totalGames;
        private set => SetProperty(ref _totalGames, value);
    }

    public int Wins
    {
        get => _wins;
        private set => SetProperty(ref _wins, value);
    }

    public string WinRate
    {
        get => _winRate;
        private set => SetProperty(ref _winRate, value);
    }

    public string BestTime
    {
        get => _bestTime;
        private set => SetProperty(ref _bestTime, value);
    }

    public string AverageTime
    {
        get => _averageTime;
        private set => SetProperty(ref _averageTime, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        private set => SetProperty(ref _statusMessage, value);
    }

    private async Task LoadAsync()
    {
        try
        {
            var statistics = await _statisticsService.GetStatisticsAsync();
            TotalGames = statistics.TotalGames;
            Wins = statistics.Wins;
            WinRate = statistics.TotalGames == 0
                ? "0%"
                : $"{(double)statistics.Wins / statistics.TotalGames:P0}";
            BestTime = FormatTime(statistics.BestTime);
            AverageTime = FormatTime(statistics.AverageCompletionTime);
            StatusMessage = "Статистику оновлено.";
        }
        catch
        {
            StatusMessage = "Не вдалося завантажити статистику.";
        }
    }

    private static string FormatTime(TimeSpan value)
    {
        if (value <= TimeSpan.Zero)
        {
            return "--:--";
        }

        return $"{(int)value.TotalMinutes:00}:{value.Seconds:00}";
    }
}
