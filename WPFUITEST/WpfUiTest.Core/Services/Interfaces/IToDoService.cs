using System;
using System.Collections.Generic;
using System.Text;
using WpfUiTest.Core.DTOs.ToDo;
using WpfUiTest.Shared.Utilities;

namespace WpfUiTest.Core.Services.Interfaces
{
    // 待办事项服务接口
    public interface IToDoService
    {
        // 以下方法均为用户隔离

        // 查询所有未完成（待办）待办事项
        public Task<ServiceResult<List<ToDoDto>>> GetAllPendingToDosAsync();
    }
}
