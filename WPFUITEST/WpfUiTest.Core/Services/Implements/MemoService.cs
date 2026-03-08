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
                this._logger.LogError("[MemoService] [用户：{Account}（{Id}）] 查询全部备忘录失败。异常信息：{ex}", this._userService.UserAccount, this._userService.UserId, ex);
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
                return ServiceResult<MemoDto>.Failure("添加备忘录时出现异常");
            }
        }

        // 修改备忘录
        public async Task<ServiceResult<MemoDto>> UpdateMemoAsync(UpdateMemoDto updateMemoDto)
        {
            try
            {
                // 判断传入的 updateMemoDto 是否为 NULL，如果是则直接输出日志并返回失败结果
                if (updateMemoDto == null || string.IsNullOrWhiteSpace(updateMemoDto.Title) || string.IsNullOrWhiteSpace((updateMemoDto.Content)))
                {
                    // 输出日志
                    this._logger.LogWarning("[MemoService] [用户：{Account}（{Id}）] 修改备忘录失败。备忘录标题或内容为空", this._userService.UserAccount, this._userService.UserId);
                    // 转为Dto集合并返回
                    return ServiceResult<MemoDto>.Failure("修改备忘录失败：备忘录标题或内容为空");
                }

                // 跨用户修改备忘录失败。判断传入的 updateMemoDto 的 userId 是否跟当前登录的用户一致，否则就提示无权修改他人备忘录
                if(updateMemoDto.UserId != this._userService.UserId)
                {
                    // 输出日志
                    this._logger.LogWarning("[MemoService] [用户：{Account}（{Id}）] 修改备忘录失败。无法跨用户修改备忘录，用户（ID={currentUserId}）正尝试修改用户（ID={userId}）的备忘录数据", this._userService.UserAccount, this._userService.UserId, this._userService.UserId, updateMemoDto.UserId);
                    // 转为Dto集合并返回
                    return ServiceResult<MemoDto>.Failure("修改备忘录失败：无权修改他人备忘录");
                }

                // ========== 正常业务流程。输入的内容均不为空且是当前用户的备忘录，则开始修改 ==========
                Memo? memo = await this._memoRepository.FindSingleAsync(m => m.Id == updateMemoDto.Id && m.UserId == updateMemoDto.UserId);
                // 判断是否获取到了，如果数据库中查到了就修改并保存至数据库
                if(memo != null)
                {
                    // 如果从数据库中查到了待修改的备忘录数据，就修改它，并保存至数据库
                    // 跟踪查到的备忘录实体
                    this._memoRepository.Update(memo);
                    // 修改备忘录实体
                    memo.ToUpdatedMemo(updateMemoDto);
                    // 保存至数据库
                    if (await this._unitOfWork.SaveChangesAsync() > 0)
                    {
                        // 保存至数据库成功
                        // 输出日志
                        this._logger.LogInformation("[MemoService] [用户：{Account}（{Id}）] 修改备忘录成功。数据：{@Memo}", this._userService.UserAccount, this._userService.UserId, memo);
                        // 转为Dto集合并返回
                        return ServiceResult<MemoDto>.Success("修改备忘录成功", memo.ToMemoDto());
                    }
                    else
                    {
                        // 保存至数据库失败
                        // 输出日志
                        this._logger.LogWarning("[MemoService] [用户：{Account}（{Id}）] 修改备忘录失败，保存的数据未生效", this._userService.UserAccount, this._userService.UserId);
                        // 转为Dto集合并返回
                        return ServiceResult<MemoDto>.Failure("修改备忘录失败，请稍后重试");
                    }
                }
                else
                {
                    // 未找到此实体
                    // 输出日志
                    this._logger.LogWarning("[MemoService] [用户：{Account}（{Id}）] 修改备忘录失败。未找到需要修改的备忘录实体，需要修改的备忘录：{@UpdateMemoDto}", this._userService.UserAccount, this._userService.UserId, updateMemoDto);
                    // 转为Dto集合并返回
                    return ServiceResult<MemoDto>.Failure("修改备忘录失败：未找到此备忘录，请检查或稍后重试");
                }
            }
            catch (Exception ex)
            {
                // 输出日志
                this._logger.LogError("[MemoService] [用户：{Account}（{Id}）] 修改备忘录失败。异常信息：{ex}", this._userService.UserAccount, this._userService.UserId, ex);
                // 转为Dto集合并返回
                return ServiceResult<MemoDto>.Failure("修改备忘录时出现异常");
            }
        }

        // 删除备忘录
        public async Task<ServiceResult<bool>> DeleteMemoAsync(DeleteMemoDto deleteMemoDto)
        {
            try
            {
                // 判断传入的 deleteMemoDto 是否为 NULL，如果是则直接输出日志并返回失败结果
                if (deleteMemoDto == null || deleteMemoDto.Id > 0)
                {
                    // 输出日志
                    this._logger.LogWarning("[MemoService] [用户：{Account}（{Id}）] 删除备忘录失败。需要删除的备忘录数据为空", this._userService.UserAccount, this._userService.UserId);
                    // 转为Dto集合并返回
                    return ServiceResult<bool>.Failure("删除备忘录失败：需要删除的备忘录数据为空");
                }

                // 判断传入的 deleteMemoDto 的 userId 是否跟当前登录的用户一致，否则就提示无权删除他人备忘录
                if (deleteMemoDto.UserId != this._userService.UserId)
                {
                    // 输出日志
                    this._logger.LogWarning("[MemoService] [用户：{Account}（{Id}）] 删除备忘录失败。无法跨用户删除备忘录，用户（ID={currentUserId}）正尝试修改用户（ID={userId}）的备忘录数据", this._userService.UserAccount, this._userService.UserId, this._userService.UserId, deleteMemoDto.UserId);
                    // 转为Dto集合并返回
                    return ServiceResult<bool>.Failure("删除备忘录失败：无权删除他人备忘录");
                }

                // ========== 正常业务流程。输入的内容均不为空且是当前用户的备忘录，则开始删除 ==========
                // 调用仓储删除服务，判断返回的bool值，true表示找到了且已标记为删除，保存更改即可。反之失败
                if(await this._memoRepository.DeleteAsync(m => m.Id == deleteMemoDto.Id && m.UserId == deleteMemoDto.UserId && m.Title == deleteMemoDto.Title))
                {
                    if(await this._unitOfWork.SaveChangesAsync() > 0)
                    {
                        // 保存至数据库成功
                        // 输出日志
                        this._logger.LogInformation("[MemoService] [用户：{Account}（{Id}）] 删除备忘录成功。ID={Id}，标题=\"{Title}\"", this._userService.UserAccount, this._userService.UserId, deleteMemoDto.Id, deleteMemoDto.Title);
                        // 转为Dto集合并返回
                        return ServiceResult<bool>.Success("删除备忘录成功", true);
                    }
                    else
                    {
                        // 保存至数据库失败
                        // 输出日志
                        this._logger.LogWarning("[MemoService] [用户：{Account}（{Id}）] 删除备忘录失败，删除的数据未生效", this._userService.UserAccount, this._userService.UserId);
                        // 转为Dto集合并返回
                        return ServiceResult<bool>.Failure("修改备忘录失败，请稍后重试");
                    }
                }
                else
                {
                    // 未找到此实体
                    // 输出日志
                    this._logger.LogWarning("[MemoService] [用户：{Account}（{Id}）] 删除备忘录失败。未找到需要删除的备忘录实体，需要删除的备忘录：ID={Id}，标题=\"{Title}\"", this._userService.UserAccount, this._userService.UserId, deleteMemoDto.Id, deleteMemoDto.Title);
                    // 转为Dto集合并返回
                    return ServiceResult<bool>.Failure("修改备忘录失败：未找到此备忘录，请检查或稍后重试");
                }
            }
            catch (Exception ex)
            {
                // 输出日志
                this._logger.LogError("[MemoService] [用户：{Account}（{Id}）] 删除备忘录失败。异常信息：{ex}", this._userService.UserAccount, this._userService.UserId, ex);
                // 转为Dto集合并返回
                return ServiceResult<bool>.Failure("删除备忘录时出现异常");
            }
        }
    }
}
