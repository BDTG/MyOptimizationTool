// In folder: Core/TweakScriptExecutor.cs
using MyOptimizationTool.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace MyOptimizationTool.Core
{
    public class TweakScriptExecutor
    {
        private readonly RegistryManager _regManager = new();

        // Phương thức này đọc và thực thi một file kịch bản JSON
        public async Task ExecuteFromFileAsync(string scriptFileName)
        {
            var scriptPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Scripts", scriptFileName);
            if (!File.Exists(scriptPath))
            {
                Debug.WriteLine($"Script file not found: {scriptPath}");
                return;
            }

            var json = await File.ReadAllTextAsync(scriptPath);
            var scriptFile = JsonSerializer.Deserialize<TweakScriptFile>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (scriptFile?.Tweaks == null) return;

            foreach (var tweak in scriptFile.Tweaks)
            {
                if (tweak.Type == "registry" && tweak.Path != null && tweak.ValueName != null && tweak.Data != null)
                {
                    // Chuyển đổi DataType từ chuỗi sang Enum
                    var valueKind = tweak.DataType?.ToUpper() == "DWORD" ? RegistryValueKind.DWord : RegistryValueKind.String;

                    // Chuyển đổi Data sang đúng kiểu int nếu là DWord
                    object valueData = valueKind == RegistryValueKind.DWord
                        ? int.Parse(tweak.Data.ToString()!)
                        : tweak.Data.ToString()!;

                    _regManager.SetRegistryValue(tweak.Path, tweak.ValueName, valueData, valueKind);
                }
                else if (tweak.Type == "script" && tweak.ScriptContent != null)
                {
                    await ExecuteBatchScriptAsync(tweak.ScriptContent);
                }
            }
        }

        // Phương thức này thực thi kịch bản khôi phục mạng mặc định
        public Task ExecuteNetworkResetScriptAsync()
        {
            string scriptContent = @"
@echo off
echo Resetting network settings to Windows defaults...

rem Reset TCP/IP and Winsock
(
    netsh int ip reset
    netsh interface ipv4 reset
    netsh interface ipv6 reset
    netsh interface tcp reset
    netsh winsock reset
) > nul

echo Reinstalling network drivers...
rem Uninstalls all physical network adapters, then rescans to let Windows reinstall them
PowerShell -NoP -C ""foreach ($dev in Get-PnpDevice -Class Net -Status 'OK' | Where-Object { $_.InstanceId -like 'PCI*' -or $_.InstanceId -like 'USB*' }) { pnputil /remove-device $dev.InstanceId }"" > nul
pnputil /scan-devices > nul

echo Finished, please reboot your device for changes to apply.
";
            return ExecuteBatchScriptAsync(scriptContent);
        }

        // Phương thức chung để chạy một đoạn kịch bản batch
        private Task ExecuteBatchScriptAsync(string scriptContent)
        {
            return Task.Run(() =>
            {
                var tempPath = Path.GetTempFileName() + ".bat";
                File.WriteAllText(tempPath, scriptContent);

                var startInfo = new ProcessStartInfo(tempPath)
                {
                    UseShellExecute = true,
                    CreateNoWindow = true,
                    Verb = "runas" // Yêu cầu quyền Administrator
                };

                try
                {
                    using (var process = Process.Start(startInfo))
                    {
                        process?.WaitForExit(30000); // Chờ tối đa 30 giây
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error executing batch script: {ex.Message}");
                }
                finally
                {
                    File.Delete(tempPath); // Luôn dọn dẹp file tạm sau khi chạy
                }
            });
        }
    }
}