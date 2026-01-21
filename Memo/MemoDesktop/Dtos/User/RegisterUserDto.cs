using System.ComponentModel.DataAnnotations;

namespace MemoDesktop.Dtos.User
{
    /// <summary>
    /// 用户注册请求数据传输对象
    /// 用于接收客户端提交的用户注册信息
    /// </summary>
    public class RegisterUserDto : BaseDto
    {
        /// <summary>
        /// 用户账户名，作为登录标识
        /// 要求：必填，长度3-20，只能包含字母、数字、下划线
        /// </summary>
        private string _account = string.Empty;
        public string Account
        {
            get { return _account; }
            set { _account = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 用户显示昵称
        /// 要求：必填，长度2-20
        /// </summary>
        private string _userName = string.Empty;
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 用户登录密码
        /// 要求：必填，长度至少6位
        /// </summary>
        private string _password = string.Empty;
        public string Password
        {
            get { return _password; }
            set { _password = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 确认密码，必须与Password字段值完全一致
        /// </summary>
        private string _confirmPassword = string.Empty;
        public string ConfirmPassword
        {
            get { return _confirmPassword; }
            set { _confirmPassword = value; OnPropertyChanged(); }
        }

        // 注意：不包含 Id、CreateDate、UpdateDate 字段
        // 因为这些字段应由服务器或数据库生成，不应由客户端提供
    }
}
