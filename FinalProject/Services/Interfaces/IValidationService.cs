using FinalProject.Models;

namespace FinalProject.Services.Interfaces;

public interface IValidationService
{
    bool IsMoveValid(SudokuBoard board, int row, int column, int value);

    bool IsBoardComplete(SudokuBoard board);
}
