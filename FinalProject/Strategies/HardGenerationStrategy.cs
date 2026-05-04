using FinalProject.Models;

namespace FinalProject.Strategies;

public class HardGenerationStrategy : BaseGenerationStrategy
{
    private const int HardCellsToRemove = 54;

    public HardGenerationStrategy() : base(HardCellsToRemove)
    {
    }

    public override GameDifficulty Difficulty => GameDifficulty.Hard;
}
