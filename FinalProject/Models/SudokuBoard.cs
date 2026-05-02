using System.Collections.ObjectModel;

namespace FinalProject.Models;

public class SudokuBoard
{
    public const int Size = 9;

    public SudokuBoard(IEnumerable<Cell> cells)
    {
        var preparedCells = cells.ToList();
        if (preparedCells.Count != Size * Size)
        {
            throw new ArgumentException("Sudoku board must contain exactly 81 cells.", nameof(cells));
        }

        Cells = new ReadOnlyCollection<Cell>(preparedCells);
    }

    public ReadOnlyCollection<Cell> Cells { get; }

    public Cell GetCell(int row, int column)
    {
        ValidateCoordinates(row, column);
        return Cells[(row * Size) + column];
    }

    private static void ValidateCoordinates(int row, int column)
    {
        if (row is < 0 or >= Size)
        {
            throw new ArgumentOutOfRangeException(nameof(row), "Row must be between 0 and 8.");
        }

        if (column is < 0 or >= Size)
        {
            throw new ArgumentOutOfRangeException(nameof(column), "Column must be between 0 and 8.");
        }
    }
}
