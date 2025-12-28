using MemoDesktop.ApiResponses;
using MemoDesktop.Dtos.Memo;
using MemoDesktop.Dtos.ToDo;
using System;
using System.Collections.Generic;
using System.Text;

namespace MemoDesktop.Services.Interfaces
{
    /// <summary>
    /// 待办事项API服务接口
    /// </summary>
    public interface IToDoApiService
    {
        /// <summary>
        /// 根据ID获取待办事项
        /// </summary>
        /// <param name="id">待办事项ID</param>
        /// <returns>API响应结果</returns>
        public Task<ApiResponse<ToDoDto>> GetToDoByIdAsync(int id);

        /// <summary>
        /// 获取所有待办事项
        /// </summary>
        /// <returns>API响应结果</returns>
        public Task<ApiResponse<List<ToDoDto>>> GetAllToDoAsync();

        /// <summary>
        /// 根据条件获取待办事项
        /// </summary>
        /// <returns>API响应结果</returns>
        public Task<ApiResponse<List<ToDoDto>>> GetToDoByConditionAsync(string toDoTitle, int? toDoStatus = null);

        /// <summary>
        /// 创建待办事项
        /// </summary>
        /// <param name="createDto">创建DTO</param>
        /// <returns>API响应结果</returns>
        public Task<ApiResponse<ToDoDto>> AddToDoAsync(CreateToDoDto createDto);

        /// <summary>
        /// 更新待办事项
        /// </summary>
        /// <param name="updateDto">更新DTO</param>
        /// <returns>API响应结果</returns>
        public Task<ApiResponse<ToDoDto>> UpdateToDoAsync(UpdateToDoDto updateDto);

        /// <summary>
        /// 删除待办事项
        /// </summary>
        /// <param name="id">待办事项ID</param>
        /// <returns>API响应结果</returns>
        public Task<ApiResponse<int>> DeleteToDoAsync(int id);
    }
}
