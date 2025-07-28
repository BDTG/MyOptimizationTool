// In folder: Core/TweakScriptExecutor.cs
using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Win32;
using MyOptimizationTool.Shared.Models;
using MyOptimizationTool.Service.Core;

namespace MyOptimizationTool.Service
{
    public class TweakScriptExecutor
    {
        private readonly RegistryManager _regManager = new();
        private readonly NetworkService _netService = new();

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
                switch (tweak.Type)
                {
                    case "registry":
                        if (tweak.Path != null && tweak.ValueName != null && tweak.Data != null)
                        {
                            var valueKind = tweak.DataType?.ToUpper() == "DWORD" ? RegistryValueKind.DWord : RegistryValueKind.String;
                            object valueData = valueKind == RegistryValueKind.DWord ? int.Parse(tweak.Data.ToString()!) : tweak.Data.ToString()!;
                            _regManager.SetRegistryValue(tweak.Path, tweak.ValueName, valueData, valueKind);
                        }
                        break;

                    case "script":
                        if (tweak.ScriptContent != null)
                        {
                            await ExecuteBatchScriptAsync(tweak.ScriptContent);
                        }
                        break;

                    case "networkAdapterRegistry":
                        ExecuteNetworkAdapterTweak(tweak);
                        break;
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

        // Phương thức mới để thực thi tinh chỉnh card mạng
        private void ExecuteNetworkAdapterTweak(TweakScriptItem tweak)
        {
            var adapterPath = _netService.GetActiveNetworkAdapterRegistryPath();
            if (adapterPath == null)
            {
                Debug.WriteLine("Could not find active network adapter registry path. Skipping tweak.");
                return;
            }

            var valueName = tweak.ValueName;
            var data = tweak.Data;

            if (valueName == null || data == null) return;

            // Mặc định là REG_SZ vì đó là kiểu dữ liệu trong script batch gốc
            _regManager.SetRegistryValue(adapterPath, valueName, data.ToString()!, RegistryValueKind.String);
            _regManager.SetRegistryValue(adapterPath, $"*{valueName}", data.ToString()!, RegistryValueKind.String);

            Debug.WriteLine($"Applied Network Tweak: {valueName} = {data} to {adapterPath}");
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
                    Verb = "runas"
                };

                try
                {
                    using (var process = Process.Start(startInfo))
                    {
                        process?.WaitForExit(30000);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error executing batch script: {ex.Message}");
                }
                finally
                {
                    File.Delete(tempPath);
                }
            });
        }
    }
}