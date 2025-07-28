// In file: Core/RegistryManager.cs
using Microsoft.Win32;
using System.Diagnostics; // Cần cho Debug.WriteLine
using System;
using MyOptimizationTool.Shared.Models;

namespace MyOptimizationTool.Service.Core
{
    public class RegistryManager
    {
        public void SetRegistryValue(string keyPath, string valueName, object value, RegistryValueKind valueKind)
        {
            try
            {
                keyPath = keyPath.Replace("HKCU", "HKEY_CURRENT_USER").Replace("HKLM", "HKEY_LOCAL_MACHINE");
                Registry.SetValue(keyPath, valueName, value, valueKind);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error setting registry value at '{keyPath}': {ex.Message}");
            }
        }
        public void DeleteRegistryValue(string keyPath, string valueName)
        {
            try
            {
                keyPath = keyPath.Replace("HKCU", "HKEY_CURRENT_USER").Replace("HKLM", "HKEY_LOCAL_MACHINE");
                using (var key = Registry.CurrentUser.OpenSubKey(keyPath, true) ?? Registry.LocalMachine.OpenSubKey(keyPath, true))
                {
                    key?.DeleteValue(valueName, false); // false = không ném lỗi nếu không tìm thấy
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting registry value at '{keyPath}': {ex.Message}");
            }
        }

        public object? GetRegistryValue(string keyPath, string valueName)
        {
            try
            {
                return Registry.GetValue(keyPath, valueName, null);
            }
            catch { return null; }
        }

        // Sửa lại để nhận SystemTweak
        public void ApplyTweak(SystemTweak tweak)
        {
            // SỬA LỖI: Kiểm tra null cho các thuộc tính quan trọng
            if (string.IsNullOrEmpty(tweak.RegistryPath) || string.IsNullOrEmpty(tweak.ValueName))
            {
                return; // Thoát nếu đường dẫn hoặc tên giá trị bị rỗng
            }

            object? valueToSet = tweak.IsApplied ? tweak.EnabledValue : tweak.DisabledValue;
            if (valueToSet != null)
            {
                SetRegistryValue(tweak.RegistryPath, tweak.ValueName, valueToSet, tweak.ValueKind);
            }
        }

        public void CheckTweakStatus(SystemTweak tweak)
        {
            // SỬA LỖI: Kiểm tra null chặt chẽ hơn
            if (string.IsNullOrEmpty(tweak.RegistryPath) || string.IsNullOrEmpty(tweak.ValueName) || tweak.EnabledValue == null)
            {
                tweak.IsApplied = false;
                return;
            }

            var currentValue = GetRegistryValue(tweak.RegistryPath, tweak.ValueName);
            tweak.IsApplied = currentValue != null && currentValue.ToString() == tweak.EnabledValue.ToString();
        }
    }
}