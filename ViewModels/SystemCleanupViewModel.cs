// In folder: ViewModels/SystemCleanupViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyOptimizationTool.Core;
using MyOptimizationTool.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyOptimizationTool.ViewModels
{
    public partial class SystemCleanupViewModel : ObservableObject
    {
        private readonly CleanupService _cleanupService = new();
        public ObservableCollection<CleanupItem> CleanupItems { get; } = new();

        [ObservableProperty] private bool isBusy;
        [ObservableProperty] private string statusText = "Sẵn sàng để quét.";
        [ObservableProperty] private double totalCleanedSizeMB;

        public SystemCleanupViewModel()
        {
            LoadCleanupItems();
        }

        private void LoadCleanupItems()
        {
            CleanupItems.Clear();

            // === MỤC CƠ BẢN ===
            CleanupItems.Add(new CleanupItem { DisplayName = "Tệp tạm thời của Windows", Paths = { @"C:\Windows\Temp" } });
            CleanupItems.Add(new CleanupItem { DisplayName = "Tệp tạm thời của Người dùng", Paths = { Path.GetTempPath() } });
            CleanupItems.Add(new CleanupItem { DisplayName = "Thư mục Prefetch", Paths = { @"C:\Windows\Prefetch" }, IsSelected = false });
            CleanupItems.Add(new CleanupItem { DisplayName = "Thư mục Tải về (Downloads)", Paths = { Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads") }, IsSelected = false });
            CleanupItems.Add(new CleanupItem { DisplayName = "Thùng rác (Recycle Bin)", Paths = { "$Recycle.Bin" }, IsSelected = false });


            // === TRÌNH DUYỆT ===
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            CleanupItems.Add(new CleanupItem
            {
                DisplayName = "Google Chrome Cache",
                Paths = { Path.Combine(localAppData, @"Google\Chrome\User Data\Default\Cache") }
            });
            CleanupItems.Add(new CleanupItem
            {
                DisplayName = "Microsoft Edge Cache",
                Paths = { Path.Combine(localAppData, @"Microsoft\Edge\User Data\Default\Cache") }
            });

            // === ỨNG DỤNG ===
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            CleanupItems.Add(new CleanupItem
            {
                DisplayName = "Discord Cache",
                Paths = { Path.Combine(appData, @"discord\Cache"), Path.Combine(appData, @"discord\Code Cache") }
            });
            // Lưu ý: Đường dẫn Steam có thể cần được cấu hình nếu người dùng cài ở vị trí khác
            var steamPath = @"C:\Program Files (x86)\Steam";
            if (Directory.Exists(steamPath))
            {
                CleanupItems.Add(new CleanupItem
                {
                    DisplayName = "Steam Download Cache",
                    Paths = { Path.Combine(steamPath, @"steamapps\downloading"), Path.Combine(steamPath, @"appcache\httpcache") }
                });
            }
        }

        [RelayCommand]
        private async Task ScanAsync()
        {
            IsBusy = true;
            TotalCleanedSizeMB = 0;
            StatusText = "Đang quét các tệp rác...";

            var scanTasks = CleanupItems.Select(async item =>
            {
                item.Status = CleanupStatus.Scanning;
                item.ScannedSizeInBytes = 0;
                long totalSize = 0;
                foreach (var path in item.Paths)
                {
                    // SỬA LỖI: Sử dụng tên đúng '_cleanupService'
                    totalSize += await _cleanupService.GetDirectorySizeAsync(path);
                }
                item.ScannedSizeInBytes = totalSize;
                item.Status = CleanupStatus.Scanned;
            });

            await Task.WhenAll(scanTasks);

            var totalScannableSize = CleanupItems.Sum(i => i.ScannedSizeInBytes) / (1024.0 * 1024.0);
            StatusText = $"Quét xong. Tìm thấy {totalScannableSize:N2} MB có thể dọn dẹp.";
            IsBusy = false;
        }

        [RelayCommand]
        private async Task CleanAsync()
        {
            var itemsToClean = CleanupItems.Where(i => i.IsSelected && i.ScannedSizeInBytes > 0).ToList();
            if (!itemsToClean.Any())
            {
                StatusText = "Vui lòng chọn các mục cần dọn dẹp.";
                return;
            }

            IsBusy = true;
            StatusText = "Đang dọn dẹp...";
            long totalBytesCleaned = 0;

            foreach (var item in itemsToClean)
            {
                item.Status = CleanupStatus.Cleaning;
                long itemBytesCleaned = 0;
                foreach (var path in item.Paths)
                {
                    // SỬA LỖI: Sử dụng tên đúng '_cleanupService'
                    itemBytesCleaned += await _cleanupService.CleanDirectoryAsync(path);
                }
                totalBytesCleaned += itemBytesCleaned;
                item.ScannedSizeInBytes = 0;
                item.Status = CleanupStatus.Cleaned;
            }

            TotalCleanedSizeMB = totalBytesCleaned / (1024.0 * 1024.0);
            StatusText = $"Hoàn tất! Đã dọn dẹp thành công {TotalCleanedSizeMB:N2} MB.";
            IsBusy = false;
        }
    }
}