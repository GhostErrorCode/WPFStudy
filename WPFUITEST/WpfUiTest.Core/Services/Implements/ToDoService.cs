using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using WpfUiTest.Core.Data.Entities;
using WpfUiTest.Core.Data.Repositories.Interfaces;
using WpfUiTest.Core.Data.UnitOfWork.Interfaces;
using WpfUiTest.Core.DTOs.Memo;
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
        private readonly ILogger<ToDoService> _logger;


        // ==================== 构造函数 ====================
        public ToDoService(IUserService userService, IToDoRepository toDoRepository, IUnitOfWork unitOfWork, ILogger<ToDoService> logger)
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
                List<ToDo> toDos = await this._toDoRepository.FindAsync(
                    (ToDo toDo) => toDo.UserId == this._userService.UserId && toDo.Status == TodoStatusEnum.Pending,
                    (IQueryable<ToDo> toDoOrderBy) => toDoOrderBy.OrderByDescending((ToDo toDo) => toDo.CreateDate));
                // 输出日志
                this._logger.LogInformation("[ToDoService] [用户：{Account}（{Id}）] 查询全部未完成待办事项成功。数据条数={Count}", this._userService.UserAccount, this._userService.UserId, toDos.Count);
                // 转为Dto集合并返回
                return ServiceResult<List<ToDoDto>>.Success("查询全部未完成待办事项成功", toDos.ToToDoDtoCollection());
            }
            catch (Exception ex)
            {
                // 输出日志
                this._logger.LogError("[ToDoService] [用户：{Account}（{Id}）] 查询全部未完成待办事项时出现异常。异常信息：{ex}", this._userService.UserAccount, this._userService.UserId, ex);
                // 转为Dto集合并返回
                return ServiceResult<List<ToDoDto>>.Failure("查询全部未完成待办事项失败");
            }
        }

        // 添加待办事项
        public async Task<ServiceResult<ToDoDto>> AddToDoAsync(AddToDoDto addToDoDto)
        {
            try
            {
                // 判断传入的 addToDoDto 是否为 NULL，如果是则直接输出日志并返回失败结果
                if(addToDoDto == null || string.IsNullOrWhiteSpace(addToDoDto.Title) || string.IsNullOrWhiteSpace(addToDoDto.Content) || (addToDoDto.Status != TodoStatusEnum.Pending && addToDoDto.Status != TodoStatusEnum.Completed))
                {
                    // 输出日志
                    this._logger.LogWarning("[ToDoService] [用户：{Account}（{Id}）] 添加待办事项失败。待办事项标题或内容为空，或状态不符合规则", this._userService.UserAccount, this._userService.UserId);
                    // 转为Dto集合并返回
                    return ServiceResult<ToDoDto>.Failure("添加待办事项失败：待办事项标题或内容为空，或状态不符合规则");
                }
                else
                {
                    // 传入的 addToDoDto 数据均不为空且合法，则继续写入数据库操作
                    ToDo toDo = await this._toDoRepository.AddAsync(addToDoDto.ToToDo(this._userService.UserId));
                    // 通过工作单元提交至数据库并判断是否成功
                    if (await this._unitOfWork.SaveChangesAsync() > 0)
                    {
                        // 提交至数据库成功
                        this._logger.LogInformation("[ToDoService] [用户：{Account}（{Id}）] 添加待办事项成功。数据：{@ToDo}", this._userService.UserAccount, this._userService.UserId, toDo);
                        return ServiceResult<ToDoDto>.Success("添加待办事项成功", toDo.ToToDoDto());
                    }
                    else
                    {
                        // 提交至数据库失败
                        this._logger.LogWarning("[ToDoService] [用户：{Account}（{Id}）] 添加待办事项失败。保存的数据未生效", this._userService.UserAccount, this._userService.UserId);
                        return ServiceResult<ToDoDto>.Failure("添加待办事项失败，请稍后重试");
                    }
                }
            }
            catch (Exception ex)
            {
                // 输出日志
                this._logger.LogError("[ToDoService] [用户：{Account}（{Id}）] 添加待办事项失败。异常信息：{ex}", this._userService.UserAccount, this._userService.UserId, ex);
                // 转为Dto集合并返回
                return ServiceResult<ToDoDto>.Failure("添加待办事项失败");
            }
        }

        // 修改待办事项
        public async Task<ServiceResult<ToDoDto>> UpdateToDoAsync(UpdateToDoDto updateToDoDto)
        {
            try
            {
                // 判断传入的 updateToDoDto 是否为 NULL，如果是则直接输出日志并返回失败结果
                if (updateToDoDto == null || string.IsNullOrWhiteSpace(updateToDoDto.Title) || string.IsNullOrWhiteSpace((updateToDoDto.Content)) || (updateToDoDto.Status != TodoStatusEnum.Pending && updateToDoDto.Status != TodoStatusEnum.Completed))
                {
                    // 输出日志
                    this._logger.LogWarning("[ToDoService] [用户：{Account}（{Id}）] 修改待办事项失败。待办事项标题或内容为空，或状态不符合规则", this._userService.UserAccount, this._userService.UserId);
                    // 转为Dto集合并返回
                    return ServiceResult<ToDoDto>.Failure("修改待办事项失败：待办事项标题或内容为空，或状态不符合规则");
                }

                // 跨用户修改待办事项失败。判断传入的 updateToDoDto 的 userId 是否跟当前登录的用户一致，否则就提示无权修改他人待办事项
                if (updateToDoDto.UserId != this._userService.UserId)
                {
                    // 输出日志
                    this._logger.LogWarning("[ToDoService] [用户：{Account}（{Id}）] 修改待办事项失败。无法跨用户修改待办事项，用户（ID={currentUserId}）正尝试修改用户（ID={userId}）的待办事项数据", this._userService.UserAccount, this._userService.UserId, this._userService.UserId, updateToDoDto.UserId);
                    // 转为Dto集合并返回
                    return ServiceResult<ToDoDto>.Failure("修改待办事项失败：无权修改他人待办事项");
                }

                // ========== 正常业务流程。输入的内容均不为空且是当前用户的待办事项，则开始修改 ==========
                ToDo? toDo = await this._toDoRepository.FindSingleAsync(t => t.Id == updateToDoDto.Id && t.UserId == updateToDoDto.UserId);
                // 判断是否获取到了，如果数据库中查到了就修改并保存至数据库
                if (toDo != null)
                {
                    // 如果从数据库中查到了待修改的待办事项数据，就修改它，并保存至数据库
                    // 跟踪查到的待办事项实体
                    this._toDoRepository.Update(toDo);
                    // 修改待办事项实体
                    toDo.ToUpdatedToDo(updateToDoDto);
                    // 保存至数据库
                    if (await this._unitOfWork.SaveChangesAsync() > 0)
                    {
                        // 保存至数据库成功
                        // 输出日志
                        this._logger.LogInformation("[ToDoService] [用户：{Account}（{Id}）] 修改待办事项成功。数据：{@ToDo}", this._userService.UserAccount, this._userService.UserId, toDo);
                        // 转为Dto集合并返回
                        return ServiceResult<ToDoDto>.Success("修改待办事项成功", toDo.ToToDoDto());
                    }
                    else
                    {
                        // 保存至数据库失败
                        // 输出日志
                        this._logger.LogWarning("[ToDoService] [用户：{Account}（{Id}）] 修改待办事项失败，保存的数据未生效", this._userService.UserAccount, this._userService.UserId);
                        // 转为Dto集合并返回
                        return ServiceResult<ToDoDto>.Failure("修改待办事项失败，请稍后重试");
                    }
                }
                else
                {
                    // 未找到此实体
                    // 输出日志
                    this._logger.LogWarning("[ToDoService] [用户：{Account}（{Id}）] 修改待办事项失败。未找到需要修改的待办事项实体，需要修改的待办事项：{@updateToDoDto}", this._userService.UserAccount, this._userService.UserId, updateToDoDto);
                    // 转为Dto集合并返回
                    return ServiceResult<ToDoDto>.Failure("修改待办事项失败：未找到此待办事项，请检查或稍后重试");
                }
            }
            catch (Exception ex)
            {
                // 输出日志
                this._logger.LogError("[ToDoService] [用户：{Account}（{Id}）] 修改待办事项失败。异常信息：{ex}", this._userService.UserAccount, this._userService.UserId, ex);
                // 转为Dto集合并返回
                return ServiceResult<ToDoDto>.Failure("修改待办事项时出现异常");
            }
        }

        // 删除待办事项
        public async Task<ServiceResult<bool>> DeleteToDoAsync(DeleteToDoDto deleteToDoDto)
        {
            try
            {
                // 判断传入的 deleteToDoDto 是否为 NULL，如果是则直接输出日志并返回失败结果
                if (deleteToDoDto == null || deleteToDoDto.Id > 0)
                {
                    // 输出日志
                    this._logger.LogWarning("[ToDoService] [用户：{Account}（{Id}）] 删除待办事项失败。需要删除的待办事项数据为空", this._userService.UserAccount, this._userService.UserId);
                    // 转为Dto集合并返回
                    return ServiceResult<bool>.Failure("删除待办事项失败：需要删除的待办事项数据为空");
                }

                // 判断传入的 deleteToDoDto 的 userId 是否跟当前登录的用户一致，否则就提示无权删除他人待办事项
                if (deleteToDoDto.UserId != this._userService.UserId)
                {
                    // 输出日志
                    this._logger.LogWarning("[ToDoService] [用户：{Account}（{Id}）] 删除待办事项失败。无法跨用户删除待办事项，用户（ID={currentUserId}）正尝试修改用户（ID={userId}）的待办事项数据", this._userService.UserAccount, this._userService.UserId, this._userService.UserId, deleteToDoDto.UserId);
                    // 转为Dto集合并返回
                    return ServiceResult<bool>.Failure("删除待办事项失败：无权删除他人待办事项");
                }

                // ========== 正常业务流程。输入的内容均不为空且是当前用户的待办事项，则开始删除 ==========
                // 调用仓储删除服务，判断返回的bool值，true表示找到了且已标记为删除，保存更改即可。反之失败
                if (await this._toDoRepository.DeleteAsync(t => t.Id == deleteToDoDto.Id && t.UserId == deleteToDoDto.UserId && t.Title == deleteToDoDto.Title))
                {
                    if (await this._unitOfWork.SaveChangesAsync() > 0)
                    {
                        // 保存至数据库成功
                        // 输出日志
                        this._logger.LogInformation("[ToDoService] [用户：{Account}（{Id}）] 删除待办事项成功。ID={Id}，标题=\"{Title}\"", this._userService.UserAccount, this._userService.UserId, deleteToDoDto.Id, deleteToDoDto.Title);
                        // 转为Dto集合并返回
                        return ServiceResult<bool>.Success("删除待办事项成功", true);
                    }
                    else
                    {
                        // 保存至数据库失败
                        // 输出日志
                        this._logger.LogWarning("[ToDoService] [用户：{Account}（{Id}）] 删除待办事项失败，删除的数据未生效", this._userService.UserAccount, this._userService.UserId);
                        // 转为Dto集合并返回
                        return ServiceResult<bool>.Failure("修改待办事项失败，请稍后重试");
                    }
                }
                else
                {
                    // 未找到此实体
                    // 输出日志
                    this._logger.LogWarning("[ToDoService] [用户：{Account}（{Id}）] 删除待办事项失败。未找到需要删除的待办事项实体，需要删除的待办事项：ID={Id}，标题=\"{Title}\"", this._userService.UserAccount, this._userService.UserId, deleteToDoDto.Id, deleteToDoDto.Title);
                    // 转为Dto集合并返回
                    return ServiceResult<bool>.Failure("修改待办事项失败：未找到此待办事项，请检查或稍后重试");
                }
            }
            catch(Exception ex)
            {
                // 输出日志
                this._logger.LogError("[ToDoService] [用户：{Account}（{Id}）] 删除待办事项失败。异常信息：{ex}", this._userService.UserAccount, this._userService.UserId, ex);
                // 转为Dto集合并返回
                return ServiceResult<bool>.Failure("删除待办事项时出现异常");
            }
        }
    }
}
