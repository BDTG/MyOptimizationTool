// In folder: Core/TweakManager.cs
using MyOptimizationTool.Models;
using System.Linq;

namespace MyOptimizationTool.Core
{
    public class TweakManager
    {
        private readonly RegistryManager _registryManager = new();
        private readonly PowerShellManager _powerShellManager = new();

        public void ApplyTweak(SystemTweak tweak)
        {
            switch (tweak.Type)
            {
                case TweakType.Registry:
                    object? valueToSet = tweak.IsApplied ? tweak.EnabledValue : tweak.DisabledValue;
                    if (valueToSet != null)
                    {
                        _registryManager.SetRegistryValue(tweak.RegistryPath!, tweak.ValueName!, valueToSet, tweak.ValueKind);
                    }
                    break;
                case TweakType.PowerShell:
                    if (tweak.IsApplied && !string.IsNullOrEmpty(tweak.PowerShellCommand))
                    {
                        _powerShellManager.ExecuteScript(tweak.PowerShellCommand);
                    }
                    // Logic hoàn tác cho PowerShell có thể phức tạp và cần thêm sau
                    break;
            }
        }

        public void CheckTweakStatus(SystemTweak tweak)
        {
            switch (tweak.Type)
            {
                case TweakType.Registry:
                    var currentValue = _registryManager.GetRegistryValue(tweak.RegistryPath!, tweak.ValueName!);
                    tweak.IsApplied = currentValue != null && currentValue.ToString() == tweak.EnabledValue?.ToString();
                    break;
                case TweakType.PowerShell:
                    if (!string.IsNullOrEmpty(tweak.CheckStatusCommand))
                    {
                        var result = _powerShellManager.ExecuteScriptWithResult(tweak.CheckStatusCommand).FirstOrDefault();
                        tweak.IsApplied = result != null && result.ToString() == tweak.ExpectedCheckValue;
                    }
                    else
                    {
                        tweak.IsApplied = false; // Mặc định là chưa áp dụng nếu không có lệnh kiểm tra
                    }
                    break;
            }
        }
    }
}