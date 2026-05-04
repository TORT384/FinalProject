using FinalProject.Models;

namespace FinalProject.Strategies;

public class MediumGenerationStrategy : BaseGenerationStrategy
{
    private const int MediumCellsToRemove = 46;

    public MediumGenerationStrategy() : base(MediumCellsToRemove)
    {
    }

    public override GameDifficulty Difficulty => GameDifficulty.Medium;
}
