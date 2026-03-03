using System;
using System.Collections.Generic;
using System.Text;
using WpfUiTest.Core.DTOs;
using WpfUiTest.Shared.Utilities;

namespace WpfUiTest.Core.Services.Interfaces
{
    // 首页汇总数据接口
    public interface ISummaryService
    {
        // 数据汇总，统计待办事项总量、待办事项完成数量、待办事项完成比例、备忘录总数等
        public Task<ServiceResult<SummaryDto>> GetSummaryAsync();
    }
}
