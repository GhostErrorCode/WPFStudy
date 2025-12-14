using MemoApi.Dtos.ToDo;
using MemoApi.Dtos.User;
using MemoApi.Results;
using MemoApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MemoApi.Controllers
{
    // 用户账户控制器
    // 路径: Api/User/...

    [ApiController]
    [Route("Api/User")]
    public class UserController : ApiControllerBase
    {
        // 依赖注入服务层对象
        private readonly IUserService _userService;

        // 构造函数注入
        public UserController(IUserService userService)
        {
            this._userService = userService;
        }

        // 用户登录
        [HttpPost("LoginUser")]
        public async Task<ActionResult<UserDto>> LoginUser(LoginUserDto loginUserDto)
        {
            // 调用服务层进行登录并接收返回值
            ServiceResult<UserDto> loginUserResult = await this._userService.LoginUserAsync(loginUserDto);
            return HandleServiceResult<UserDto>(loginUserResult);
        }

        // 用户注册
        [HttpPost("RegisterUser")]
        public async Task<ActionResult<UserDto>> RegisterUser(RegisterUserDto registerUserDto)
        {
            // 调用服务层进行注册并接收返回值
            ServiceResult<UserDto> registerUserResult = await this._userService.RegisterUserAsync(registerUserDto);
            return HandleServiceResult<UserDto>(registerUserResult);
        }
    }
}
