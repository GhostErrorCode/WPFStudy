using MemoDesktop.ApiResponses;
using MemoDesktop.Dtos.Memo;
using MemoDesktop.Dtos.ToDo;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace MemoDesktop.Services.Implements
{
    /// <summary>
    /// 待办事项API服务实现
    /// 对应后端的ToDoController
    /// </summary>
    public class ToDoApiService : BaseApiService, IToDoApiService
    {
        /// <summary>
        /// API基础路径
        /// </summary>
        private const string BasePath = "Api/ToDo";

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="httpClient">HTTP客户端</param>
        public ToDoApiService(HttpClient httpClient) : base(httpClient)
        {
            // 调用基类构造函数
        }


        /// <summary>
        /// 根据ID获取待办事项
        /// </summary>
        /// <param name="id">待办事项ID</param>
        /// <returns>API响应结果</returns>
        public async Task<ApiResponse<ToDoDto>> GetToDoByIdAsync(int id)
        {
            // 构建请求URL
            string requestUrl = $"{BasePath}/GetToDoById/{id}";

            // 发送GET请求
            ApiResponse<ToDoDto> response = await this.GetAsync<ToDoDto>(requestUrl);

            return response;
        }

        /// <summary>
        /// 获取所有待办事项
        /// </summary>
        /// <returns>API响应结果</returns>
        public async Task<ApiResponse<List<ToDoDto>>> GetAllToDoAsync()
        {
            // 构建请求URL
            string requestUrl = $"{BasePath}/GetToDoAll";

            // 发送GET请求
            ApiResponse<List<ToDoDto>> response = await this.GetAsync<List<ToDoDto>>(requestUrl);

            return response;
        }

        /// <summary>
        /// 创建待办事项
        /// </summary>
        /// <param name="createDto">创建DTO</param>
        /// <returns>API响应结果</returns>
        public async Task<ApiResponse<ToDoDto>> AddToDoAsync(CreateToDoDto createDto)
        {
            // 构建请求URL
            string requestUrl = $"{BasePath}/AddToDo";

            // 发送POST请求
            ApiResponse<ToDoDto> response = await this.PostAsync<ToDoDto>(requestUrl, createDto);

            return response;
        }

        /// <summary>
        /// 更新待办事项
        /// </summary>
        /// <param name="updateDto">更新DTO</param>
        /// <returns>API响应结果</returns>
        public async Task<ApiResponse<ToDoDto>> UpdateToDoAsync(UpdateToDoDto updateDto)
        {
            // 构建请求URL
            string requestUrl = $"{BasePath}/UpdateToDo";

            // 发送PUT请求
            ApiResponse<ToDoDto> response = await this.PutAsync<ToDoDto>(requestUrl, updateDto);

            return response;
        }

        /// <summary>
        /// 删除待办事项
        /// </summary>
        /// <param name="id">待办事项ID</param>
        /// <returns>API响应结果</returns>
        public async Task<ApiResponse<int>> DeleteToDoAsync(int id)
        {
            // 构建请求URL
            string requestUrl = $"{BasePath}/DeleteToDo/{id}";

            // 发送DELETE请求
            ApiResponse<int> response = await this.DeleteAsync<int>(requestUrl);

            return response;
        }
    }
}
