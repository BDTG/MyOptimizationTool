// In folder: Core/ProcessExecutor.cs
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace MyOptimizationTool.Core
{
    public class ProcessExecutor
    {
        public void ExecutePowerShellCommand(string command)
        {
            var startInfo = new ProcessStartInfo("powershell.exe")
            {
                Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{command.Replace("\"", "\\\"")}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                Verb = "runas"
            };
            ExecuteProcess(startInfo);
        }
        public string? ExecutePowerShellWithResult(string command)
        {
            var startInfo = new ProcessStartInfo("powershell.exe")
            {
                Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{command.Replace("\"", "\\\"")}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true, // Bắt buộc để đọc kết quả
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8,
                Verb = "runas"
            };
            try
            {
                using (var process = Process.Start(startInfo))
                {
                    if (process == null) return null;
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    if (!string.IsNullOrEmpty(error))
                    {
                        Debug.WriteLine($"[PowerShell Error]: {error}");
                    }
                    return output.Trim();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error executing process with result: {ex.Message}");
                return null;
            }
        }

        private void ExecuteProcess(ProcessStartInfo startInfo)
        {
            try
            {
                using (var process = Process.Start(startInfo))
                {
                    process?.WaitForExit(15000);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error executing process '{startInfo.FileName}': {ex.Message}");
            }
        }
    }
}