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
    public partial class PlaybookEngineViewModel : ObservableObject
    {
        private readonly PlaybookParserService _parserService;

        [ObservableProperty]
        private Playbook? currentPlaybook;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string statusText = "Vui lòng chọn một thư mục Playbook để bắt đầu.";

        // Constructor với DI inject (thay vì new() trực tiếp)
        public PlaybookEngineViewModel(PlaybookParserService parserService)
        {
            _parserService = parserService ?? throw new ArgumentNullException(nameof(parserService));
        }

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
                try
                {
                    IsBusy = true;
                    StatusText = $"Đang phân tích playbook tại: {folder.Path}...";
                    CurrentPlaybook = await _parserService.ParsePlaybookAsync(folder.Path);
                    StatusText = $"Đã tải thành công Playbook '{CurrentPlaybook.Name}'. Sẵn sàng để áp dụng.";
                }
                catch (Exception ex)
                {
                    StatusText = $"Lỗi khi tải: {ex.Message}";
                    Debug.WriteLine($"Load error: {ex}");
                    // Có thể thêm dialog ở đây nếu cần
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        [RelayCommand]
        private async Task ApplyPlaybookAsync()
        {
            if (CurrentPlaybook == null || CurrentPlaybook.Tasks.Count == 0) return;

            try
            {
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
                    await Task.Delay(500);  // Placeholder - thay bằng real execution ở đây
                }
                StatusText = "Hoàn tất áp dụng Playbook!";
            }
            catch (Exception ex)
            {
                StatusText = $"Lỗi khi apply: {ex.Message}";
                Debug.WriteLine($"Apply error: {ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
