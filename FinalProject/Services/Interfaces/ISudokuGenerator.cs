using FinalProject.Models;

namespace FinalProject.Services.Interfaces;

public interface ISudokuGenerator
{
    SudokuBoard Generate(GameDifficulty difficulty);
}
