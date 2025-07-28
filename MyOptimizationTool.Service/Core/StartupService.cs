// In folder: Core/StartupService.cs
using Microsoft.Win32;
using MyOptimizationTool.Shared.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyOptimizationTool.Service.Core
{
    public class StartupService
    {
        private const string DisabledPrefix = "[Disabled] ";

        public Task<List<StartupItem>> GetStartupItemsAsync()
        {
            return Task.Run(() =>
            {
                var items = new List<StartupItem>();
                items.AddRange(ScanRegistryKey(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", StartupItemLocation.Registry_CurrentUser));
                items.AddRange(ScanRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", StartupItemLocation.Registry_LocalMachine));
                items.AddRange(ScanStartupFolder(Environment.GetFolderPath(Environment.SpecialFolder.Startup), StartupItemLocation.Folder_CurrentUser));
                items.AddRange(ScanStartupFolder(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup), StartupItemLocation.Folder_LocalMachine));
                return items.OrderBy(i => i.Name).ToList();
            });
        }

        public void SetStartupItemStatus(StartupItem item, bool enabled)
        {
            if (item.LocationType == StartupItemLocation.Registry_CurrentUser || item.LocationType == StartupItemLocation.Registry_LocalMachine)
            {
                SetRegistryItemStatus(item, enabled);
            }
            else
            {
                SetFolderItemStatus(item, enabled);
            }
        }

        private List<StartupItem> ScanRegistryKey(RegistryKey rootKey, string path, StartupItemLocation location)
        {
            var items = new List<StartupItem>();
            try
            {
                using (var key = rootKey.OpenSubKey(path))
                {
                    if (key == null) return items;
                    foreach (var valueName in key.GetValueNames())
                    {
                        var isEnabled = !valueName.StartsWith(DisabledPrefix);
                        var originalName = isEnabled ? valueName : valueName.Substring(DisabledPrefix.Length);

                        // SỬA LỖI Ở ĐÂY
                        // 1. Lấy đường dẫn file và lưu vào một biến
                        var filePath = key.GetValue(valueName)?.ToString() ?? "";

                        // 2. Sử dụng biến 'filePath' đã được tạo
                        items.Add(new StartupItem
                        {
                            Name = originalName,
                            OriginalName = valueName,
                            FilePath = filePath, // Dùng biến ở đây
                            Publisher = GetPublisherInfo(filePath), // Và dùng biến ở đây
                            IsEnabled = isEnabled,
                            LocationType = location,
                            LocationDisplay = location.ToString().Replace('_', ' ')
                        });
                    }
                }
            }
            catch (Exception ex) { Debug.WriteLine($"Error scanning registry key {path}: {ex.Message}"); }
            return items;
        }

        private List<StartupItem> ScanStartupFolder(string path, StartupItemLocation location)
        {
            var items = new List<StartupItem>();
            try
            {
                if (!Directory.Exists(path)) return items;
                foreach (var file in Directory.GetFiles(path))
                {
                    var fileInfo = new FileInfo(file);
                    var isEnabled = fileInfo.Extension.Equals(".lnk", StringComparison.OrdinalIgnoreCase);

                    // Chỉ xử lý các file .lnk hoặc file đã bị disable bởi chúng ta
                    if (!isEnabled && !fileInfo.Extension.Equals(".lnk_disabled", StringComparison.OrdinalIgnoreCase)) continue;

                    items.Add(new StartupItem
                    {
                        Name = Path.GetFileNameWithoutExtension(fileInfo.Name),
                        OriginalName = fileInfo.Name,
                        FilePath = fileInfo.FullName,
                        Publisher = GetPublisherInfo(fileInfo.FullName),
                        IsEnabled = isEnabled,
                        LocationType = location,
                        LocationDisplay = location.ToString().Replace('_', ' ')
                    });
                }
            }
            catch (Exception ex) { Debug.WriteLine($"Error scanning startup folder {path}: {ex.Message}"); }
            return items;
        }
        private string GetPublisherInfo(string path)
        {
            try
            {
                // Xử lý các đường dẫn có dấu ngoặc kép và có tham số
                var cleanPath = path.Trim();
                if (cleanPath.StartsWith("\""))
                {
                    cleanPath = cleanPath.Substring(1, cleanPath.IndexOf("\"", 1) - 1);
                }
                else
                {
                    // Tách đường dẫn khỏi các tham số (ví dụ: "app.exe -arg")
                    var parts = cleanPath.Split(new[] { ".exe", ".com", ".bat" }, StringSplitOptions.None);
                    if (parts.Length > 0)
                    {
                        cleanPath = parts[0] + cleanPath.Substring(parts[0].Length, 4);
                    }
                }

                if (File.Exists(cleanPath))
                {
                    var versionInfo = FileVersionInfo.GetVersionInfo(cleanPath);
                    return !string.IsNullOrEmpty(versionInfo.CompanyName) ? versionInfo.CompanyName : "Không rõ";
                }
            }
            catch { /* Bỏ qua lỗi */ }
            return "Không rõ";
        }
        private void SetRegistryItemStatus(StartupItem item, bool enabled)
        {
            var keyPath = item.LocationType == StartupItemLocation.Registry_CurrentUser
                ? @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run"
                : @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
            var rootKey = item.LocationType == StartupItemLocation.Registry_CurrentUser
                ? Registry.CurrentUser
                : Registry.LocalMachine;

            try
            {
                using (var key = rootKey.OpenSubKey(keyPath, true))
                {
                    if (key == null) return;

                    string oldName = item.OriginalName;
                    string newName = enabled ? item.Name : DisabledPrefix + item.Name;

                    var value = key.GetValue(oldName);
                    if (value != null)
                    {
                        key.DeleteValue(oldName);
                        key.SetValue(newName, value);
                        item.OriginalName = newName; // Cập nhật lại tên gốc
                    }
                }
            }
            catch (Exception ex) { Debug.WriteLine($"Error changing registry status for {item.Name}: {ex.Message}"); }
        }

        private void SetFolderItemStatus(StartupItem item, bool enabled)
        {
            try
            {
                string newPath = enabled
                    ? Path.ChangeExtension(item.FilePath, ".lnk")
                    : Path.ChangeExtension(item.FilePath, ".lnk_disabled");

                File.Move(item.FilePath, newPath);
                item.FilePath = newPath; // Cập nhật lại đường dẫn
            }
            catch (Exception ex) { Debug.WriteLine($"Error changing folder status for {item.Name}: {ex.Message}"); }
        }
    }
}