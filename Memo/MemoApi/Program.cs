
using MemoApi.Data;
using MemoApi.Repositories;
using MemoApi.Repositories.Implements;
using MemoApi.Services;
using MemoApi.Services.Implements;
using MemoApi.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using System;
using System.Runtime.CompilerServices;

namespace MemoApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 添加服务到容器
            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            // 配置Swagger服务
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v2025", new Microsoft.OpenApi.OpenApiInfo
                {
                    Title = "MemoAPI",
                    Version = "v2025",
                    Description = "MemoAPI",
                    Contact = new Microsoft.OpenApi.OpenApiContact
                    {
                        Name = "个人开发项目",
                        Email = "xxxxxxx@xxx.com"
                    }
                });
            });
            // ========================== 动态构建数据库 =========================
            // 1. 从配置中读取路径模板
            //    从"Database:PathTemplate"配置键读取值，若为空则使用默认值"app.db"
            string pathTemplate = builder.Configuration["Database:PathTemplate"] ?? "Memo.db";

            // 2. 动态解析为最终绝对路径
            //    调用辅助方法，将配置中的路径模板解析为实际可用的文件系统路径
            string resolvedDbPath = ResolveDatabasePath(pathTemplate);

            // 3. 配置数据库上下文 (DbContext)
            //    将ApplicationDbContext注册到依赖注入容器，使用SQLite数据库提供程序
            //    并将解析得到的完整路径作为连接字符串的数据源(Data Source)
            // 1. 注册数据库上下文（Scoped生命周期）
            // 重要：DbContext必须是Scoped，确保一次HTTP请求内共享同一个实例
            builder.Services.AddDbContext<MemoApplicationDbContext>(options =>
            {
                // 获取连接字符串
                string connectionString = $"Data Source={resolvedDbPath}";

                // 配置SQL Server（根据你的数据库类型调整）
                options.UseSqlite(connectionString);

                // 开发环境启用详细日志
                if (builder.Environment.IsDevelopment())
                {
                    options.EnableSensitiveDataLogging(); // 记录SQL参数值
                    options.EnableDetailedErrors();       // 详细错误信息
                }
            });

            // 为什么是Scoped？确保一次HTTP请求内的所有操作共享同一个工作单元
            // 注册通用仓储和工作单元
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork.Implements.UnitOfWork>();

            // 3. 注册具体仓储（Scoped生命周期）
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IToDoRepository, ToDoRepository>();
            builder.Services.AddScoped<IMemoRepository, MemoRepository>();

            // 4. 注册业务服务（Scoped生命周期）
            builder.Services.AddScoped<IToDoService, ToDoService>();

            // 5. 注册API控制器
            builder.Services.AddControllers();
            // ===================================================================

            var app = builder.Build();
            // ==========================创建数据库=============================
            // 1. 创建一个明确的作用域 (Scope)
            // `app.Services` 的属性类型是 `System.IServiceProvider`。
            // `CreateScope()` 方法的返回类型是 `System.IServiceScope`。
            // `using` 语句确保该作用域在代码块结束时被妥善释放。
            using (Microsoft.Extensions.DependencyInjection.IServiceScope scope = app.Services.CreateScope())
            {
                // 2. 从作用域中获取服务提供者
                // `scope.ServiceProvider` 的属性类型同样是 `System.IServiceProvider`。
                // 这个 `serviceProvider` 是与当前 `scope` 绑定的。
                System.IServiceProvider serviceProvider = scope.ServiceProvider;

                // 3. 从服务提供者中解析（获取）数据库上下文实例
                // `GetRequiredService<T>()` 是一个泛型方法，此处 `T` 为 `ApplicationDbContext`。
                // 它的作用是：从依赖注入容器中获取 `ApplicationDbContext` 类型的服务。
                // 如果容器中未注册该服务，此方法将抛出 `System.InvalidOperationException` 异常。
                // 如果注册成功，则返回一个 `ApplicationDbContext` 的实例。
                MemoApplicationDbContext dbContext = serviceProvider.GetRequiredService<MemoApplicationDbContext>();

                // 4. 执行数据库迁移
                // `dbContext.Database` 属性的类型是 `Microsoft.EntityFrameworkCore.Infrastructure.DatabaseFacade`。
                // `Migrate()` 是 `DatabaseFacade` 类的一个方法，返回类型为 `void`。
                // 该方法会检查数据库状态，并自动应用所有未执行的迁移文件。
                dbContext.Database.Migrate();

                // （可选）5. 调用种子数据初始化方法
                // `SeedData` 是一个假设存在的静态工具类。
                // `Initialize` 是一个假设存在的静态方法，它接受一个 `ApplicationDbContext` 参数。
                // SeedData.Initialize(dbContext);
            }
            // ========================================================================================

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            // 配置HTTP请求管道
            if (app.Environment.IsDevelopment())
            {
                // 启用Swagger中间件
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v2025/swagger.json", "MemoAPI");
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }



        // =================== 动态构建数据库目录 ===================
        // --- 辅助方法：核心解析逻辑 ---
        // 定义为静态方法，便于在Program类的上下文中调用且不依赖实例状态
        private static string ResolveDatabasePath(string template)
        {
            // 检查传入的路径模板是否已经是绝对路径（如C:\Data\app.db 或 /var/data/app.db）
            // Path.IsPathRooted可跨平台工作，如果是绝对路径则直接返回，无需进一步处理
            if (Path.IsPathRooted(template)) return template;

            // 获取应用程序的当前工作目录
            // 在开发时通常是项目的输出目录（如bin\Debug\net8.0）
            string baseDir = Directory.GetCurrentDirectory();

            // 将当前工作目录与相对路径模板组合，并解析为完整的绝对路径
            // Path.Combine能正确处理不同操作系统的路径分隔符
            // Path.GetFullPath会解析相对路径符号（如..表示上级目录）
            string fullPath = Path.GetFullPath(Path.Combine(baseDir, template));

            // 获取目标路径所在的目录部分（例如从"C:\Project\app.db"得到"C:\Project"）
            string? directory = Path.GetDirectoryName(fullPath);

            // 如果目录路径不为空且该目录不存在，则创建它
            // 这是必要的，因为SQLite可以创建数据库文件，但不会自动创建不存在的目录
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // 在控制台输出最终路径，便于调试和确认
            // 在实际部署到生产环境时，建议改用日志系统（如ILogger）
            Console.WriteLine($"数据库文件将保存至：{fullPath}");

            // 返回最终的、确定存在的绝对路径
            return fullPath;
        }
    }
}
