namespace MemoApi.Entities
{
    // 用户实体类
    public class User
    {
        // 用户ID
        public int Id { get; set; }

        // 用户账户
        public string? Account {  get; set; }

        // 用户昵称
        public string? UserName { get; set; }

        // 用户密码
        public string? Password { get; set; }

        // 用户创建时间
        public DateTime CreateDate { get; set; }

        // 用户修改时间
        public DateTime UpdateDate { get; set; }
    }
}
