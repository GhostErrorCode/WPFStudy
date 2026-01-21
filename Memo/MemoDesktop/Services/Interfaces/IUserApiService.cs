using MemoDesktop.ApiResponses;
using MemoDesktop.Dtos.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace MemoDesktop.Services.Interfaces
{
    // 用户API接口
    public interface IUserApiService
    {
        // 用户登录
        public Task<ApiResponse<UserDto>> LoginUserAsync(LoginUserDto loginUserDto);

        // 用户注册
        public Task<ApiResponse<UserDto>> RegisterUserAsync(RegisterUserDto registerUserDto);
    }
}
