// In project: MyOptimizationTool.Shared
// File: Models/GameBoostCommand.cs
namespace MyOptimizationTool.Shared.Models
{
    public class GameBoostCommand
    {
        public Game? GameToLaunch { get; set; }
        public BoostMode Mode { get; set; }
    }
}