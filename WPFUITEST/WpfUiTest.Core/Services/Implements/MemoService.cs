using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using WpfUiTest.Core.Data.Entities;
using WpfUiTest.Core.Data.Repositories.Implements;
using WpfUiTest.Core.Data.Repositories.Interfaces;
using WpfUiTest.Core.Data.UnitOfWork.Implements;
using WpfUiTest.Core.Data.UnitOfWork.Interfaces;
using WpfUiTest.Core.DTOs.Memo;
using WpfUiTest.Core.DTOs.ToDo;
using WpfUiTest.Core.Mapping;
using WpfUiTest.Core.Services.Interfaces;
using WpfUiTest.Shared.Utilities;

namespace WpfUiTest.Core.Services.Implements
{
    // 备忘录服务接口实现类
    public class MemoService : IMemoService
    {
        // ==================== 字段、属性 ====================
        // 字段：用户服务
        private readonly IUserService _userService;
        // 字段：待办事项仓储
        private readonly IMemoRepository _memoRepository;
        // 字段：工作单元
        private readonly IUnitOfWork _unitOfWork;
        // 字段：日志管理
        private readonly ILogger<MemoService> _logger;


        // ==================== 构造函数 ====================
        public MemoService(IUserService userService, IMemoRepository memoRepository, IUnitOfWork unitOfWork, ILogger<MemoService> logger)
        {
            // 初始化字段
            this._memoRepository = memoRepository;
            this._userService = userService;
            this._unitOfWork = unitOfWork;
            this._logger = logger;
        }


        // ==================== 实现方法 ====================
        // 查询所有备忘录
        public async Task<ServiceResult<List<MemoDto>>> GetAllMemosAsync()
        {
            try
            {
                // 从数据库中查到当前用户备忘录实体集合
                List<Memo> memos = await this._memoRepository.FindAsync((Memo memo) => memo.UserId == this._userService.UserId);
                // 输出日志
                this._logger.LogInformation("[MemoService] 查询当前用户 {Account} 全部备忘录完成。数据条数={Count}", this._userService.UserAccount, memos.Count);
                // 转为Dto集合并返回
                return ServiceResult<List<MemoDto>>.Success("查询当前用户全部备忘录完成", memos.ToMemoDtoCollection());
            }
            catch (Exception ex)
            {
                // 输出日志
                this._logger.LogInformation("查询当前用户 {Account} 全部备忘录失败。异常信息：{ex}", this._userService.UserAccount, ex);
                // 转为Dto集合并返回
                return ServiceResult<List<MemoDto>>.Failure("查询当前用户全部备忘录失败");
            }
        }
    }
}
