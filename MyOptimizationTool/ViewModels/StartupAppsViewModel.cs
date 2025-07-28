// In folder: ViewModels/StartupAppsViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using MyOptimizationTool.Shared.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;

namespace MyOptimizationTool.ViewModels
{
    public partial class StartupAppsViewModel : ObservableObject
    {
        //private readonly StartupService _startupService = new();
        public ObservableCollection<StartupItem> StartupItems { get; } = new();

        [ObservableProperty]
        private bool isLoading = true;

        public StartupAppsViewModel()
        {
            //_ = LoadStartupItemsAsync();
        }
        [RelayCommand]
        private async Task RefreshAsync()
        {
            await Task.CompletedTask;
        }
        /*private async Task LoadStartupItemsAsync()
        {
            IsLoading = true;
            StartupItems.Clear();
            var items = await _startupService.GetStartupItemsAsync();
            foreach (var item in items)
            {
                item.PropertyChanged += OnItemPropertyChanged; // Lắng nghe sự thay đổi của ToggleSwitch
                StartupItems.Add(item);
            }
            IsLoading = false;
        }

        private void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(StartupItem.IsEnabled) && sender is StartupItem changedItem)
            {
                // Khi người dùng bật/tắt ToggleSwitch, gọi service để thực hiện
                _startupService.SetStartupItemStatus(changedItem, changedItem.IsEnabled);
            }
        }*/
    }
}