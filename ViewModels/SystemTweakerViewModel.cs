// In folder: ViewModels/SystemTweakerViewModel.cs
using Microsoft.Win32;
using MyOptimizationTool.Core;
using MyOptimizationTool.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MyOptimizationTool.ViewModels
{
    public class SystemTweakerViewModel
    {
        private readonly TweakManager _tweakManager;
        public ObservableCollection<SystemTweak> Tweaks { get; set; }

        public SystemTweakerViewModel()
        {
            _tweakManager = new TweakManager();
            Tweaks = new ObservableCollection<SystemTweak>();
            LoadTweaks();
        }

        private void LoadTweaks()
        {
            Tweaks.Clear();
            var tweakList = new[]
            {
                /*new SystemTweak
                {
                    DisplayName = "Tắt Fault Tolerant Heap (FTH)",
                    Description = "Ngăn Windows áp dụng các bản vá hiệu năng cho các ứng dụng thường xuyên bị crash.",
                    
                    // Hành động khi BẬT tweak
                    Command_Apply = "rundll32.exe fthsvc.dll,FthSysprepSpecialize",
                    RegistryPath_Apply = @"HKLM\SOFTWARE\Microsoft\FTH",
                    ValueName_Apply = "Enabled",
                    EnabledValue_Apply = 0,
                    ValueKind_Apply = RegistryValueKind.DWord,

                    // Hành động khi TẮT tweak (khôi phục)
                    RegistryPath_Undo = @"HKLM\SOFTWARE\Microsoft\FTH",
                    ValueName_Undo = "Enabled",
                    DisabledValue_Undo = 1,
                    ValueKind_Undo = RegistryValueKind.DWord,

                    // Dùng để kiểm tra trạng thái công tắc
                    RegistryPath_Check = @"HKLM\SOFTWARE\Microsoft\FTH",
                    ValueName_Check = "Enabled",
                    EnabledValue_Check = 0
                },*/
                new SystemTweak
                {
                    DisplayName = "Tắt Ứng dụng chạy nền (BackgroundAccess)",
                    Description = "Ngăn các ứng dụng WinRT truy cập tài nguyên ở chế độ nền.",
                    Type = TweakType.Registry,
                    RegistryPath = @"HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\BackgroundAccessApplications",
                    ValueName = "GlobalUserDisabled",
                    EnabledValue = 1,
                    DisabledValue = 0,
                    ValueKind = RegistryValueKind.DWord
                },
                new SystemTweak
                {
                    DisplayName = "Tắt Ứng dụng chạy nền (Search)",
                    Description = "Ngăn các ứng dụng nền được lập chỉ mục bởi Windows Search.",
                    Type = TweakType.Registry,
                    RegistryPath = @"HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Search",
                    ValueName = "BackgroundAppGlobalToggle",
                    EnabledValue = 0,
                    DisabledValue = 1,
                    ValueKind = RegistryValueKind.DWord
                },

                // === Tinh chỉnh Bảo trì Hệ thống ===
                new SystemTweak
                {
                    DisplayName = "Tắt Tự động Đánh thức để Bảo trì",
                    Description = "Ngăn máy tính tự động bật dậy từ chế độ Sleep để chạy tác vụ bảo trì hệ thống.",
                    Type = TweakType.Registry,
                    RegistryPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\Task Scheduler\Maintenance",
                    ValueName = "WakeUp",
                    EnabledValue = 0,
                    DisabledValue = 1,
                    ValueKind = RegistryValueKind.DWord
                },

                // === Tinh chỉnh Hiệu năng Giao diện ===
                new SystemTweak
                {
                    DisplayName = "Tắt Tự động Nhận diện Thư mục",
                    Description = "Cải thiện hiệu năng File Explorer bằng cách không tự động xác định loại thư mục (Ảnh, Video, v.v.).",
                    Type = TweakType.Registry,
                    RegistryPath = @"HKCU\Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\Bags\AllFolders\Shell",
                    ValueName = "FolderType",
                    EnabledValue = "DELETE",
                    DisabledValue = "NotSpecified",
                    ValueKind = RegistryValueKind.String
                },
                new SystemTweak
                {
                    DisplayName = "Sử dụng Menu Chuột Phải cũ (Windows 10)",
                    Description = "Tắt menu chuột phải mới của Windows 11 và quay lại menu đầy đủ của Windows 10.",
                    Type = TweakType.Registry,
                    RegistryPath = @"HKEY_CURRENT_USER\Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a6}\InprocServer32",
                    ValueName = "", // Giá trị (Default)
                    EnabledValue = "",
                    DisabledValue = " ", // Một giá trị không tồn tại để RegistryManager có thể ghi đè và sau đó có thể xóa
                    ValueKind = RegistryValueKind.String
                },

                // === Tinh chỉnh Hiệu năng Hệ thống ===
                new SystemTweak
                {
                    DisplayName = "Cấu hình Dịch vụ Lập lịch Đa phương tiện (MMCSS)",
                    Description = "Phân bổ 90% tài nguyên CPU cho các tác vụ ưu tiên cao (như game) thay vì 80% mặc định của Windows.",
                    Type = TweakType.Registry,
                    RegistryPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile",
                    ValueName = "SystemResponsiveness",
                    EnabledValue = 10,
                    DisabledValue = 20,
                    ValueKind = RegistryValueKind.DWord
                },

                // === Tinh chỉnh Quyền riêng tư & Gỡ cài đặt ===
                new SystemTweak
                {
                    DisplayName = "Gỡ cài đặt OneDrive",
                    Description = "Chạy tiến trình gỡ cài đặt hoàn toàn OneDrive khỏi hệ thống.",
                    Type = TweakType.PowerShell,
                    PowerShellCommand = @"
                        if (Test-Path ""$Env:SystemRoot\System32\OneDriveSetup.exe"") {
                            Start-Process ""$Env:SystemRoot\System32\OneDriveSetup.exe"" -ArgumentList '/uninstall' -NoNewWindow -Wait
                        }
                        if (Test-Path ""$Env:SystemRoot\SysWOW64\OneDriveSetup.exe"") {
                    Start-Process ""$Env:SystemRoot\SysWOW64\OneDriveSetup.exe"" -ArgumentList '/uninstall' -NoNewWindow -Wait
                        }
                    ",
                    CheckStatusCommand = "Test-Path \"$Env:LOCALAPPDATA\\Microsoft\\OneDrive\"",
                    ExpectedCheckValue = "False"
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
                _tweakManager.CheckTweakStatus(tweak);
                Tweaks.Add(tweak);
            }
        }

        private void OnTweakPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SystemTweak.IsApplied) && sender is SystemTweak tweak)
            {
                // SỬA LỖI: Sử dụng tên đúng là '_tweakManager'
                _tweakManager.ApplyTweak(tweak);
            }
        }
    }
}