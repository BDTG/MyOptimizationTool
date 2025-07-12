// In folder: Models/RegistryTweak.cs
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Win32;

namespace MyOptimizationTool.Models
{
    public enum TweakType
    {
        Registry,    // Tinh chỉnh bằng cách sửa Registry
        PowerShell   // Tinh chỉnh bằng cách chạy lệnh PowerShell
    }
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

        // Dành cho TweakType.Registry
        public string? RegistryPath { get; set; }
        public string? ValueName { get; set; }
        public object? EnabledValue { get; set; }
        public object? DisabledValue { get; set; }
        public RegistryValueKind ValueKind { get; set; }

        // Dành cho TweakType.PowerShell
        public string? PowerShellCommand { get; set; }
        public string? CheckStatusCommand { get; set; } // Lệnh PS để kiểm tra trạng thái
        public string? ExpectedCheckValue { get; set; } // Giá trị mong đợi khi kiểm tra

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}