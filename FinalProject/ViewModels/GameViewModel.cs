using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Threading;
using FinalProject.Commands;
using FinalProject.Factories;
using FinalProject.Models;
using FinalProject.Services;
using FinalProject.Services.Interfaces;
using FinalProject.Strategies;

namespace FinalProject.ViewModels;

public class GameViewModel : BaseViewModel
{
    private readonly SudokuFactory _sudokuFactory;
    private readonly IValidationService _validationService;
    private readonly ISaveLoadService _saveLoadService;
    private readonly DispatcherTimer _gameTimer;
    private GameSession? _currentSession;
    private DateTime _lastResumeUtc;
    private GameDifficulty _selectedDifficulty;
    private string _elapsedTimeDisplay = "00:00";
    private string _pauseButtonText = "Пауза";
    private string _statusMessage = "Натисніть \"Нова гра\", щоб згенерувати дошку.";
    private bool _isBoardInteractionEnabled;
    private bool _isGameCompleted;

    public GameViewModel() : this(CreateDefaultFactory(), new ValidationService(), CreateDefaultSaveLoadService())
    {
    }

    public GameViewModel(
        SudokuFactory sudokuFactory,
        IValidationService validationService,
        ISaveLoadService saveLoadService)
    {
        _sudokuFactory = sudokuFactory ?? throw new ArgumentNullException(nameof(sudokuFactory));
        _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
        _saveLoadService = saveLoadService ?? throw new ArgumentNullException(nameof(saveLoadService));

        Difficulties = new ObservableCollection<GameDifficulty>(Enum.GetValues<GameDifficulty>());
        _selectedDifficulty = GameDifficulty.Easy;
        Cells = new ObservableCollection<GameCellViewModel>();

        NewGameCommand = new RelayCommand(_ => StartNewGame());
        PauseResumeCommand = new RelayCommand(_ => TogglePauseResume(), _ => _currentSession is not null && !_isGameCompleted);
        SaveGameCommand = new RelayCommand(_ => _ = SaveGameAsync(), _ => _currentSession is not null);
        LoadGameCommand = new RelayCommand(_ => _ = LoadGameAsync());

        _gameTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _gameTimer.Tick += OnGameTimerTick;
    }

    public string Title => "Sudoku 9x9";

    public ObservableCollection<GameDifficulty> Difficulties { get; }

    public ObservableCollection<GameCellViewModel> Cells { get; }

    public ICommand NewGameCommand { get; }

    public ICommand PauseResumeCommand { get; }

    public ICommand SaveGameCommand { get; }

    public ICommand LoadGameCommand { get; }

    public GameDifficulty SelectedDifficulty
    {
        get => _selectedDifficulty;
        set => SetProperty(ref _selectedDifficulty, value);
    }

    public string ElapsedTimeDisplay
    {
        get => _elapsedTimeDisplay;
        private set => SetProperty(ref _elapsedTimeDisplay, value);
    }

