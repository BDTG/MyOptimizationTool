// In folder: Core/NetworkService.cs
using System.Diagnostics;
using System.Linq;
using System.Management.Automation; // Cần thêm gói NuGet System.Management.Automation
using System;

namespace MyOptimizationTool.Core
{
    public class NetworkService
    {
        // Phương thức này dịch lại logic tìm key trong file batch của bạn
        public string? GetActiveNetworkAdapterRegistryPath()
        {
            try
            {
                using (var ps = PowerShell.Create())
                {
                    // Lệnh 1: Lấy PNPDeviceID của các card mạng vật lý
                    ps.AddScript("(Get-CimInstance Win32_NetworkAdapter).PNPDeviceID | Where-Object { $_ -like 'PCI\\VEN_*' }");
                    var pnpDeviceIds = ps.Invoke<string>();

                    if (ps.HadErrors) return null;

                    var firstPnpDeviceId = pnpDeviceIds.FirstOrDefault();
                    if (firstPnpDeviceId == null) return null;

                    // Lệnh 2: Từ PNPDeviceID, truy vấn registry để lấy GUID của driver
                    ps.Commands.Clear();
                    ps.AddScript($"(Get-ItemProperty -Path 'HKLM:\\SYSTEM\\CurrentControlSet\\Enum\\{firstPnpDeviceId}').Driver");
                    var driverGuid = ps.Invoke<string>().FirstOrDefault();

                    if (ps.HadErrors || driverGuid == null) return null;

                    // Trả về đường dẫn hoàn chỉnh
                    return $"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Class\\{driverGuid}";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting network adapter key: {ex.Message}");
                return null;
            }
        }
    }
}