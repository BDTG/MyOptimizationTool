// In project: MyOptimizationTool.Shared
// File: Services/GameService.cs

using MyOptimizationTool.Shared.Models; // <-- ĐÂY LÀ DÒNG QUAN TRỌNG NHẤT ĐÃ ĐƯỢC SỬA
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace MyOptimizationTool.Shared.Services
{
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
                            var safeName = Path.GetFileNameWithoutExtension(exePath).Replace(" ", "_");
                            var imagePath = Path.Combine(_artCacheFolder, $"{safeName}.png");

                            bmp.Save(imagePath, ImageFormat.Png);

                            return imagePath;
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
    }
}