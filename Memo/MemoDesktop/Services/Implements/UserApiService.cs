using MemoDesktop.ApiResponses;
using MemoDesktop.Dtos.ToDo;
using MemoDesktop.Dtos.User;
using MemoDesktop.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace MemoDesktop.Services.Implements
{
    // 用户API服务实现
    public class UserApiService : BaseApiService, IUserApiService
    {
        // API基础路径
        private const string BasePath = "Api/User";

        // 构造函数
        public UserApiService(HttpClient httpClient) : base(httpClient)
        {
            // 调用基类构造函数
        }

        // 用户登录方法
        public async Task<ApiResponse<UserDto>> LoginUserAsync(LoginUserDto loginUserDto)
        {
            // 构建请求URL
            string requestUrl = $"{BasePath}/LoginUser";
            // 发送Post请求
            ApiResponse<UserDto> apiResponse = await this.PostAsync<UserDto, LoginUserDto>(requestUrl, loginUserDto);
            return apiResponse;
        }

        // 用户注册方法
        public async Task<ApiResponse<UserDto>> RegisterUserAsync(RegisterUserDto registerUserDto)
        {
            // 构建请求URL
            string requestUrl = $"{BasePath}/RegisterUser";
            // 发送Post请求
            ApiResponse<UserDto> apiResponse = await this.PostAsync<UserDto, RegisterUserDto>(requestUrl, registerUserDto);
            return apiResponse;
        }
    }
}
