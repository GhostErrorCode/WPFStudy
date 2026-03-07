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
                List<Memo> memos = await this._memoRepository.FindAsync(
                    (Memo memo) => memo.UserId == this._userService.UserId,
                    (IQueryable<Memo> memoOrderby) => memoOrderby.OrderByDescending((Memo memo) => memo.CreateDate));
                // 输出日志
                this._logger.LogInformation("[MemoService] [用户：{Account}（{Id}）] 查询全部备忘录完成。数据条数={Count}", this._userService.UserAccount, this._userService.UserId, memos.Count);
                // 转为Dto集合并返回
                return ServiceResult<List<MemoDto>>.Success("查询全部备忘录成功", memos.ToMemoDtoCollection());
            }
            catch (Exception ex)
            {
                // 输出日志
                this._logger.LogInformation("[MemoService] [用户：{Account}（{Id}）] 查询全部备忘录失败。异常信息：{ex}", this._userService.UserAccount, this._userService.UserId, ex);
                // 转为Dto集合并返回
                return ServiceResult<List<MemoDto>>.Failure("查询全部备忘录失败");
            }
        }

        // 添加备忘录
        public async Task<ServiceResult<MemoDto>> AddMemoAsync(AddMemoDto addMemoDto)
        {
            try
            {
                // 判断传入的addMemoDto是否为NULL，如果是则直接输出日志并返回失败结果
                if(addMemoDto == null || string.IsNullOrWhiteSpace(addMemoDto.Title) || string.IsNullOrWhiteSpace((addMemoDto.Content)))
                {
                    // 输出日志
                    this._logger.LogWarning("[MemoService] [用户：{Account}（{Id}）] 添加备忘录失败。备忘录标题或内容为空", this._userService.UserAccount, this._userService.UserId);
                    // 转为Dto集合并返回
                    return ServiceResult<MemoDto>.Failure("添加备忘录失败：备忘录标题或内容为空");
                }
                else
                {
                    // 传入的备忘录标题和内容均不为空
                    // 将addMemoDto映射为实体并添加UserId后写入数据库
                    Memo memo = await this._memoRepository.AddAsync(addMemoDto.ToMemo(this._userService.UserId));
                    // 通过工作单元提交至数据库并判断是否成功
                    if(await this._unitOfWork.SaveChangesAsync() > 0)
                    {
                        // 提交至数据库成功
                        this._logger.LogInformation("[MemoService] [用户：{Account}（{Id}）] 添加备忘录成功。数据：{@Memo}", this._userService.UserAccount, this._userService.UserId, memo);
                        return ServiceResult<MemoDto>.Success("添加备忘录成功",memo.ToMemoDto());
                    }
                    else
                    {
                        // 提交至数据库失败
                        this._logger.LogWarning("[MemoService] [用户：{Account}（{Id}）] 添加备忘录失败。保存的数据未生效", this._userService.UserAccount, this._userService.UserId);
                        return ServiceResult<MemoDto>.Failure("添加备忘录失败，请稍后重试");
                    }
                }
            }
            catch (Exception ex)
            {
                // 输出日志
                this._logger.LogError("[MemoService] [用户：{Account}（{Id}）] 添加备忘录失败。异常信息：{ex}", this._userService.UserAccount, this._userService.UserId, ex);
                // 转为Dto集合并返回
                return ServiceResult<MemoDto>.Failure("添加备忘录失败");
            }
        }
    }
}
