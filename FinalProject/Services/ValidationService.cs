using FinalProject.Models;
using FinalProject.Services.Interfaces;

namespace FinalProject.Services;

public class ValidationService : IValidationService
{
    public bool IsMoveValid(SudokuBoard board, int row, int column, int value)
    {
        ArgumentNullException.ThrowIfNull(board);

        if (value == 0)
        {
            return true;
        }

        if (value is < 1 or > 9)
        {
            return false;
        }

        for (var index = 0; index < SudokuBoard.Size; index++)
        {
            if (index != column && board.GetCell(row, index).Value == value)
            {
                return false;
            }

            if (index != row && board.GetCell(index, column).Value == value)
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
                if (r == row && c == column)
                {
                    continue;
                }

                if (board.GetCell(r, c).Value == value)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public bool IsBoardComplete(SudokuBoard board)
    {
        ArgumentNullException.ThrowIfNull(board);

        foreach (var cell in board.Cells)
        {
            if (cell.Value == 0 || !IsMoveValid(board, cell.Row, cell.Column, cell.Value))
            {
                return false;
            }
        }

        return true;
    }
}
