// In folder: Models/StartupItem.cs
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace MyOptimizationTool.Shared.Models
{
    public enum StartupItemLocation { Registry_CurrentUser, Registry_LocalMachine, Folder_CurrentUser, Folder_LocalMachine }

    public partial class StartupItem : ObservableObject
    {
        public string Name { get; set; } = string.Empty;
        public string Publisher { get; set; } = "Không rõ";
        public string FilePath { get; set; } = string.Empty;
        public string LocationDisplay { get; set; } = string.Empty;
        public StartupItemLocation LocationType { get; set; }

        // Dùng để lưu lại giá trị gốc (vd: "AppName") khi bị đổi tên
        public string OriginalName { get; set; } = string.Empty;

        [ObservableProperty]
        private bool isEnabled;
    }
}