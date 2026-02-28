using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using WpfUiTest.Core.Data.Entities;
using WpfUiTest.Core.Data.Repositories.Interfaces;
using WpfUiTest.Core.Data.UnitOfWork.Interfaces;
using WpfUiTest.Core.DTOs.User;
using WpfUiTest.Core.Mapping;
using WpfUiTest.Core.Services.Interfaces;
using WpfUiTest.Shared.Models;
using WpfUiTest.Shared.Utilities;

namespace WpfUiTest.Core.Services.Implements
{
    // 用户服务层
    public class UserService : IUserService
    {
        // 属性：当前登录的用户
        public UserResultDto? CurrentUser { get => this._currentUser; }
        // 属性：当前是否有用户登录
        public bool IsLoggedIn { get => this.CurrentUser != null; }
        // 属性：当前登录用户的ID
        public int UserId { get => this.CurrentUser?.Id ?? 0; }
        // 属性：当前登录用户的账户
        public string UserAccount { get => this.CurrentUser?.Account ?? string.Empty; }

        // 字段：用户返回Dto
        private UserResultDto? _currentUser;
        // 字段：用户仓储
        private readonly IUserRepository _userRepository;
        // 字段：工作单元
        private readonly IUnitOfWork _unitOfWork;
        // 字段：日志管理
        private readonly ILogger<UserService> _logger;


        // 构造函数(依赖注入)
        public UserService(ILogger<UserService> logger, IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            // 依赖注入解析
            this._logger = logger;
            this._userRepository = userRepository;
            this._unitOfWork = unitOfWork;

            // 输出日志
            // this._logger.LogInformation("UserService: 已成功解析所有依赖");
        }


        // 用户登录方法
        public async Task<ServiceResult<bool>> LoginAsync(LoginUserDto loginUserDto)
        {
            try
            {
                // 登录失败：用户账户或密码为空
                if (loginUserDto == null || string.IsNullOrWhiteSpace(loginUserDto.Account) || string.IsNullOrWhiteSpace(loginUserDto.Password))
                {
                    // 输出日志
                    this._logger.LogWarning("登录失败: 用户账户与密码不能为空!");
                    // 返回失败的服务结果
                    return ServiceResult<bool>.Failure("用户账户与密码不能为空!");
                }

                // 登录失败：用户找不到或密码验证不通过
                // 先从数据库中获取用户账户
                User? currentLoginUser = await this._userRepository.GetByAccountAsync(loginUserDto.Account);
                // 如果数据库没找到这个用户账户
                if (currentLoginUser == null)
{
                    this._logger.LogWarning("登录失败: 用户 {Account} 不存在!", loginUserDto.Account);
                    return ServiceResult<bool>.Failure("用户账户或密码错误!");
                }

                // 用户存在，但密码验证失败 -> 失败
                if (!PasswordHasher.Verify(loginUserDto.Password, currentLoginUser.Password))
                {
                    this._logger.LogWarning("登录失败: 用户 {Account} 密码错误!", currentLoginUser.Account);
                    return ServiceResult<bool>.Failure("用户账户或密码错误!");
                }

                // 登录成功：保存当前登录的用户以及是否生成本地令牌
                // 保存当前用户
                this._currentUser = currentLoginUser.ToUserResultDto();
                // 检查是否勾选了记住我，以便生成登录凭证
                if(loginUserDto.IsRememberMe == true)
                {
                    try
                    {
                        // 调用工具集保存登录凭证
                        await LoginCredentialHelper.SaveLoginCredential(new LoginCredential()
                        {
                            UserId = this._currentUser.Id,
                            Account = this._currentUser.Account
                        });
                    }
                    catch(Exception ex)
                    {
                        // 记录报错登录凭证的异常日志
                        this._logger.LogError("保存登录凭证失败: 异常信息: {ex}", ex);
                    }
                }
                // 输出登录成功日志
                this._logger.LogInformation("登录成功: 用户 {Account} 登录成功!", currentLoginUser.Account);
                return ServiceResult<bool>.Success("登录成功", true);
            }
            catch(Exception ex)
            {
                // 记录异常日志
                this._logger.LogError("登录失败: 用户登录时出现系统内部异常! 异常信息: {ex}", ex);
                // 登录失败：系统内部异常
                return ServiceResult<bool>.Failure("系统内部异常,请稍后重试!");
            }
        }

