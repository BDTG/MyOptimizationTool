// In folder: ViewModels/GameLauncherViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyOptimizationTool.Shared.Services;
using MyOptimizationTool.Shared.Models;
using MyOptimizationTool.Services; // Thêm using cho Client mới
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using WinRT.Interop;
using Microsoft.UI.Xaml.Controls;

namespace MyOptimizationTool.ViewModels
{
    public partial class GameLauncherViewModel : ObservableObject
    {
        private readonly GameService _gameService = new();
        private readonly GameBoostServiceClient _boostClient = new();
        public ObservableCollection<Game> Games { get; } = new();

        [ObservableProperty] private bool isOptimizing;
        [ObservableProperty] private string statusText = "Sẵn sàng để khởi chạy.";
        
        public GameLauncherViewModel() { _ = LoadGamesAsync(); }
        private async Task LoadGamesAsync()
        {
            var loadedGames = await _gameService.LoadGamesAsync();
            Games.Clear();
            foreach (var game in loadedGames) { Games.Add(game); }
        }
        [RelayCommand]
        private async Task RemoveGame(Game? game)
        {
            if (game == null) return;
            var dialog = new ContentDialog
            {
                Title = "Xác nhận Xóa",
                Content = $"Bạn có chắc chắn muốn xóa '{game.Name}' khỏi danh sách không?",
                PrimaryButtonText = "Xóa",
                CloseButtonText = "Hủy",
                DefaultButton = ContentDialogButton.Close,
                // Gắn dialog với cửa sổ chính
                XamlRoot = App.MainWindow?.Content.XamlRoot
            };
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                Games.Remove(game);
                await _gameService.SaveGamesAsync(Games);
            }
        }
        [RelayCommand]
        private async Task AddGameAsync()
        {
            var filePicker = new FileOpenPicker { ViewMode = PickerViewMode.Thumbnail, SuggestedStartLocation = PickerLocationId.ComputerFolder };
            filePicker.FileTypeFilter.Add(".exe");
            var hwnd = WindowNative.GetWindowHandle(App.MainWindow);
            InitializeWithWindow.Initialize(filePicker, hwnd);
            var file = await filePicker.PickSingleFileAsync();
            if (file != null)
            {
                var newGame = await _gameService.CreateGameFromFileAsync(file.Path);

                if (newGame != null && !Games.Any(g => g.ExecutablePath.Equals(newGame.ExecutablePath, StringComparison.OrdinalIgnoreCase)))
                {
                    Games.Add(newGame);
                    await _gameService.SaveGamesAsync(Games);
                }
            }
        }
        [RelayCommand]
        private async Task LaunchGame(Game? game)
        {
            if (game == null) return;
            // Gửi yêu cầu boost đi và không cần chờ đợi
            await _boostClient.RequestBoostAndLaunch(game, BoostMode.Normal);
            // Có thể thêm một thông báo nhỏ ở đây "Đã gửi yêu cầu Boost!"
        }

        // LỆNH MỚI CHO NÚT MAX BOOST
        [RelayCommand]
        private async Task LaunchMax(Game? game)
        {
            if (game == null) return;
            await _boostClient.RequestBoostAndLaunch(game, BoostMode.Max);
        }
    }
}