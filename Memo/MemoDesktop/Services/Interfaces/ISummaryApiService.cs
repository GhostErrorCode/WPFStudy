using MemoDesktop.ApiResponses;
using MemoDesktop.Dtos.Dashboard;
using System;
using System.Collections.Generic;
using System.Text;

namespace MemoDesktop.Services.Interfaces
{
    /// <summary>
    /// 首页数据汇总的服务接口
    /// </summary>
    public interface ISummaryApiService
    {
        /// <summary>
        /// 获取数据汇总结果
        /// </summary>
        /// <returns></returns>
        public Task<ApiResponse<SummaryDto>> GetSummaryAsync();
    }
}
