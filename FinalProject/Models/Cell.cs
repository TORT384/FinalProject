namespace FinalProject.Models;

public class Cell
{
    public Cell(int row, int column, int value, bool isFixed)
    {
        Row = row;
        Column = column;
        Value = value;
        IsFixed = isFixed;
    }

    public int Row { get; }

    public int Column { get; }

    public int Value { get; private set; }

    public bool IsFixed { get; }

    public bool IsEmpty => Value == 0;

    public void SetValue(int value)
    {
        if (IsFixed)
        {
            return;
        }

        if (value is < 0 or > 9)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Cell value must be between 0 and 9.");
        }

        Value = value;
    }
}
