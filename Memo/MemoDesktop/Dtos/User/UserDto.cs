namespace MemoDesktop.Dtos.User
{
    /// <summary>
    /// 用户公开信息数据传输对象
    /// 用于向客户端返回用户非敏感信息，不包含密码等私密数据
    /// </summary>
    public class UserDto : BaseDto
    {
        /// <summary>
        /// 用户唯一标识符
        /// </summary>
        private int _id;
        public int Id
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 用户账户名
        /// </summary>
        private string _account = string.Empty;
        public string Account
        {
            get { return _account; }
            set { _account = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 用户显示昵称
        /// </summary>
        private string _userName = string.Empty;
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 账户创建时间
        /// </summary>
        private DateTime _createDate;
        public DateTime CreateDate
        {
            get { return _createDate; }
            set { _createDate = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 账户最后更新时间
        /// </summary>
        private DateTime _updateDate;
        public DateTime UpdateDate
        {
            get { return _updateDate; }
            set { _updateDate = value; OnPropertyChanged(); }
        }
    }
}
