// In folder: ViewModels/SystemTweakerViewModel.cs
using MyOptimizationTool.Core;
using MyOptimizationTool.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.Win32;

namespace MyOptimizationTool.ViewModels
{
    public class SystemTweakerViewModel
    {
        private readonly TweakManager tweakManager;
        public ObservableCollection<SystemTweak> Tweaks { get; set; }
        public SystemTweakerViewModel()
        {
            tweakManager = new TweakManager();
            Tweaks = new ObservableCollection<SystemTweak>();
            LoadTweaks();
        }
        private void LoadTweaks()
        {
            var tweakList = new[]
            {

                // --- TWEAK CŨ (REGISTRY) ---
                new SystemTweak
                {
                    DisplayName = "Sử dụng Menu Chuột Phải cũ (Windows 10)",
                    Description = "Tắt menu chuột phải mới của Windows 11 và quay lại menu đầy đủ của Windows 10.",
                    Type = TweakType.Registry,
                    RegistryPath = @"HKEY_CURRENT_USER\Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a6}\InprocServer32",
                    ValueName = "",
                    EnabledValue = "",
                    DisabledValue = " ",
                    ValueKind = RegistryValueKind.String
                },
                // --- TWEAK MỚI (POWERSHELL) - Lấy ý tưởng từ Sophia Script ---
                new SystemTweak
                {
                    DisplayName = "Gỡ cài đặt OneDrive",
                    Description = "Chạy tiến trình gỡ cài đặt hoàn toàn OneDrive khỏi hệ thống.",
                    Type = TweakType.PowerShell,
                    // Lệnh này tìm file setup của OneDrive và chạy nó với tham số uninstall
                    PowerShellCommand = @"
                        if (Test-Path ""$Env:SystemRoot\System32\OneDriveSetup.exe"") {
                            Start-Process ""$Env:SystemRoot\System32\OneDriveSetup.exe"" -ArgumentList '/uninstall' -NoNewWindow -Wait
                        }
                        if (Test-Path ""$Env:SystemRoot\SysWOW64\OneDriveSetup.exe"") {
                            Start-Process ""$Env:SystemRoot\SysWOW64\OneDriveSetup.exe"" -ArgumentList '/uninstall' -NoNewWindow -Wait
                        }
                    ",
                    // Lệnh kiểm tra xem thư mục OneDrive có còn tồn tại trong AppData không
                    CheckStatusCommand = "Test-Path \"$Env:LOCALAPPDATA\\Microsoft\\OneDrive\"",
                    ExpectedCheckValue = "False" // Tweak được coi là "đã áp dụng" nếu kết quả là False (không còn thư mục)
                },
                new SystemTweak
                {
                    DisplayName = "Tắt Telemetry của Cortana",
                    Description = "Ngăn Cortana thu thập dữ liệu về cách bạn sử dụng máy tính.",
                    Type = TweakType.Registry,
                    RegistryPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\Windows Search",
                    ValueName = "AllowCortana",
                    EnabledValue = 0,
                    DisabledValue = 1,
                    ValueKind = RegistryValueKind.DWord
                },
            };

            foreach (var tweak in tweakList)
            {
                tweak.PropertyChanged += OnTweakPropertyChanged;
                tweakManager.CheckTweakStatus(tweak);
                Tweaks.Add(tweak);
            }
        }

        private void OnTweakPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SystemTweak.IsApplied) && sender is SystemTweak tweak)
            {
                tweakManager.ApplyTweak(tweak);
            }
        }
    }
}