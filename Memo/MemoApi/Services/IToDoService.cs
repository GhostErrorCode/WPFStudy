using MemoApi.Entities;
using MemoApi.Results;
using Microsoft.AspNetCore.Mvc;

namespace MemoApi.Services
{
    public interface IToDoService
    {
        // ========== 待办事项服务层接口 ==========

        // 根据ID查询待办事项
        public Task<ServiceResult<ToDo>> GetToDoByIdAsync(int id);

        // 查询所有待办事项
        public Task<ServiceResult<List<ToDo>>> GetToDoAllAsync();

        // 更新待办事项
        public Task<ServiceResult<ToDo>> UpdateToDoAsync(ToDo toDo);

        // 添加待办事项
        public Task<ServiceResult<ToDo>> AddToDoAsync(ToDo toDo);

        // 删除待办事项
        public Task<ServiceResult<int>> DeleteToDoAsync(int id);
    }
}
