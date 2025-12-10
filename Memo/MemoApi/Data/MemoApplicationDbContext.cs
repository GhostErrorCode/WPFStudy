using MemoApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace MemoApi.Data
{
    // 应用程序数据库上下文
    // 负责与 SQLite 数据库进行交互
    public class MemoApplicationDbContext : DbContext
    {
        // 此构造函数让 ASP.NET Core 能通过依赖注入自动配置和创建数据库上下文，无需手动管理配置和连接
        // 允许 ASP.NET Core 依赖注入容器创建 ApplicationDbContext 实例
        // 通过 DbContextOptions 传递数据库配置（连接字符串、提供程序等）
        // 参数DbContextOptions包含数据库配置信息
        // base(options)：调用基类 DbContext 的构造函数，将配置选项传递给基类，让基类完成初始化
        public MemoApplicationDbContext(DbContextOptions<MemoApplicationDbContext> options) : base(options)
        {

        }

        // 数据库表集合属性定义
        // 每个 DbSet<T> 属性对应数据库中的一个表
        // ToDo 待办事项表
        public DbSet<ToDo> ToDo { get; set; }

        // Memo 备忘录表
        public DbSet<Memo> Memo { get; set; }

        // User 用户表
        public DbSet<User> User { get; set; }
    }
}
