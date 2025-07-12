// In folder: ViewModels/PlaybookEngineViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyOptimizationTool.Core;
using MyOptimizationTool.Models;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace MyOptimizationTool.ViewModels
{
    // SỬA LỖI Ở ĐÂY: Thêm từ khóa "partial"
    public partial class PlaybookEngineViewModel : ObservableObject
    {
        private readonly PlaybookParserService _parserService = new();

        [ObservableProperty]
        private Playbook? currentPlaybook;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string statusText = "Vui lòng chọn một thư mục Playbook để bắt đầu.";

        [RelayCommand]
        private async Task LoadPlaybookAsync()
        {
            var folderPicker = new FolderPicker
            {
                SuggestedStartLocation = PickerLocationId.Desktop,
                ViewMode = PickerViewMode.List
            };
            folderPicker.FileTypeFilter.Add("*");

            var hwnd = WindowNative.GetWindowHandle(App.MainWindow);
            InitializeWithWindow.Initialize(folderPicker, hwnd);

            var folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                IsBusy = true;
                StatusText = $"Đang phân tích playbook tại: {folder.Path}...";
                CurrentPlaybook = await _parserService.ParsePlaybookAsync(folder.Path);
                StatusText = $"Đã tải thành công Playbook '{CurrentPlaybook.Name}'. Sẵn sàng để áp dụng.";
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task ApplyPlaybookAsync()
        {
            if (CurrentPlaybook == null || CurrentPlaybook.Tasks.Count == 0) return;

            IsBusy = true;
            int taskCount = 0;
            foreach (var task in CurrentPlaybook.Tasks)
            {
                taskCount++;
                StatusText = $"Đang thực thi tác vụ {taskCount}/{CurrentPlaybook.Tasks.Count}: {task.Name}...";

                Debug.WriteLine($"Executing Task: {task.Name}, Type: {task.Type}");
                foreach (var param in task.Parameters)
                {
                    Debug.WriteLine($" -> {param.Key}: {param.Value}");
                }
                await Task.Delay(500);
            }
            StatusText = "Hoàn tất áp dụng Playbook!";
            IsBusy = false;
        }
    }
}