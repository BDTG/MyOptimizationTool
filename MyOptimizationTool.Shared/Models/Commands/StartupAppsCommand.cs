namespace MyOptimizationTool.Shared.Models.Commands
{
    public class StartupAppsCommand
    {
        public StartupItem? ItemToChange { get; set; }
        public bool IsEnabled { get; set; }
    }
}