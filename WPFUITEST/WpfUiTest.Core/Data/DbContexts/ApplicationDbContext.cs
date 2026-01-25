using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using WpfUiTest.Core.Data.Entities;

namespace WpfUiTest.Core.Data.DbContexts
{
    /// <summary>
    /// 应用程序核心数据库上下文类（EF Core）
    /// 作用：作为WPF应用与Sqlite3数据库之间的核心“桥梁”，负责：
    /// 1. 映射实体类（如ToDo）到数据库表；
    /// 2. 接收数据库连接配置（如Sqlite3连接字符串）；
    /// 3. 提供数据增删查改（CRUD）的操作入口；
    /// 4. 自定义实体与数据库表的映射规则。
    /// 继承自EF Core的DbContext基类：这是EF Core操作数据库的核心基类，封装了数据库连接、事务、查询等底层逻辑。
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// 构造函数（依赖注入模式）
        /// 作用：接收数据库上下文的配置选项（如Sqlite3的连接字符串、数据库提供器等），并传递给父类DbContext
        /// 适配场景：在你的WPF应用中，可通过依赖注入（DI）配置此构造函数，传入Sqlite3的连接配置，实现数据库连接的统一管理
        /// 参数说明：
        ///   options：DbContextOptions<ApplicationDbContext> 是EF Core的强类型配置选项，包含数据库连接字符串、提供器（如Sqlite）等核心配置
        /// </summary>
        /// <param name="options">数据库上下文配置选项（包含Sqlite3连接信息等）</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            // 调用父类DbContext的构造函数，将配置选项传递进去，让EF Core底层使用这些配置连接数据库
            : base(options)
        {
            // 构造函数内可添加初始化逻辑（如自动创建数据库、检查迁移等）
            // 示例：this.Database.EnsureCreated(); // 若数据库不存在则自动创建（适合你的单机Sqlite3场景）
        }

        /// <summary>
        /// 待办事项数据表集合（DbSet）
        /// 作用：EF Core中，DbSet<T> 对应数据库中的一张表，此处ToDos对应Sqlite3中的「Todos表」
        /// 用法：通过ApplicationDbContext.ToDos 可实现对Todos表的增删查改（如Add/Remove/ToList/FirstOrDefault等）
        /// 示例：_dbContext.ToDos.Add(newToDo); // 新增待办事项到数据库
        /// 注意：EF Core会自动将DbSet名称（ToDos）映射为数据库表名（默认小写复数todos，可通过OnModelCreating自定义）
        /// </summary>
        public DbSet<ToDo> ToDos { get; set; }
        // 其他的表集合
        public DbSet<Memo> Memos { get; set; }
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// 模型构建方法（自定义实体↔表的映射规则）
        /// 作用：重写此方法，可自定义实体类与数据库表/字段的映射规则（如指定主键、字段长度、默认值、索引等）
        /// 适配你的场景：针对ToDo实体，可在此配置Status字段默认值、Title字段最大长度、Id为主键等规则
        /// 执行时机：EF Core初始化数据库上下文时自动执行，仅执行一次
        /// </summary>
        /// <param name="modelBuilder">模型构建器：用于配置实体映射规则的核心对象</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 先调用父类的OnModelCreating方法，确保EF Core默认的映射规则生效
            base.OnModelCreating(modelBuilder);

            // 【示例：适配你的ToDo实体的自定义配置（可根据需求添加）】
            // modelBuilder.Entity<ToDo>(entity =>
            // {
            //     // 指定Id为主键（EF Core默认会识别名为Id的字段为主键，此处显式配置更清晰）
            //     entity.HasKey(t => t.Id);
            //     // 配置Title字段最大长度为100，非空（适配你的待办标题业务规则）
            //     entity.Property(t => t.Title).HasMaxLength(100).IsRequired();
            //     // 配置Status字段默认值为0（对应你的待办状态枚举Pending）
            //     entity.Property(t => t.Status).HasDefaultValue(0);
            //     // 配置CreateDate字段默认值为当前时间（Sqlite3语法）
            //     entity.Property(t => t.CreateDate).HasDefaultValueSql("datetime('now')");
            // });
        }
    }
}
