namespace FinalProject.Models;

public class GameSession
{
    public GameSession(
        Guid sessionId,
        SudokuBoard board,
        GameDifficulty difficulty,
        DateTime startedAtUtc,
        TimeSpan elapsed,
        bool isPaused)
    {
        SessionId = sessionId;
        Board = board;
        Difficulty = difficulty;
        StartedAtUtc = startedAtUtc;
        Elapsed = elapsed;
        IsPaused = isPaused;
    }

    public Guid SessionId { get; }

    public SudokuBoard Board { get; }

    public GameDifficulty Difficulty { get; }

    public DateTime StartedAtUtc { get; }

    public TimeSpan Elapsed { get; private set; }

    public bool IsPaused { get; private set; }

    public void Pause()
    {
        IsPaused = true;
    }

    public void Resume()
    {
        IsPaused = false;
    }

    public void UpdateElapsed(TimeSpan elapsed)
    {
        if (elapsed < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(elapsed), "Elapsed time cannot be negative.");
        }

        Elapsed = elapsed;
    }
}
