using MemoDesktop.ApiResponses;
using MemoDesktop.Dtos.Dashboard;
using MemoDesktop.Dtos.Memo;
using MemoDesktop.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace MemoDesktop.Services.Implements
{
    /// <summary>
    /// 数据汇总API实现
    /// 对应后端的SummaryController
    /// </summary>
    public class SummaryApiService : BaseApiService, ISummaryApiService
    {
        /// <summary>
        /// API基础路径
        /// </summary>
        private const string BasePath = "Api/Summary";

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="httpClient">HTTP客户端</param>
        public SummaryApiService(HttpClient httpClient) : base(httpClient)
        {
            // 调用基类构造函数
        }

        /// <summary>
        /// 获取首页数据汇总结果
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ApiResponse<SummaryDto>> GetSummaryAsync()
        {
            // 构建请求URL
            string requestUrl = $"{BasePath}/GetSummary";

            // 发送GET请求
            ApiResponse<SummaryDto> response = await this.GetAsync<SummaryDto>(requestUrl);

            // 返回结果
            return response;
        }
    }
}
