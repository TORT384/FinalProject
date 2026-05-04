namespace FinalProject.Persistence.Contracts;

public sealed class SaveCellDto
{
    public int Row { get; set; }

    public int Column { get; set; }

    public int Value { get; set; }

    public bool IsFixed { get; set; }
}
