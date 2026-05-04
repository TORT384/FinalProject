using FinalProject.Models;

namespace FinalProject.Tests.TestData;

internal static class SudokuBoardBuilder
{
    public static SudokuBoard FromRows(int[][] rows)
    {
        if (rows.Length != 9 || rows.Any(row => row.Length != 9))
        {
            throw new ArgumentException("Board must contain 9 rows with 9 values each.", nameof(rows));
        }

        var cells = new List<Cell>(81);
        for (var row = 0; row < 9; row++)
        {
            for (var column = 0; column < 9; column++)
            {
                var value = rows[row][column];
                cells.Add(new Cell(row, column, value, isFixed: value != 0));
            }
        }

        return new SudokuBoard(cells);
    }
}
