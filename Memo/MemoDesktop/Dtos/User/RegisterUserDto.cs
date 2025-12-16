using System.ComponentModel.DataAnnotations;

namespace MemoDesktop.Dtos.User
{
    /// <summary>
    /// 用户注册请求数据传输对象
    /// 用于接收客户端提交的用户注册信息
    /// </summary>
    public class RegisterUserDto
    {
        /// <summary>
        /// 用户账户名，作为登录标识
        /// 要求：必填，长度3-20，只能包含字母、数字、下划线
        /// </summary>
        [Required(ErrorMessage = "账户名是必填字段，不能为空。")]
        [MinLength(3, ErrorMessage = "账户名长度不能少于3个字符。")]
        [MaxLength(20, ErrorMessage = "账户名长度不能超过20个字符。")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "账户名只能包含英文字母、数字和下划线(_)。")]
        public string Account { get; set; } = string.Empty;

        /// <summary>
        /// 用户显示昵称
        /// 要求：必填，长度2-20
        /// </summary>
        [Required(ErrorMessage = "用户昵称是必填字段，不能为空。")]
        [MinLength(2, ErrorMessage = "用户昵称长度不能少于2个字符。")]
        [MaxLength(20, ErrorMessage = "用户昵称长度不能超过20个字符。")]
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 用户登录密码
        /// 要求：必填，长度至少6位
        /// </summary>
        [Required(ErrorMessage = "密码是必填字段，不能为空。")]
        [MinLength(6, ErrorMessage = "密码长度不能少于6个字符。")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// 确认密码，必须与Password字段值完全一致
        /// </summary>
        [Required(ErrorMessage = "确认密码是必填字段，不能为空。")]
        [Compare(nameof(Password), ErrorMessage = "两次输入的密码不一致，请重新输入。")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;

        // 注意：不包含 Id、CreateDate、UpdateDate 字段
        // 因为这些字段应由服务器或数据库生成，不应由客户端提供
    }
}
