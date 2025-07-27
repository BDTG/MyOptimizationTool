// In folder: Shared/Core/GameService.cs
using MyOptimizationTool.Models; // Sửa using
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

// SỬA LỖI: Bọc tất cả code trong một namespace
namespace MyOptimizationTool.Core
{
    // SỬA LỖI: Bọc tất cả code trong một class
    public class GameService
    {
        private readonly string _saveFilePath;
        private readonly string _artCacheFolder;

        public GameService()
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appDataFolder = Path.Combine(localAppData, "MyOptimizationTool");
            Directory.CreateDirectory(appDataFolder);
            
            _saveFilePath = Path.Combine(appDataFolder, "games.json");
            _artCacheFolder = Path.Combine(appDataFolder, "GameArt");
            Directory.CreateDirectory(_artCacheFolder);
        }
        public async Task<Game?> CreateGameFromFileAsync(string exePath)
        {
            try
            {
                var newGame = new Game
                {
                    Name = Path.GetFileNameWithoutExtension(exePath),
                    ExecutablePath = exePath
                };

                // Trích xuất và lưu icon
                string? iconPath = await ExtractAndSaveIconAsync(exePath);
                newGame.ImagePath = iconPath;

                return newGame;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error creating game from file: {ex.Message}");
                return null;
            }
        }

        private Task<string?> ExtractAndSaveIconAsync(string exePath)
        {
            return Task.Run(() =>
            {
                try
                {
                    Icon? ico = Icon.ExtractAssociatedIcon(exePath);
                    if (ico != null)
                    {
                        using (var bmp = ico.ToBitmap())
                        {
                            // Tạo tên file an toàn và lưu vào thư mục cache
                            var safeName = Path.GetFileNameWithoutExtension(exePath).Replace(" ", "_");
                            var imagePath = Path.Combine(_artCacheFolder, $"{safeName}.png");

                            bmp.Save(imagePath, ImageFormat.Png);

                            return imagePath; // Trả về đường dẫn đến file icon đã lưu
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Could not extract icon for {exePath}: {ex.Message}");
                }
                return null;
            });
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