// In folder: Core/PowerShellManager.cs
using System.Collections.ObjectModel;
using System.Diagnostics; // Cần cho Process
using System.Management.Automation;
using System.Text;

namespace MyOptimizationTool.Core
{
    public class PowerShellManager
    {
        // --- PHƯƠNG THỨC NÀY ĐƯỢC VIẾT LẠI HOÀN TOÀN ---
        public void ExecuteScript(string script)
        {
            // Cấu hình để chạy một tiến trình powershell.exe mới
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                // -NoProfile: Không tải profile người dùng, chạy nhanh hơn
                // -ExecutionPolicy Bypass: Tạm thời bỏ qua chính sách thực thi
                // -Command: Chỉ định lệnh cần chạy
                Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{script.Replace("\"", "\\\"")}\"",
                UseShellExecute = false,      // Không dùng Shell của hệ điều hành
                CreateNoWindow = true,        // Chạy ẩn, không hiện cửa sổ PowerShell
                RedirectStandardOutput = true, // Cho phép đọc kết quả trả về
                RedirectStandardError = true,  // Cho phép đọc thông báo lỗi
            };

            using (Process process = Process.Start(startInfo)!)
            {
                process.WaitForExit(); // Chờ cho đến khi lệnh chạy xong
            }
        }

        // --- PHƯƠNG THỨC NÀY CŨNG ĐƯỢC VIẾT LẠI HOÀN TOÀN ---
        public Collection<PSObject> ExecuteScriptWithResult(string script)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{script.Replace("\"", "\\\"")}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8, // Đảm bảo đọc tiếng Việt (nếu có)
            };

            var output = new Collection<PSObject>();

            using (Process process = Process.Start(startInfo)!)
            {
                // Đọc kết quả từ tiến trình
                string result = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (!string.IsNullOrEmpty(error))
                {
                    Debug.WriteLine($"[PowerShell Error] {error}");
                }

                if (!string.IsNullOrWhiteSpace(result))
                {
                    // Chuyển kết quả dạng chuỗi thành đối tượng PSObject
                    output.Add(new PSObject(result.Trim()));
                }
            }

            return output;
        }
    }
}