namespace FinalProject.Strategies;

public interface ISudokuGenerationStrategy
{
    Models.GameDifficulty Difficulty { get; }

    int[,] CreatePuzzleFromSolution(int[,] solvedGrid, Random random);
}
