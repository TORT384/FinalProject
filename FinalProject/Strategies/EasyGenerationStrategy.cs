using FinalProject.Models;

namespace FinalProject.Strategies;

public class EasyGenerationStrategy : BaseGenerationStrategy
{
    private const int EasyCellsToRemove = 36;

    public EasyGenerationStrategy() : base(EasyCellsToRemove)
    {
    }

    public override GameDifficulty Difficulty => GameDifficulty.Easy;
}
