using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using WpfUiTest.Core.Data.Entities;
using WpfUiTest.Core.Data.Repositories.Interfaces;
using WpfUiTest.Core.Data.UnitOfWork.Interfaces;
using WpfUiTest.Core.DTOs.ToDo;
using WpfUiTest.Core.Mapping;
using WpfUiTest.Core.Services.Interfaces;
using WpfUiTest.Shared.Enums;
using WpfUiTest.Shared.Utilities;

namespace WpfUiTest.Core.Services.Implements
{
    // 待办事项服务接口实现类
    public class ToDoService : IToDoService
    {
        // ==================== 字段、属性 ====================
        // 字段：用户服务
        private readonly IUserService _userService;
        // 字段：待办事项仓储
        private readonly IToDoRepository _toDoRepository;
        // 字段：工作单元
        private readonly IUnitOfWork _unitOfWork;
        // 字段：日志管理
        private readonly ILogger<MemoService> _logger;


        // ==================== 构造函数 ====================
        public ToDoService(IUserService userService, IToDoRepository toDoRepository, IUnitOfWork unitOfWork, ILogger<MemoService> logger)
        {
            // 初始化字段
            this._toDoRepository = toDoRepository;
            this._userService = userService;
            this._unitOfWork = unitOfWork;
            this._logger = logger;
        }

        // ==================== 实现方法 ====================
        // 查询所有未完成（待办）待办事项
        public async Task<ServiceResult<List<ToDoDto>>> GetAllPendingToDosAsync()
        {
            try
            {
                // 从数据库中查到当前用户未完成的待办事项实体集合
                List<ToDo> toDos = await this._toDoRepository.FindAsync((ToDo toDo)=>toDo.UserId == this._userService.UserId && toDo.Status == TodoStatusEnum.Pending);
                // 输出日志
                this._logger.LogInformation("[ToDoService] 查询当前用户 {Account} 全部未完成待办事项完成。数据条数={Count}", this._userService.UserAccount, toDos.Count);
                // 转为Dto集合并返回
                return ServiceResult<List<ToDoDto>>.Success("查询当前用户全部未完成待办事项完成", toDos.ToToDoDtoCollection());
            }
            catch (Exception ex)
            {
                // 输出日志
                this._logger.LogInformation("查询当前用户 {Account} 全部未完成待办事项失败。异常信息：{ex}", this._userService.UserAccount, ex);
                // 转为Dto集合并返回
                return ServiceResult<List<ToDoDto>>.Failure("查询当前用户全部未完成待办事项失败");
            }
        }
    }
}
