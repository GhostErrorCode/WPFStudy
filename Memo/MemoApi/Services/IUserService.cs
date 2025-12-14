
using MemoApi.Dtos.User;
using MemoApi.Results;

namespace MemoApi.Services
{
    public interface IUserService
    {
        // ========== 登录注册服务层接口 ==========

        // 注册
        public Task<ServiceResult<UserDto>> RegisterUserAsync(RegisterUserDto registerUserDto);

        // 登录
        public Task<ServiceResult<UserDto>> LoginUserAsync(LoginUserDto loginUserDto);
    }
}
