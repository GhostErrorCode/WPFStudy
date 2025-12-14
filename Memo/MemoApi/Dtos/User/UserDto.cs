namespace MemoApi.Dtos.User
{
    /// <summary>
    /// 用户公开信息数据传输对象
    /// 用于向客户端返回用户非敏感信息，不包含密码等私密数据
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// 用户唯一标识符
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 用户账户名
        /// </summary>
        public string Account { get; set; } = string.Empty;

        /// <summary>
        /// 用户显示昵称
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 账户创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 账户最后更新时间
        /// </summary>
        public DateTime UpdateDate { get; set; }
    }
}
