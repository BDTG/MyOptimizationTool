// In folder: Models/StartupItem.cs
using CommunityToolkit.Mvvm.ComponentModel;

namespace MyOptimizationTool.Models
{
    public enum StartupItemLocation { Registry, StartupFolder }

    public partial class StartupItem : ObservableObject
    {
        [ObservableProperty]
        private bool isEnabled;

        public string Name { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string LocationDescription { get; set; } = string.Empty;
        public StartupItemLocation LocationType { get; set; }
    }
}