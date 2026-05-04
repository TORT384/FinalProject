using FinalProject.Models;
using FinalProject.Services.Interfaces;
using FinalProject.Strategies;

namespace FinalProject.Services;

public class SudokuGenerator : ISudokuGenerator
{
    private readonly Dictionary<GameDifficulty, ISudokuGenerationStrategy> _strategies;
    private readonly Random _random;

    public SudokuGenerator(IEnumerable<ISudokuGenerationStrategy> strategies, Random? random = null)
    {
        ArgumentNullException.ThrowIfNull(strategies);

        _strategies = strategies.ToDictionary(strategy => strategy.Difficulty);
        _random = random ?? new Random();
    }

    public SudokuBoard Generate(GameDifficulty difficulty)
    {
        var solvedGrid = CreateSolvedGrid();
        var strategy = GetStrategy(difficulty);
        var puzzleGrid = strategy.CreatePuzzleFromSolution(solvedGrid, _random);
        return ConvertToBoard(puzzleGrid);
    }

    private ISudokuGenerationStrategy GetStrategy(GameDifficulty difficulty)
    {
        if (_strategies.TryGetValue(difficulty, out var strategy))
        {
            return strategy;
        }

        throw new InvalidOperationException($"No generation strategy configured for difficulty '{difficulty}'.");
    }

    private int[,] CreateSolvedGrid()
    {
        var grid = new int[9, 9];
        if (!TryFillGrid(grid, 0, 0))
        {
            throw new InvalidOperationException("Unable to generate a solved Sudoku grid.");
        }

        return grid;
    }

    private bool TryFillGrid(int[,] grid, int row, int column)
    {
        if (row == 9)
        {
            return true;
        }

        var nextRow = column == 8 ? row + 1 : row;
        var nextColumn = column == 8 ? 0 : column + 1;

        foreach (var value in CreateShuffledValues())
        {
            if (!CanPlaceValue(grid, row, column, value))
            {
                continue;
            }

            grid[row, column] = value;
            if (TryFillGrid(grid, nextRow, nextColumn))
            {
                return true;
            }

            grid[row, column] = 0;
        }

        return false;
    }

    private IEnumerable<int> CreateShuffledValues()
    {
        var numbers = Enumerable.Range(1, 9).ToList();
        for (var index = numbers.Count - 1; index > 0; index--)
        {
            var swapIndex = _random.Next(0, index + 1);
            (numbers[index], numbers[swapIndex]) = (numbers[swapIndex], numbers[index]);
        }

        return numbers;
    }

    private static bool CanPlaceValue(int[,] grid, int row, int column, int value)
    {
        for (var index = 0; index < 9; index++)
        {
            if (grid[row, index] == value || grid[index, column] == value)
            {
                return false;
            }
        }

        var blockRowStart = row - (row % 3);
        var blockColumnStart = column - (column % 3);
        for (var r = blockRowStart; r < blockRowStart + 3; r++)
        {
            for (var c = blockColumnStart; c < blockColumnStart + 3; c++)
            {
                if (grid[r, c] == value)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private static SudokuBoard ConvertToBoard(int[,] puzzleGrid)
    {
        var cells = new List<Cell>(81);
        for (var row = 0; row < 9; row++)
        {
            for (var column = 0; column < 9; column++)
            {
                var value = puzzleGrid[row, column];
                cells.Add(new Cell(row, column, value, isFixed: value != 0));
            }
        }

        return new SudokuBoard(cells);
    }
}