    public string PauseButtonText
    {
        get => _pauseButtonText;
        private set => SetProperty(ref _pauseButtonText, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        private set => SetProperty(ref _statusMessage, value);
    }

    public bool IsBoardInteractionEnabled
    {
        get => _isBoardInteractionEnabled;
        private set => SetProperty(ref _isBoardInteractionEnabled, value);
    }

    private void StartNewGame()
    {
        _gameTimer.Stop();

        _currentSession = _sudokuFactory.CreateNewSession(SelectedDifficulty);
        _currentSession.Resume();
        _lastResumeUtc = DateTime.UtcNow;
        _isGameCompleted = false;

        BuildCellsFromBoard(_currentSession.Board);
        UpdateElapsedTimeDisplay(TimeSpan.Zero);
        PauseButtonText = "Пауза";
        IsBoardInteractionEnabled = true;
        _gameTimer.Start();

        StatusMessage = $"Нова гра ({SelectedDifficulty}).";
        CommandManager.InvalidateRequerySuggested();
    }

    private async Task SaveGameAsync()
    {
        if (_currentSession is null)
        {
            StatusMessage = "Немає активної гри для збереження.";
            return;
        }

        try
        {
            if (!_currentSession.IsPaused && !_isGameCompleted)
            {
                PersistElapsedUntilNow(_currentSession);
                _lastResumeUtc = DateTime.UtcNow;
            }

            await _saveLoadService.SaveSessionAsync(_currentSession);
            StatusMessage = "Гру успішно збережено.";
        }
        catch
        {
            StatusMessage = "Помилка збереження гри.";
        }
    }

    private async Task LoadGameAsync()
    {
        try
        {
            var loadedSession = await _saveLoadService.LoadSessionAsync();
            if (loadedSession is null)
            {
                StatusMessage = "Збереженої гри не знайдено.";
                return;
            }

            ApplyLoadedSession(loadedSession);
            StatusMessage = "Збережену гру завантажено.";
        }
        catch
        {
            StatusMessage = "Помилка завантаження гри.";
        }
    }

    private void BuildCellsFromBoard(SudokuBoard board)
    {
        Cells.Clear();
        foreach (var cell in board.Cells)
        {
            Cells.Add(new GameCellViewModel(cell, OnCellInputChanged));
        }
    }

    private void OnCellInputChanged(GameCellViewModel cellViewModel, string? inputValue)
    {
        if (_currentSession is null || _currentSession.IsPaused || _isGameCompleted || cellViewModel.IsFixed)
        {
            return;
        }

        var cell = cellViewModel.Cell;
        var previousValue = cell.Value;

        if (string.IsNullOrWhiteSpace(inputValue))
        {
            cell.SetValue(0);
            cellViewModel.IsConflict = false;
            StatusMessage = "Значення очищено.";
            return;
        }

        if (!TryParseSingleDigit(inputValue, out var parsedValue))
        {
            RevertToPreviousValue(cellViewModel, previousValue);
            StatusMessage = "Вводьте лише одну цифру від 1 до 9.";
            return;
        }

        cell.SetValue(parsedValue);
        if (!_validationService.IsMoveValid(_currentSession.Board, cell.Row, cell.Column, parsedValue))
        {
            cell.SetValue(previousValue);
            cellViewModel.IsConflict = true;
            RevertToPreviousValue(cellViewModel, previousValue);
            StatusMessage = "Некоректний хід: цифра конфліктує з рядком, стовпцем або блоком.";
            return;
        }

        cellViewModel.IsConflict = false;
        cellViewModel.RefreshFromModel();
        StatusMessage = "Хід прийнято.";

        if (_validationService.IsBoardComplete(_currentSession.Board))
        {
            _isGameCompleted = true;
            _gameTimer.Stop();
            FinalizeElapsedOnCompletion(_currentSession);
            IsBoardInteractionEnabled = false;
            StatusMessage = "Вітаю! Sudoku розв'язано.";
            CommandManager.InvalidateRequerySuggested();
        }
    }

    private void TogglePauseResume()
    {
        if (_currentSession is null || _isGameCompleted)
        {
            return;
        }

        if (_currentSession.IsPaused)
        {
            _currentSession.Resume();
            _lastResumeUtc = DateTime.UtcNow;
            _gameTimer.Start();
            PauseButtonText = "Пауза";
            IsBoardInteractionEnabled = true;
            StatusMessage = "Гру продовжено.";
        }
        else
        {
            PersistElapsedUntilNow(_currentSession);
            _currentSession.Pause();
            _gameTimer.Stop();
            PauseButtonText = "Продовжити";
            IsBoardInteractionEnabled = false;
            StatusMessage = "Гру поставлено на паузу.";
        }
    }

    private void ApplyLoadedSession(GameSession loadedSession)
    {
        _gameTimer.Stop();

        _currentSession = loadedSession;
        SelectedDifficulty = loadedSession.Difficulty;
        BuildCellsFromBoard(loadedSession.Board);
        UpdateElapsedTimeDisplay(loadedSession.Elapsed);

        _isGameCompleted = _validationService.IsBoardComplete(loadedSession.Board);
        if (_isGameCompleted)
        {
            loadedSession.Pause();
            PauseButtonText = "Завершено";
            IsBoardInteractionEnabled = false;
            CommandManager.InvalidateRequerySuggested();
            return;
        }

        if (loadedSession.IsPaused)
        {
            PauseButtonText = "Продовжити";
            IsBoardInteractionEnabled = false;
        }
        else
        {
            PauseButtonText = "Пауза";
            IsBoardInteractionEnabled = true;
            _lastResumeUtc = DateTime.UtcNow;
            _gameTimer.Start();
        }

        CommandManager.InvalidateRequerySuggested();
    }

    private void OnGameTimerTick(object? sender, EventArgs e)
    {
        if (_currentSession is null || _currentSession.IsPaused || _isGameCompleted)
        {
            return;
        }

        UpdateElapsedTimeDisplay(GetCurrentElapsed(_currentSession));
    }

    private void PersistElapsedUntilNow(GameSession session)
    {
        var elapsed = GetCurrentElapsed(session);
        session.UpdateElapsed(elapsed);
        UpdateElapsedTimeDisplay(elapsed);
    }

    private void FinalizeElapsedOnCompletion(GameSession session)
    {
        PersistElapsedUntilNow(session);
        session.Pause();
        PauseButtonText = "Завершено";
    }

    private TimeSpan GetCurrentElapsed(GameSession session)
    {
        if (session.IsPaused)
        {
            return session.Elapsed;
        }

        return session.Elapsed + (DateTime.UtcNow - _lastResumeUtc);
    }

    private void UpdateElapsedTimeDisplay(TimeSpan elapsed)
    {
        ElapsedTimeDisplay = $"{(int)elapsed.TotalMinutes:00}:{elapsed.Seconds:00}";
    }

    private static bool TryParseSingleDigit(string value, out int parsedValue)
    {
        parsedValue = 0;
        return value.Length == 1 && int.TryParse(value, out parsedValue) && parsedValue is >= 1 and <= 9;
    }

    private static void RevertToPreviousValue(GameCellViewModel cellViewModel, int previousValue)
    {
        var uiValue = previousValue == 0 ? string.Empty : previousValue.ToString();
        cellViewModel.SetInputSilently(uiValue);
    }

    private static SudokuFactory CreateDefaultFactory()
    {
        var strategies = new ISudokuGenerationStrategy[]
        {
            new EasyGenerationStrategy(),
            new MediumGenerationStrategy(),
            new HardGenerationStrategy()
        };

        var generator = new SudokuGenerator(strategies);
        return new SudokuFactory(generator);
    }

    private static ISaveLoadService CreateDefaultSaveLoadService()
    {
        return new SaveLoadService();
    }
}
