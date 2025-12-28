using MemoDesktop.ApiResponses;
using MemoDesktop.Dtos.Memo;
using MemoDesktop.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace MemoDesktop.Services.Implements
{
    /// <summary>
    /// 备忘录API服务实现
    /// 对应后端的MemoController
    /// </summary>
    public class MemoApiService : BaseApiService, IMemoApiService
    {
        /// <summary>
        /// API基础路径
        /// </summary>
        private const string BasePath = "Api/Memo";

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="httpClient">HTTP客户端</param>
        public MemoApiService(HttpClient httpClient) : base(httpClient)
        {
            // 调用基类构造函数
        }


        /// <summary>
        /// 根据ID获取备忘录
        /// </summary>
        /// <param name="id">备忘录ID</param>
        /// <returns>API响应结果</returns>
        public async Task<ApiResponse<MemoDto>> GetMemoByIdAsync(int id)
        {
            // 构建请求URL
            string requestUrl = $"{BasePath}/GetMemoById/{id}";

            // 发送GET请求
            ApiResponse<MemoDto> response = await this.GetAsync<MemoDto>(requestUrl);

            return response;
        }

        /// <summary>
        /// 获取所有备忘录
        /// </summary>
        /// <returns>API响应结果</returns>
        public async Task<ApiResponse<List<MemoDto>>> GetAllMemoAsync()
        {
            // 构建请求URL
            string requestUrl = $"{BasePath}/GetMemoAll";

            // 发送GET请求
            ApiResponse<List<MemoDto>> response = await this.GetAsync<List<MemoDto>>(requestUrl);

            return response;
        }

        /// <summary>
        /// 根据条件获取备忘录
        /// </summary>
        /// <returns>API响应结果</returns>
        public async Task<ApiResponse<List<MemoDto>>> GetMemoByConditionAsync(string? memoTitle = null)
        {
            // 构建请求URL
            string requestUrl = $"{BasePath}/GetMemoByCondition?memoTitle={memoTitle}";

            // 发送GET请求
            ApiResponse<List<MemoDto>> response = await this.GetAsync<List<MemoDto>>(requestUrl);

            return response;
        }

        /// <summary>
        /// 创建备忘录
        /// </summary>
        /// <param name="createDto">创建DTO</param>
        /// <returns>API响应结果</returns>
        public async Task<ApiResponse<MemoDto>> AddMemoAsync(CreateMemoDto createDto)
        {
            // 构建请求URL
            string requestUrl = $"{BasePath}/AddMemo";

            // 发送POST请求
            ApiResponse<MemoDto> response = await this.PostAsync<MemoDto, CreateMemoDto>(requestUrl, createDto);

            return response;
        }

        /// <summary>
        /// 更新备忘录
        /// </summary>
        /// <param name="updateDto">更新DTO</param>
        /// <returns>API响应结果</returns>
        public async Task<ApiResponse<MemoDto>> UpdateMemoAsync(UpdateMemoDto updateDto)
        {
            // 构建请求URL
            string requestUrl = $"{BasePath}/UpdateMemo";

            // 发送PUT请求
            ApiResponse<MemoDto> response = await this.PutAsync<MemoDto, UpdateMemoDto>(requestUrl, updateDto);

            return response;
        }

        /// <summary>
        /// 删除备忘录
        /// </summary>
        /// <param name="id">备忘录ID</param>
        /// <returns>API响应结果</returns>
        public async Task<ApiResponse<int>> DeleteMemoAsync(int id)
        {
            // 构建请求URL
            string requestUrl = $"{BasePath}/DeleteMemo/{id}";

            // 发送DELETE请求
            ApiResponse<int> response = await this.DeleteAsync<int>(requestUrl);

            return response;
        }
    }
}
