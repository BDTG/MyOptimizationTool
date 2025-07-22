// In folder: Core/TweakManager.cs
using MyOptimizationTool.Models;
using System;
using System.Linq;

namespace MyOptimizationTool.Core
{
    public class TweakManager
    {
        private readonly RegistryManager _registryManager = new();
        private readonly ProcessExecutor _processExecutor = new();

        public void ApplyTweak(SystemTweak tweak)
        {
            if (tweak.Type == TweakType.Registry)
            {
                if (string.IsNullOrEmpty(tweak.RegistryPath) || string.IsNullOrEmpty(tweak.ValueName)) return;

                object? valueToSet = tweak.IsApplied ? tweak.EnabledValue : tweak.DisabledValue;

                if (valueToSet is string strValue && strValue.Equals("DELETE", StringComparison.OrdinalIgnoreCase))
                {
                    _registryManager.DeleteRegistryValue(tweak.RegistryPath, tweak.ValueName);
                }
                else if (valueToSet != null)
                {
                    _registryManager.SetRegistryValue(tweak.RegistryPath, tweak.ValueName, valueToSet, tweak.ValueKind);
                }
            }
            else if (tweak.Type == TweakType.PowerShell)
            {
                if (tweak.IsApplied && !string.IsNullOrEmpty(tweak.PowerShellCommand))
                {
                    _processExecutor.ExecutePowerShellCommand(tweak.PowerShellCommand);
                }
            }
        }

        public void CheckTweakStatus(SystemTweak tweak)
        {
            if (tweak.Type == TweakType.Registry)
            {
                if (string.IsNullOrEmpty(tweak.RegistryPath) || string.IsNullOrEmpty(tweak.ValueName))
                {
                    tweak.IsApplied = false;
                    return;
                }
                var currentValue = _registryManager.GetRegistryValue(tweak.RegistryPath, tweak.ValueName);

                if (tweak.EnabledValue is string enabledStr && enabledStr.Equals("DELETE", StringComparison.OrdinalIgnoreCase))
                {
                    tweak.IsApplied = currentValue == null;
                }
                else
                {
                    tweak.IsApplied = currentValue != null && tweak.EnabledValue != null && currentValue.ToString() == tweak.EnabledValue.ToString();
                }
            }
            else if (tweak.Type == TweakType.PowerShell)
            {
                tweak.IsApplied = false;
            }
        }
    }
}