        // 用户自动登录方法
        public async Task<ServiceResult<bool>> AutoLoginAsync(int userId, string account)
        {
            try
            {
                // 登录失败：用户账户或密码为空
                if (!(userId > 0) || string.IsNullOrWhiteSpace(account))
                {
                    // 输出日志
                    this._logger.LogWarning("自动登录失败: 未找到对应用户!");
                    // 返回失败的服务结果
                    return ServiceResult<bool>.Failure("未找到对应用户!");
                }
                // 1.开始从数据库中尝试查找并判断
                User? user = await this._userRepository.GetByIdAsync(userId);
                if(user != null && user.Account == account)
                {
                    // 保存当前用户
                    this._currentUser = user.ToUserResultDto();
                    // 输出登录成功日志
                    this._logger.LogInformation("自动登录成功: 用户 {Account} 登录成功", user.Account);
                    return ServiceResult<bool>.Success("自动登录成功", true);
                }
                else
                {
                    // 输出日志
                    this._logger.LogWarning("自动登录失败: 未找到对应用户!");
                    // 返回失败的服务结果
                    return ServiceResult<bool>.Failure("未找到对应用户!");
                }
            }
            catch (Exception ex)
            {
                // 记录异常日志
                this._logger.LogError("自动登录失败: 用户自动登录时出现系统内部异常! 异常信息: {ex}", ex);
                // 登录失败：系统内部异常
                return ServiceResult<bool>.Failure("系统内部异常,请稍后重试!");
            }
        }

        // 用户注册方法
        public async Task<ServiceResult<bool>> RegisterAsync(RegisterUserDto registerUserDto)
        {
            try
            {
                // 1.输入的数据不完整 - 注册失败
                if (registerUserDto == null ||
                    string.IsNullOrWhiteSpace(registerUserDto.Account) ||
                    string.IsNullOrWhiteSpace(registerUserDto.UserName) ||
                    string.IsNullOrWhiteSpace(registerUserDto.Password) ||
                    string.IsNullOrWhiteSpace(registerUserDto.ConfirmPassword))
                {
                    this._logger.LogWarning("注册失败: 提交的数据不完整或为空");
                    return ServiceResult<bool>.Failure("注册信息不能为空");
                }
                // 2.验证两次输入的密码是否一致
                if (registerUserDto.Password != registerUserDto.ConfirmPassword)
                {
                    this._logger.LogWarning("注册失败: 用户 {Account} 输入的两次密码不一致!", registerUserDto.Account);
                    return ServiceResult<bool>.Failure("输入的两次密码不一致");
                }
                // 3.验证密码复杂度（规则）
                if (registerUserDto.Password.Length < 6)
                {
                    this._logger.LogWarning("注册失败: 用户 {Account} 密码长度至少为6位!", registerUserDto.Account);
                    return ServiceResult<bool>.Failure("密码长度至少为6位");
                }
                // 4.检查用户账户是否已存在
                if (await this._userRepository.ExistsAsync((User user) => user.Account == registerUserDto.Account))
                {
                    this._logger.LogWarning("注册失败: 用户 {Account} 已存在!", registerUserDto.Account);
                    return ServiceResult<bool>.Failure("该账户已被注册");
                }


                // 5.都没问题后，开始注册，开始显示事务
                await this._unitOfWork.BeginTransactionAsync();
                try
                {
                    // 6.创建用户实体并哈希密码，保存到数据库
                    User registerUser = await this._userRepository.AddAsync(registerUserDto.ToUser());
                    // 7.显示提交事务
                    await this._unitOfWork.CommitTransactionAsync();
                    _logger.LogInformation("注册成功: 用户 {Account}, ID: {Id}", registerUser.Account, registerUser.Id);
                    return ServiceResult<bool>.Success("注册成功", true);
                }
                catch (Exception ex)
                {
                    // 8. 事务回滚
                    await this._unitOfWork.RollbackTransactionAsync();
                    throw; // 抛出异常让外层catch块处理
                }
            }
            catch(Exception ex)
            {
                this._logger.LogError("注册失败: 用户注册时出现系统内部异常! 异常信息: {ex}", ex);
                return ServiceResult<bool>.Failure("注册失败: 系统内部错误，请稍后重试");
            }
        }
        // 用户登出方法
        public void Logout()
        {
            // 1.安全检查 - 检查是否已登录
            if(this.IsLoggedIn == false) { this._logger.LogWarning("登出失败: 当前无用户登录!"); return; }
            // 2.登出的日志记录
            this._logger.LogInformation("登出成功: 用户 {Account} 已登出...", this.UserAccount);
            // 3.清理登录状态
            this._currentUser = null;
            // 4.清理自动登录缓存
            LoginCredentialHelper.DeleteLoginCredential();
        }
    }
}
