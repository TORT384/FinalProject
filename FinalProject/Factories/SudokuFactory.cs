using FinalProject.Models;
using FinalProject.Services.Interfaces;

namespace FinalProject.Factories;

public class SudokuFactory
{
    private readonly ISudokuGenerator _sudokuGenerator;

    public SudokuFactory(ISudokuGenerator sudokuGenerator)
    {
        _sudokuGenerator = sudokuGenerator ?? throw new ArgumentNullException(nameof(sudokuGenerator));
    }

    public GameSession CreateNewSession(GameDifficulty difficulty)
    {
        var board = _sudokuGenerator.Generate(difficulty);
        return new GameSession(
            sessionId: Guid.NewGuid(),
            board: board,
            difficulty: difficulty,
            startedAtUtc: DateTime.UtcNow,
            elapsed: TimeSpan.Zero,
            isPaused: false);
    }
}
