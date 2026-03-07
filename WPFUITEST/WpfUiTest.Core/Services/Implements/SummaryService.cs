using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using WpfUiTest.Core.Data.Entities;
using WpfUiTest.Core.Data.Repositories.Interfaces;
using WpfUiTest.Core.Data.UnitOfWork.Interfaces;
using WpfUiTest.Core.DTOs;
using WpfUiTest.Core.Services.Interfaces;
using WpfUiTest.Shared.Enums;
using WpfUiTest.Shared.Utilities;

namespace WpfUiTest.Core.Services.Implements
{
    // 首页汇总服务层
    public class SummaryService : ISummaryService
    {
        // 字段：用户服务
        private readonly IUserService _userService;
        // 字段：备忘录仓储
        private readonly IMemoRepository _memoRepository;
        // 字段：待办事项仓储
        private readonly IToDoRepository _toDoRepository;
        // 字段：工作单元
        private readonly IUnitOfWork _unitOfWork;
        // 字段：日志管理
        private readonly ILogger<SummaryService> _logger;

        // ==================== 构造函数 ====================
        public SummaryService(IMemoRepository memoRepository, IToDoRepository toDoRepository, IUserService userService, IUnitOfWork unitOfWork, ILogger<SummaryService> logger)
        {
            // 初始化字段
            this._memoRepository = memoRepository;
            this._toDoRepository = toDoRepository;
            this._userService = userService;
            this._unitOfWork = unitOfWork;
            this._logger = logger;
        }

        // 数据汇总，统计待办事项总量、待办事项完成数量、待办事项完成比例、备忘录总数等
        public async Task<ServiceResult<SummaryDto>> GetSummaryAsync()
        {
            try
            {
                // 待办总数
                int toDoTotal = await this._toDoRepository.CountAsync((ToDo toDo) => toDo.UserId == this._userService.UserId);
                // 待办完成数
                int toDoCompleted = await this._toDoRepository.CountAsync((ToDo toDo) => toDo.UserId == this._userService.UserId && toDo.Status == TodoStatusEnum.Completed);

                SummaryDto summaryDto = new SummaryDto()
                {
                    ToDoTotal = toDoTotal,
                    TodoCompleted = toDoCompleted,
                    TodoCompletionRate = toDoTotal == 0 ? "0.00%" : $"{(double)toDoCompleted / toDoTotal * 100:F2}%",
                    MemoTotal = await this._memoRepository.CountAsync((Memo memo) => memo.UserId == this._userService.UserId),
                    Custom = 0
                };

                this._logger.LogInformation("[SummaryService] [用户：{Account}（{Id}）] 查询汇总数据完成。数据：待办总量={ToDoTotal}，待办完成量={TodoCompleted}，待办完成比例={TodoCompletionRate}，备忘总量={MemoTotal}，自定义={Custom}", this._userService.UserAccount, this._userService.UserId, summaryDto.ToDoTotal, summaryDto.TodoCompleted, summaryDto.TodoCompletionRate, summaryDto.MemoTotal, summaryDto.Custom);
                // 返回汇总Dto
                return ServiceResult<SummaryDto>.Success("汇总数据获取成功", summaryDto);
            }
            catch(Exception ex)
            {
                this._logger.LogError("[SummaryService]  [用户：{Account}（{Id}）] 查询汇总数据时出现异常。异常信息：{ex}", this._userService.UserAccount, this._userService.UserId, ex);
                // 返回汇总Dto
                return ServiceResult<SummaryDto>.Failure("汇总数据获取失败");
            }
        }
    }
}
