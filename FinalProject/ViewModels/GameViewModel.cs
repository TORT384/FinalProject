using System.Collections.ObjectModel;
using System.Windows.Input;
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
    private GameSession? _currentSession;
    private GameDifficulty _selectedDifficulty;
    private string _statusMessage = "Натисніть \"Нова гра\", щоб згенерувати дошку.";

    public GameViewModel() : this(CreateDefaultFactory(), new ValidationService())
    {
    }

    public GameViewModel(SudokuFactory sudokuFactory, IValidationService validationService)
    {
        _sudokuFactory = sudokuFactory ?? throw new ArgumentNullException(nameof(sudokuFactory));
        _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));

        Difficulties = new ObservableCollection<GameDifficulty>(Enum.GetValues<GameDifficulty>());
        _selectedDifficulty = GameDifficulty.Easy;
        Cells = new ObservableCollection<GameCellViewModel>();

        NewGameCommand = new RelayCommand(_ => StartNewGame());
    }

    public string Title => "Sudoku 9x9";

    public ObservableCollection<GameDifficulty> Difficulties { get; }

    public ObservableCollection<GameCellViewModel> Cells { get; }

    public ICommand NewGameCommand { get; }

    public GameDifficulty SelectedDifficulty
    {
        get => _selectedDifficulty;
        set => SetProperty(ref _selectedDifficulty, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        private set => SetProperty(ref _statusMessage, value);
    }

    private void StartNewGame()
    {
        _currentSession = _sudokuFactory.CreateNewSession(SelectedDifficulty);
        BuildCellsFromBoard(_currentSession.Board);
        StatusMessage = $"Нова гра ({SelectedDifficulty}).";
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
        if (_currentSession is null || cellViewModel.IsFixed)
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
            StatusMessage = "Вітаю! Sudoku розв'язано.";
        }
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
}
