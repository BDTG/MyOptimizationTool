// In folder: Shared/Models/CleanupItem.cs
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace MyOptimizationTool.Shared.Models
{
    public enum CleanupStatus { Ready, Scanning, Scanned, Cleaning, Cleaned, Error }

    public partial class CleanupItem : ObservableObject
    {
        public string DisplayName { get; set; } = string.Empty;
        public List<string> Paths { get; set; } = new();

        [ObservableProperty]
        private bool isSelected = true;

        [ObservableProperty]
        private long scannedSizeInBytes;

        [ObservableProperty]
        private CleanupStatus status = CleanupStatus.Ready;

        // SỬA LỖI: Truy cập qua Property viết hoa "ScannedSizeInBytes"
        public string SizeDisplay => ScannedSizeInBytes > 0 ? $"{ScannedSizeInBytes / (1024.0 * 1024.0):N2} MB" : "0 MB";
        partial void OnScannedSizeInBytesChanged(long value)
        {
            // Và nó sẽ chủ động báo cho UI rằng 'SizeDisplay' cũng cần được cập nhật
            OnPropertyChanged(nameof(SizeDisplay));
        }
    }
}