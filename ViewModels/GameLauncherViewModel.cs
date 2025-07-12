// In folder: ViewModels/GameLauncherViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyOptimizationTool;
using MyOptimizationTool.Core;
using MyOptimizationTool.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using WinRT.Interop;

public partial class GameLauncherViewModel : ObservableObject
{
    private readonly GameService _gameService;
    public ObservableCollection<Game> Games { get; } = new();

    // SỬA LỖI: Đổi tên field thành chữ thường, không có gạch dưới
    [ObservableProperty]
    private bool isOptimizing;

    public GameLauncherViewModel()
    {
        _gameService = new GameService();
        _ = LoadGamesAsync();
    }

    private async Task LoadGamesAsync()
    {
        var loadedGames = await _gameService.LoadGamesAsync();
        foreach (var game in loadedGames) { Games.Add(game); }
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
            var newGame = new Game { Name = Path.GetFileNameWithoutExtension(file.Name), ExecutablePath = file.Path };
            if (!Games.Any(g => g.ExecutablePath.Equals(newGame.ExecutablePath, StringComparison.OrdinalIgnoreCase)))
            {
                Games.Add(newGame);
                await _gameService.SaveGamesAsync(Games);
            }
        }
    }

    [RelayCommand]
    private async Task LaunchGameAsync(Game? game)
    {
        if (game is null) return;
        IsOptimizing = true;
        await Task.Delay(3000);
        _gameService.LaunchGame(game);
        IsOptimizing = false;
    }
}