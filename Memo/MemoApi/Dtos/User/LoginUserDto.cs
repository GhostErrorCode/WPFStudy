using System.ComponentModel.DataAnnotations;

namespace MemoApi.Dtos.User
{
    /// <summary>
    /// 用户登录请求数据传输对象
    /// 用于接收客户端提交的用户登录凭证
    /// </summary>
    public class LoginUserDto
    {
        /// <summary>
        /// 用户账户名，用于登录识别
        /// </summary>
        [Required(ErrorMessage = "账户名是必填字段，不能为空。")]
        public string Account { get; set; } = string.Empty;

        /// <summary>
        /// 用户登录密码
        /// </summary>
        [Required(ErrorMessage = "密码是必填字段，不能为空。")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
