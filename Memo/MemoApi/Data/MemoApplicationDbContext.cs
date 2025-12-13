// 引入命名空间：MemoApi.Entities
// 作用：引用我们定义的实体类
using MemoApi.Entities;
// 引入命名空间：Microsoft.EntityFrameworkCore
// 作用：提供Entity Framework Core的核心功能
using Microsoft.EntityFrameworkCore;

namespace MemoApi.Data
{
    // 数据库上下文类
    // 作用：Entity Framework Core的核心类，是代码与数据库之间的桥梁
    // 继承关系：继承自DbContext，这是EF Core的基类
    // 核心职责：
    //   1. 定义实体与数据库表的映射关系
    //   2. 管理数据库连接
    //   3. 跟踪实体状态变化
    //   4. 将LINQ查询转换为SQL语句
    //   5. 执行数据库操作
    // 设计原则：一个DbContext代表一个数据库"工作单元"，通常对应一个数据库
    public class MemoApplicationDbContext : DbContext
    {
        // 此构造函数让 ASP.NET Core 能通过依赖注入自动配置和创建数据库上下文，无需手动管理配置和连接
        // 允许 ASP.NET Core 依赖注入容器创建 ApplicationDbContext 实例
        // 通过 DbContextOptions 传递数据库配置（连接字符串、提供程序等）
        // 参数DbContextOptions包含数据库配置信息
        // base(options)：调用基类 DbContext 的构造函数，将配置选项传递给基类，让基类完成初始化

        // 构造函数
        // 作用：初始化数据库上下文，接收配置选项
        // 参数：
        //   - options: DbContextOptions<MemoApplicationDbContext> - 泛型配置选项
        //     泛型参数确保类型安全，MemoApplicationDbContext
        // 设计模式：依赖注入，通过构造函数接收外部配置
        // 调用基类构造函数：base(options)将选项传递给基类DbContext
        public MemoApplicationDbContext(DbContextOptions<MemoApplicationDbContext> options) : base(options)
        {
            // 可以在这里添加DbContext的初始化逻辑
            // 例如：配置实体跟踪行为
            // ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        // 属性：待办事项DbSet
        // 作用：表示数据库中的 ToDo 表，用于对产品数据进行操作
        // 数据类型：DbSet<ToDo> - EF Core的泛型集合类型
        // 命名约定：属性名通常对应数据库表名（复数形式）
        // 访问修饰符：public，允许仓储类访问
        // 自动实现属性：编译器自动生成私有字段和getter/setter
        // DbSet功能：
        //   1. 执行LINQ查询：context.ToDo.Where(p => p.Price > 100)
        //   2. 添加实体：context.ToDo.Add(product)
        //   3. 更新实体：context.ToDo.Update(product)
        //   4. 删除实体：context.ToDo.Remove(product)

        // ToDo 待办事项表
        public DbSet<ToDo> ToDo { get; set; }

        // Memo 备忘录表
        public DbSet<Memo> Memo { get; set; }

        // User 用户表
        public DbSet<User> User { get; set; }

        // 可以继续添加其他实体的DbSet属性
        // 每个实体类对应一个DbSet属性，每个DbSet对应数据库中的一个表
        // 示例：
        // public DbSet<Order> Orders { get; set; }
        // public DbSet<User> Users { get; set; }
        // public DbSet<AuditLog> AuditLogs { get; set; }



        // 重写方法：配置数据模型
        // 作用：配置实体与数据库表的映射关系，设置约束、索引、关系等
        // 参数：
        //   - modelBuilder: ModelBuilder - 模型构建器，用于配置数据模型
        // 调用时机：EF Core第一次使用DbContext时调用，通常只执行一次
        // 设计模式：Fluent API配置，比Data Annotations（特性标签）更灵活强大
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 调用基类实现：确保基类的配置也被应用
            base.OnModelCreating(modelBuilder);

            // 配置产品与分类的一对多关系
            // 作用：定义Product和Category实体之间的关联关系
            // 关系类型：一对多（一个分类有多个产品，一个产品属于一个分类）
            //modelBuilder.Entity<Product>()
            //    // HasOne：配置"一"的一方，产品有一个分类
            //    .HasOne(p => p.Category)
            //    // WithMany：配置"多"的一方，分类有多个产品
            //    .WithMany(c => c.Products)
            //    // HasForeignKey：指定外键属性，Product表中的CategoryId字段
            //    .HasForeignKey(p => p.CategoryId)
            //    // OnDelete：配置删除行为，Restrict表示限制删除
            //    // 行为选项：
            //    //   - Cascade：级联删除（删除分类时自动删除所有产品） - 危险！
            //    //   - Restrict：限制删除（有产品时不能删除分类） - 推荐
            //    //   - SetNull：设置为null（删除分类时将产品CategoryId设为null）
            //    //   - ClientSetNull：客户端设置为null
            //    //   - NoAction：无操作（依赖数据库约束）
            //    .OnDelete(DeleteBehavior.Restrict);

            //// 注意：由于没有BaseEntity基类，无法设置全局查询过滤器
            //// 全局查询过滤器通常用于软删除：.HasQueryFilter(e => !e.IsDeleted)
            //// 在我们的设计中，需要在每个查询中手动添加过滤条件

            //// 为产品名称添加数据库索引
            //// 作用：提高按产品名称查询的性能
            //// 索引类型：非聚集索引（默认），非唯一索引（允许重复名称）
            //modelBuilder.Entity<Product>()
            //    .HasIndex(p => p.Name)
            //    // 可以配置索引选项：
            //    // .IsUnique() - 创建唯一索引
            //    // .HasFilter() - 添加过滤条件
            //    // .HasName("IX_Products_Name") - 指定索引名称
            //    ;

            //// 为产品价格添加数据库索引
            //// 作用：提高按价格范围查询的性能（如WHERE Price BETWEEN 100 AND 500）
            //// 适用场景：经常需要按价格排序或范围查询
            //modelBuilder.Entity<Product>()
            //    .HasIndex(p => p.Price);

            //// 为分类名称添加唯一索引
            //// 作用：确保分类名称不重复，同时提高按名称查询的性能
            //modelBuilder.Entity<Category>()
            //    .HasIndex(c => c.Name)
            //    .IsUnique(); // 唯一索引，分类名称不能重复

            // 可以添加更多配置：
            // 1. 配置字符串字段长度：.Property(p => p.Name).HasMaxLength(100)
            // 2. 配置必需字段：.Property(p => p.Name).IsRequired()
            // 3. 配置默认值：.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()")
            // 4. 配置计算列：.Property(p => p.TotalValue).HasComputedColumnSql("[Price] * [StockQuantity]")
            // 5. 配置表名：.ToTable("ProductTable")
            // 6. 配置架构：.ToTable("Products", schema: "catalog")
        }
    }
}
