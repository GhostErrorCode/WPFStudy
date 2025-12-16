using MemoDesktop.ApiResponses;
using MemoDesktop.Dtos.Memo;
using System;
using System.Collections.Generic;
using System.Text;

namespace MemoDesktop.Services
{
    /// <summary>
    /// 备忘录API服务接口
    /// </summary>
    public interface IMemoApiService
    {
        /// <summary>
        /// 根据ID获取备忘录
        /// </summary>
        /// <param name="id">备忘录ID</param>
        /// <returns>API响应结果</returns>
        public Task<ApiResponse<MemoDto>> GetMemoByIdAsync(int id);

        /// <summary>
        /// 获取所有备忘录
        /// </summary>
        /// <returns>API响应结果</returns>
        public Task<ApiResponse<List<MemoDto>>> GetAllMemoAsync();

        /// <summary>
        /// 创建备忘录
        /// </summary>
        /// <param name="createDto">创建DTO</param>
        /// <returns>API响应结果</returns>
        public Task<ApiResponse<MemoDto>> AddMemoAsync(CreateMemoDto createDto);

        /// <summary>
        /// 更新备忘录
        /// </summary>
        /// <param name="updateDto">更新DTO</param>
        /// <returns>API响应结果</returns>
        public Task<ApiResponse<MemoDto>> UpdateMemoAsync(UpdateMemoDto updateDto);

        /// <summary>
        /// 删除备忘录
        /// </summary>
        /// <param name="id">备忘录ID</param>
        /// <returns>API响应结果</returns>
        public Task<ApiResponse<int>> DeleteMemoAsync(int id);
    }
}
