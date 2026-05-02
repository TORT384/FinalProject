using System.Windows.Input;
using FinalProject.Commands;

namespace FinalProject.ViewModels;

public class MainMenuViewModel : BaseViewModel
{
    private BaseViewModel _currentViewModel;

    public MainMenuViewModel()
    {
        ShowGameCommand = new RelayCommand(_ => ShowGame());
        ShowStatisticsCommand = new RelayCommand(_ => ShowStatistics());
        ShowSettingsCommand = new RelayCommand(_ => ShowSettings());

        _currentViewModel = new GameViewModel();
    }

    public BaseViewModel CurrentViewModel
    {
        get => _currentViewModel;
        private set => SetProperty(ref _currentViewModel, value);
    }

    public ICommand ShowGameCommand { get; }

    public ICommand ShowStatisticsCommand { get; }

    public ICommand ShowSettingsCommand { get; }

    private void ShowGame()
    {
        CurrentViewModel = new GameViewModel();
    }

    private void ShowStatistics()
    {
        CurrentViewModel = new StatisticsViewModel();
    }

    private void ShowSettings()
    {
        CurrentViewModel = new SettingsViewModel();
    }
}
