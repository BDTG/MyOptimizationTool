// In folder: Models/SystemTweak.cs
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Win32;

namespace MyOptimizationTool.Models
{
    public enum TweakType { Registry, PowerShell }

    public class SystemTweak : INotifyPropertyChanged
    {
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TweakType Type { get; set; }

        private bool _isApplied;
        public bool IsApplied
        {
            get => _isApplied;
            set { _isApplied = value; OnPropertyChanged(); }
        }

        // --- TRẢ LẠI CÁC THUỘC TÍNH NÀY ---
        public string? RegistryPath { get; set; }
        public string? ValueName { get; set; }
        public object? EnabledValue { get; set; }
        public object? DisabledValue { get; set; }
        public RegistryValueKind ValueKind { get; set; }

        public string? PowerShellCommand { get; set; }
        public string? CheckStatusCommand { get; set; }
        public string? ExpectedCheckValue { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}