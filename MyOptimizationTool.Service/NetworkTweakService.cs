// In project: MyOptimizationTool.Service
// File: NetworkTweakService.cs
using MyOptimizationTool.Service.Core;
using System.Threading.Tasks;

namespace MyOptimizationTool.Service
{
    public class NetworkTweakService
    {
        private readonly TweakScriptExecutor _executor = new();

        public async Task Execute(bool isReset)
        {
            if (isReset)
            {
                await _executor.ExecuteNetworkResetScriptAsync();
            }
            else
            {
                await _executor.ExecuteFromFileAsync("NetworkTweaks.json");
            }
        }
    }
}