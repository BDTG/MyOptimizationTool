// In folder: Core/StartupService.cs
// Logic để đọc và ghi vào Registry/Startup Folder sẽ được thêm sau
using MyOptimizationTool.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyOptimizationTool.Core
{
    public class StartupService
    {
        public Task<List<StartupItem>> GetStartupItemsAsync()
        {
            // Logic để quét registry và thư mục startup sẽ ở đây
            return Task.FromResult(new List<StartupItem>());
        }

        public void SetStartupItemStatus(StartupItem item, bool enabled)
        {
            // Logic để bật/tắt một mục khởi động sẽ ở đây
        }
    }
}