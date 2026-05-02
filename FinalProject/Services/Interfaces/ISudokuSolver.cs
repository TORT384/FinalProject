using FinalProject.Models;

namespace FinalProject.Services.Interfaces;

public interface ISudokuSolver
{
    bool TrySolve(SudokuBoard board);
}
