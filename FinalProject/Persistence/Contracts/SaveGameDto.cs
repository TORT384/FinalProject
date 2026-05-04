using FinalProject.Models;

namespace FinalProject.Persistence.Contracts;

public sealed class SaveGameDto
{
    public Guid SessionId { get; set; }

    public GameDifficulty Difficulty { get; set; }

    public DateTime StartedAtUtc { get; set; }

    public TimeSpan Elapsed { get; set; }

    public bool IsPaused { get; set; }

    public List<SaveCellDto> Cells { get; set; } = [];
}
