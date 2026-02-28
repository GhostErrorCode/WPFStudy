using System;
using System.Collections.Generic;
using System.Text;
using WpfUiTest.Core.DTOs.User;
using WpfUiTest.Shared.Utilities;

namespace WpfUiTest.Core.Services.Interfaces
{
    // 用户服务接口
    public interface IUserService
    {
        // 当前登录用户的昵称
        public UserResultDto? CurrentUser { get; }
        // 当前是否有用户登录
        public bool IsLoggedIn { get; }
        // 当前登录用户的ID
        public int UserId { get; }
        // 当前登录用户的账户
        public string UserAccount { get; }

        // 用户登录方法
        public Task<ServiceResult<bool>> LoginAsync(LoginUserDto loginUserDto);
        // 用户自动登录方法
        public Task<ServiceResult<bool>> AutoLoginAsync(int userId, string account);
        // 用户注册方法
        public Task<ServiceResult<bool>> RegisterAsync(RegisterUserDto registerUserDto);
        // 用户登出方法
        public void Logout();
    }
}
