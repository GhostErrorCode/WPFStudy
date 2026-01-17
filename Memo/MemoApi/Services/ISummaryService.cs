using MemoApi.Dtos.Dashboard;
using MemoApi.Results;

namespace MemoApi.Services
{
    public interface ISummaryService
    {
        // ========== 数据汇总接口 ==========

        // 数据汇总，统计待办事项总量、待办事项完成数量、待办事项完成比例、备忘录总数
        public Task<ServiceResult<SummaryDto>> GetSummaryAsync();
    }
}
