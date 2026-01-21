using System.ComponentModel.DataAnnotations;

namespace MemoDesktop.Dtos.User
{
    /// <summary>
    /// 用户登录请求数据传输对象
    /// 用于接收客户端提交的用户登录凭证
    /// </summary>
    public class LoginUserDto : BaseDto
    {
        /// <summary>
        /// 用户账户名，用于登录识别
        /// </summary>
        private string _account = string.Empty;
        public string Account
        {
            get { return _account; }
            set { _account = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 用户登录密码
        /// </summary>
        private string _password = string.Empty;
        public string Password
        {
            get { return _password; }
            set { _password = value; OnPropertyChanged(); }
        }
    }
}
