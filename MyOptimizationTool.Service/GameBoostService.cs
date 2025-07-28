// In project: MyOptimizationTool.Service
// File: Core/GameBoostService.cs
using MyOptimizationTool.Shared.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.IO;

namespace MyOptimizationTool.Service.Core
{
    public class GameBoostService
    {
        private readonly TweakScriptExecutor _tweakExecutor = new();
        private List<string> _stoppedServices = new();
        private bool _explorerKilled = false;

        private readonly List<string> _normalModeServices = new() { "BITS", "SysMain" };
        private readonly List<string> _maxModeServices = new() { "BITS", "SysMain", "WSearch", "Spooler" };

        // PHƯƠNG THỨC HOÀN CHỈNH ĐỂ TỐI ƯU, KHỞI CHẠY VÀ KHÔI PHỤC
        public async Task OptimizeAndLaunch(Game game, BoostMode mode)
        {
            // Bước 1: Áp dụng các tối ưu
            await ApplyBoost(mode);

            // Bước 2: Khởi chạy và theo dõi game
            var gameProcess = LaunchGame(game);
            if (gameProcess != null)
            {
                await gameProcess.WaitForExitAsync(); // Chờ đến khi người dùng thoát game
            }

            // Bước 3: Hoàn tác tất cả các tối ưu
            await RestoreSystem();
        }

        public async Task ApplyBoost(BoostMode mode)
        {
            var servicesToStop = (mode == BoostMode.Normal) ? _normalModeServices : _maxModeServices;
            await StopServices(servicesToStop);
            await _tweakExecutor.ExecuteFromFileAsync("NetworkTweaks.json");

            if (mode == BoostMode.Max)
            {
                KillProcess("explorer.exe");
                _explorerKilled = true;
            }
        }

        public async Task RestoreSystem()
        {
            await RestartStoppedServices();
            await _tweakExecutor.ExecuteNetworkResetScriptAsync();
            if (_explorerKilled)
            {
                LaunchProcess("explorer.exe");
                _explorerKilled = false;
            }
        }

        #region Private Methods
        private Process? LaunchGame(Game game)
        {
            try
            {
                return Process.Start(new ProcessStartInfo(game.ExecutablePath)
                {
                    WorkingDirectory = Path.GetDirectoryName(game.ExecutablePath),
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error launching game: {ex.Message}");
                return null;
            }
        }

        private Task StopServices(List<string> serviceNames)
        {
            return Task.Run(() =>
            {
                _stoppedServices.Clear();
                foreach (var name in serviceNames)
                {
                    try
                    {
                        ServiceController sc = new ServiceController(name);
                        if (sc.Status == ServiceControllerStatus.Running)
                        {
                            sc.Stop();
                            sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(15));
                            _stoppedServices.Add(name);
                            Debug.WriteLine($"Stopped service: {name}");
                        }
                    }
                    catch (Exception ex) { Debug.WriteLine($"Failed to stop service {name}: {ex.Message}"); }
                }
            });
        }

        private Task RestartStoppedServices()
        {
            return Task.Run(() =>
            {
                foreach (var name in _stoppedServices)
                {
                    try
                    {
                        ServiceController sc = new ServiceController(name);
                        if (sc.Status == ServiceControllerStatus.Stopped)
                        {
                            sc.Start();
                            sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(15));
                            Debug.WriteLine($"Restarted service: {name}");
                        }
                    }
                    catch (Exception ex) { Debug.WriteLine($"Failed to restart service {name}: {ex.Message}"); }
                }
            });
        }

        private void KillProcess(string processName)
        {
            try
            {
                foreach (var process in Process.GetProcessesByName(processName))
                {
                    process.Kill();
                }
                Debug.WriteLine($"Killed process: {processName}");
            }
            catch (Exception ex) { Debug.WriteLine($"Failed to kill process {processName}: {ex.Message}"); }
        }

        private void LaunchProcess(string processName)
        {
            try
            {
                Process.Start(processName);
            }
            catch (Exception ex) { Debug.WriteLine($"Failed to launch process {processName}: {ex.Message}"); }
        }
        #endregion
    }
}