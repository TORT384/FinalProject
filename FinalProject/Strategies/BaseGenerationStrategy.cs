namespace FinalProject.Strategies;

public abstract class BaseGenerationStrategy : ISudokuGenerationStrategy
{
    protected BaseGenerationStrategy(int cellsToRemove)
    {
        if (cellsToRemove is < 1 or > 80)
        {
            throw new ArgumentOutOfRangeException(nameof(cellsToRemove), "Cells to remove must be between 1 and 80.");
        }

        CellsToRemove = cellsToRemove;
    }

    public abstract Models.GameDifficulty Difficulty { get; }

    protected int CellsToRemove { get; }

    public int[,] CreatePuzzleFromSolution(int[,] solvedGrid, Random random)
    {
        ArgumentNullException.ThrowIfNull(solvedGrid);
        ArgumentNullException.ThrowIfNull(random);

        var puzzle = (int[,])solvedGrid.Clone();
        var removed = 0;

        while (removed < CellsToRemove)
        {
            var row = random.Next(0, 9);
            var column = random.Next(0, 9);

            if (puzzle[row, column] == 0)
            {
                continue;
            }

            puzzle[row, column] = 0;
            removed++;
        }

        return puzzle;
    }
}
