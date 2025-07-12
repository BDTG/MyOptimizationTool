// In folder: Shared/Core/GameService.cs
using MyOptimizationTool.Models; // Sửa using
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

// SỬA LỖI: Bọc tất cả code trong một namespace
namespace MyOptimizationTool.Core
{
    // SỬA LỖI: Bọc tất cả code trong một class
    public class GameService
    {
        private readonly string _saveFilePath;

        public GameService()
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appDataFolder = Path.Combine(localAppData, "MyOptimizationTool");
            Directory.CreateDirectory(appDataFolder);
            _saveFilePath = Path.Combine(appDataFolder, "games.json");
        }

        public async Task<List<Game>> LoadGamesAsync()
        {
            if (!File.Exists(_saveFilePath)) return new List<Game>();
            try
            {
                var json = await File.ReadAllTextAsync(_saveFilePath);
                return JsonSerializer.Deserialize<List<Game>>(json) ?? new List<Game>();
            }
            catch { return new List<Game>(); }
        }

        public async Task SaveGamesAsync(IEnumerable<Game> games)
        {
            var json = JsonSerializer.Serialize(games, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_saveFilePath, json);
        }

        public void LaunchGame(Game game)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(game.ExecutablePath)
                {
                    WorkingDirectory = Path.GetDirectoryName(game.ExecutablePath)
                };
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error launching game: {ex.Message}");
            }
        }
    }
}