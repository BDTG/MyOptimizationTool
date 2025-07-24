using LibreHardwareMonitor.Hardware;
using MyOptimizationTool.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Threading.Tasks;

namespace MyOptimizationTool.Services
{
    public class SystemInfoService
    {
        private PerformanceCounter? _cpuCounter;
        private ManagementObject? _ramObject;
        private double _totalRamGB;
        private Computer? _computer;

        public Task InitializeAsync()
        {
            return InitializePerformanceCountersAsync();
        }

        public Task<ComputerSpecs> GetStaticComputerSpecsAsync()
        {
            return Task.Run(() =>
            {
                var gpus = new ManagementObjectSearcher("SELECT Name FROM Win32_VideoController")
                    .Get().OfType<ManagementObject>()
                    .Select(obj => obj["Name"]?.ToString()?.Trim() ?? "N/A").ToList();

                return new ComputerSpecs
                {
                    OsVersion = GetWmiProperty("Win32_OperatingSystem", "Caption"),
                    Cpu = GetWmiProperty("Win32_Processor", "Name"),
                    Gpus = gpus,
                    Motherboard = $"{GetWmiProperty("Win32_BaseBoard", "Manufacturer")} {GetWmiProperty("Win32_BaseBoard", "Product")}"
                };
            });
        }

        private string GetWmiProperty(string wmiClass, string wmiProperty, string wmiProperty2 = "")
        {
            try
            {
                var searcher = new ManagementObjectSearcher($"SELECT {wmiProperty}{(string.IsNullOrEmpty(wmiProperty2) ? "" : $", {wmiProperty2}")} FROM {wmiClass}");
                var obj = searcher.Get().OfType<ManagementObject>().FirstOrDefault();
                if (obj == null) return "N/A";

                var value1 = obj[wmiProperty]?.ToString()?.Trim() ?? "N/A";
                var value2 = string.IsNullOrEmpty(wmiProperty2) ? "" : obj[wmiProperty2]?.ToString()?.Trim() ?? "";
                return $"{value1} {value2}".Trim();
            }
            catch { return "Lỗi"; }
        }

        private Task InitializePerformanceCountersAsync()
        {
            return Task.Run(() =>
            {
                try
                {
                    _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                    _cpuCounter.NextValue();

                    var ramSearcher = new ManagementObjectSearcher("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem");
                    var ramObj = ramSearcher.Get().OfType<ManagementObject>().FirstOrDefault();
                    if (ramObj != null)
                        _totalRamGB = Math.Round(Convert.ToUInt64(ramObj["TotalPhysicalMemory"]) / (1024.0 * 1024.0 * 1024.0), 2);

                    var osSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
                    _ramObject = osSearcher.Get().OfType<ManagementObject>().FirstOrDefault();
                    _computer = new Computer
                    {
                        IsGpuEnabled = true
                    };
                    _computer.Open();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[ERROR Initializing PerfCounters] {ex.Message}");
                }
            });
        }

        public SystemMetrics GetCurrentMetrics()
        {
            try
            {
                float cpuUsage = _cpuCounter?.NextValue() ?? 0;
                double freeRamMB = _ramObject != null ? Convert.ToDouble(_ramObject["FreePhysicalMemory"]) : 0;
                double usedRamGB = _totalRamGB - (freeRamMB / 1024.0 / 1024.0);

                var diskInfos = DriveInfo.GetDrives().Where(d => d.IsReady).Select(drive => new DiskInfo
                {
                    Name = drive.Name,
                    TotalSizeGB = drive.TotalSize / (1024.0 * 1024.0 * 1024.0),
                    FreeSpaceGB = drive.TotalFreeSpace / (1024.0 * 1024.0 * 1024.0),
                }).ToList();
                var gpuInfos = GetGpuMetrics();

                return new SystemMetrics
                {
                    CpuUsagePercentage = cpuUsage,
                    ProcessCount = Process.GetProcesses().Length,
                    RamTotalGB = _totalRamGB,
                    RamUsedGB = Math.Round(usedRamGB, 2),
                    Disks = diskInfos,
                    GpuInfo = gpuInfos
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR GetCurrentMetrics] {ex.Message}");
                return new SystemMetrics();
            }
        }

        // --- PHIÊN BẢN HOÀN CHỈNH, ĐÃ DỌN DẸP ---
        private List<GpuMetrics> GetGpuMetrics()
        {
            var gpuMetricsList = new List<GpuMetrics>();
            if (_computer?.Hardware == null) return gpuMetricsList;

            var gpus = _computer.Hardware.Where(h =>
                h.HardwareType == HardwareType.GpuAmd ||
                h.HardwareType == HardwareType.GpuNvidia ||
                h.HardwareType == HardwareType.GpuIntel);

            foreach (var hardware in gpus)
            {
                hardware.Update();

                var metrics = new GpuMetrics { Name = hardware.Name };

                // === TÌM KIẾM LOAD ===
                // Ưu tiên 'GPU Core' cho card rời, sau đó fallback về 'D3D 3D' cho card tích hợp.
                var gpuLoad = hardware.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Load && s.Name == "GPU Core") ??
                              hardware.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Load && s.Name == "D3D 3D");

                // === TÌM KIẾM NHIỆT ĐỘ ===
                // Ưu tiên 'GPU Core', sau đó đến 'Hot Spot'. Sẽ là null nếu không tìm thấy.
                var gpuTemp = hardware.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Temperature && s.Name == "GPU Core") ??
                              hardware.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Temperature && s.Name.Contains("Hot Spot"));

                // === TÌM KIẾM VRAM ===
                // Phân biệt rõ giữa VRAM riêng (Dedicated/Used) và VRAM chia sẻ (Shared)
                ISensor? vramUsed, vramTotal;

                if (hardware.HardwareType == HardwareType.GpuIntel) // Card Intel thường dùng VRAM chia sẻ
                {
                    vramUsed = hardware.Sensors.FirstOrDefault(s => s.SensorType == SensorType.SmallData && s.Name == "D3D Shared Memory Used");
                    vramTotal = hardware.Sensors.FirstOrDefault(s => s.SensorType == SensorType.SmallData && s.Name == "D3D Shared Memory Total");
                }
                else // Card rời dùng VRAM riêng
                {
                    vramUsed = hardware.Sensors.FirstOrDefault(s => s.SensorType == SensorType.SmallData && s.Name == "GPU Memory Used");
                    vramTotal = hardware.Sensors.FirstOrDefault(s => s.SensorType == SensorType.SmallData && s.Name == "GPU Memory Total");
                }

                // === GÁN GIÁ TRỊ ===
                if (gpuLoad?.Value != null) metrics.CoreLoad = gpuLoad.Value.Value;
                if (gpuTemp?.Value != null) metrics.Temperature = gpuTemp.Value.Value;
                if (vramUsed?.Value != null) metrics.VramUsedMB = vramUsed.Value.Value;
                if (vramTotal?.Value != null) metrics.VramTotalMB = vramTotal.Value.Value;

                gpuMetricsList.Add(metrics);
            }
            return gpuMetricsList;
        }

        public void Cleanup()
        {
            _computer?.Close();
        }
    }
}