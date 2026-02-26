using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System.CodeDom;
using System.Configuration;
using System.Data;
using System.IO;
using System.Text;
using System.Windows;
using Wpf.Ui;
using Wpf.Ui.DependencyInjection;
using WpfUiTest.App.ViewModels.Main;
using WpfUiTest.App.ViewModels.User;
using WpfUiTest.App.Views;
using WpfUiTest.App.Views.Main;
using WpfUiTest.App.Views.User;
using WpfUiTest.Core.Data.DbContexts;
using WpfUiTest.Core.Data.Repositories.Implements;
using WpfUiTest.Core.Data.Repositories.Interfaces;
using WpfUiTest.Core.Data.UnitOfWork.Implements;
using WpfUiTest.Core.Data.UnitOfWork.Interfaces;
using WpfUiTest.Core.Services.Implements;
using WpfUiTest.Core.Services.Interfaces;
using WpfUiTest.Shared.Models;
using WpfUiTest.Shared.Utilities;

namespace WpfUiTest.App
{
    /// <summary>
    /// 应用程序主类，负责配置Host、DI容器、日志系统和启动流程
    /// </summary>
    public partial class App : Application
    {
        // 字段：获取应用设置
        private readonly AppConfiguration _appConfiguration = AppConfigurationHelper.LoadMergeSettings();

        /// <summary>
        /// .NET通用主机实例，管理应用程序生命周期和服务依赖注入
        /// 定义为属性：封装访问逻辑、符合编码规范，便于后续扩展（如非空校验）
        /// </summary>
        private IHost ApplicationHost { get; set; }

        /// <summary>
        /// App类构造函数，初始化Host配置和依赖注入容器
        /// </summary>
        public App()
        {
            // 获取命令行参数：Skip(1)跳过第一个元素（程序自身完整路径），仅保留用户传入的有效参数
            string[] args = Environment.GetCommandLineArgs().Skip(1).ToArray();

            // 创建并配置.NET通用主机
            // Host.CreateDefaultBuilder(args)：创建.NET默认主机构建器，自动加载基础配置（命令行参数、环境变量等）
            // args：传入处理后的命令行参数（已跳过程序路径，仅保留用户参数），主机可解析这些参数覆盖配置
            this.ApplicationHost = Host.CreateDefaultBuilder(args)
                .ConfigureLogging((context, logging) =>
                {
                    // 1. 清除.NET默认的所有日志提供程序（如Console、Debug、EventLog等）
                    // 目的：避免Serilog和默认日志提供程序重复输出日志，导致日志冗余
                    logging.ClearProviders();

                    // 2. 构建日志路径：EXE运行目录 → Logs文件夹（核心修改：精准指向EXE所在目录）
                    // AppContext.BaseDirectory：当前运行的EXE程序所在的文件夹路径（如：D:\WPFUITEST\bin\Debug\net10-windows\）
                    string exeRunDir = AppContext.BaseDirectory;
                    // 拼接EXE目录下的Logs文件夹路径（最终：EXE目录\Logs）
                    string logDirectory = Path.Combine(exeRunDir, this._appConfiguration.LogSettings.Directory);
                    // 确保Logs文件夹存在，不存在则自动创建（避免写入日志时路径不存在报错）
                    Directory.CreateDirectory(logDirectory);
                    // 日志文件路径：EXE目录\Logs\log-.txt（Serilog会自动拼接日期，如log-2026-01-23.txt）
                    string logFilePath = Path.Combine(logDirectory, "Log-.txt");

                    try
                    {
                        // 3. 配置Serilog核心参数，构建全局日志实例Log.Logger
                        // 包裹try-catch：日志系统是核心组件，初始化失败时可降级提示并终止应用
                        Log.Logger = new LoggerConfiguration()
                            // 从应用配置（appsettings.json）读取Serilog配置（优先级低于代码硬编码）
                            // 作用：可通过配置文件动态调整日志级别，无需改代码
                            // 移除：不再从Host配置读取Serilog，改用自定义配置
                            // .ReadFrom.Configuration(context.Configuration)

                            // 设置全局日志最小级别为Debug（会输出Debug及以上级别日志：Debug/Info/Warn/Error/Fatal）
                            // .MinimumLevel.Information()
                            .MinimumLevel.Is(ParseLogLevel(this._appConfiguration.LogSettings.LogLevel.Default))
                            // 覆盖「Microsoft命名空间」下组件的日志级别为Information
                            // 作用：过滤微软组件的Debug级冗余日志（如EF Core的SQL执行日志）
                            // .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                            .MinimumLevel.Override("Microsoft", ParseLogLevel(this._appConfiguration.LogSettings.LogLevel.Microsoft))
                            // 覆盖「System命名空间」下组件的日志级别为Warning
                            // 作用：仅输出系统组件的警告/错误日志，进一步减少冗余
                            // .MinimumLevel.Override("System", LogEventLevel.Warning)
                            .MinimumLevel.Override("System", ParseLogLevel(this._appConfiguration.LogSettings.LogLevel.System))
                            // 从日志上下文（LogContext）中添加丰富信息（如当前用户、请求ID等）
                            // 扩展：后续可通过LogContext.PushProperty("UserId", 123)添加自定义上下文
                            .Enrich.FromLogContext()
                            // 配置日志输出到控制台（开发调试用）
                            .WriteTo.Console(
                                // 控制台日志输出模板：[时分秒 日志级别] 日志内容 + 异常（如有）
                                // {Timestamp:HH:mm:ss}：仅显示时分秒（控制台简洁）；{Level:u3}：3位大写级别（如DBG/INF）
                                // {Message:lj}：保留换行的日志内容；{NewLine}{Exception}：异常信息换行显示
                                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
                                // 控制台仅输出Debug及以上级别日志（与全局级别一致）
                                restrictedToMinimumLevel: LogEventLevel.Debug)
                            // 配置日志输出到本地文件（生产环境持久化日志）
                            .WriteTo.File(
                                path: logFilePath, // 日志文件路径（log-.txt会自动拼接日期，如log-2026-01-23.txt）
                                rollingInterval: RollingInterval.Day, // 按天滚动生成新日志文件
                                retainedFileCountLimit: 30, // 最多保留30天的日志文件，超过自动删除
                                                            // 文件日志输出模板：包含完整时间（年月日时分秒毫秒时区）+ 级别 + 内容 + 异常
                                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                                shared: true, // 允许多进程/多线程共享日志文件（解决SQLite日志锁文件问题）
                                encoding: Encoding.UTF8) // 优化：指定UTF8编码，避免中文日志乱码
                                                         // 创建Serilog全局日志实例（生效所有配置）
                            .CreateLogger();
                    }
                    catch (Exception ex)
                    {
                        // 日志初始化失败时，降级输出到控制台（最后兜底）
                        Console.WriteLine($"【严重错误】Serilog日志系统初始化失败：{ex.Message}\n{ex.StackTrace}");
                        // 抛出异常终止应用：日志系统是核心组件，初始化失败无继续运行的意义
                        throw new InvalidOperationException("日志系统初始化失败", ex);
                    }

                    // 4. 将Serilog集成到.NET原生ILogger系统中
                    // 优化：dispose: true → 应用退出时自动释放Serilog资源（关闭文件句柄、释放内存）
                    // 这样业务代码可通过注入ILogger<T>调用Serilog，无需直接依赖Serilog
                    logging.AddSerilog(Log.Logger, dispose: true);

                    // 5. 记录日志系统初始化完成的信息（验证日志配置是否生效）
                    // {LogPath}是结构化日志参数，便于后续检索日志目录
                    Log.Information("Serilog日志系统初始化完成，日志文件路径: {LogPath}", logDirectory);
                })
                // 自定义配置应用程序的配置源（扩展默认配置，添加JSON配置文件）
                // context：配置上下文，包含主机环境信息（如运行环境、配置根等）
                // config：配置构建器，用于添加/管理各类配置源（JSON、环境变量、命令行等）

                /* 此处不在需要Host读取配置文件，由AppConfigurationHelper接管
                .ConfigureAppConfiguration((context, config) =>
                {
                    // 第一步：添加主配置文件 appsettings.json
                    // 参数1 (path)：配置文件名称，此处为相对路径（指向应用运行目录的appsettings.json）
                    // 参数2 (optional)：是否可选，设为true表示文件不存在时不抛出异常（避免因配置文件缺失导致应用启动失败）
                    // 参数3 (reloadOnChange)：是否监听文件变更并自动重新加载，设为true表示修改配置文件后，应用无需重启即可读取新配置（适合运行时调整日志级别等场景）
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                    // 第二步：添加环境专属配置文件（如 appsettings.Development.json / appsettings.Production.json）
                    // path：拼接环境名称的配置文件名，context.HostingEnvironment.EnvironmentName表示当前运行环境（如Development/Production，可从环境变量/命令行/配置文件中读取）
                    // optional: true：环境专属配置文件为可选（比如生产环境可能不需要单独的配置文件）
                    // reloadOnChange: true：同样支持配置文件变更后自动重新加载
                    config.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json",
                        optional: true, reloadOnChange: true);

                    // 这句日志不会输出，因为Hosting存在强制顺序，第一个永远是ConfigureAppConfiguration
                    Log.Information("配置文件初始化完成!");
                })
                */

                // ConfigureServices: 配置依赖注入(DI)容器，是WPF/ASP.NET Core应用的核心服务注册入口
                // context：配置上下文，可访问当前应用的配置（appsettings.json）、环境信息等
                // services：DI容器的服务集合，所有需要注入的服务都通过这个对象注册
                .ConfigureServices((context, services) =>
                {
                    // 关闭Host生命周期状态消息（Application started等）
                    services.Configure<ConsoleLifetimeOptions>(options =>
                    {
                        options.SuppressStatusMessages = true; // 核心配置：设为true禁用状态消息
                    });

                    // ========== 核心服务注册：主窗口（单例模式） ==========
                    // AddSingleton<T>：注册「单例生命周期」服务
                    // 生命周期说明：整个应用程序运行期间，DI容器只会创建一个该服务的实例，所有地方获取的都是同一个
                    // 注册MainWindow为单例的原因：WPF应用通常只有一个主窗口，单例能避免重复创建多个主窗口实例
                    services.AddSingleton<MainView>();


                    // 注册数据库上下文
                    // 数据库路径串
                    // string dbDir = context.Configuration.GetConnectionString("DefaultConnection") ?? "DataBase/WpfUiTest.db;Cache=Shared";
                    // 创建数据库目录
                    string dbDir = Path.Combine(AppContext.BaseDirectory, context.Configuration.GetConnectionString("DefaultDir") ?? "DataBase"); // 拼接相对路径目录
                    dbDir = Path.GetFullPath(dbDir); // 转成真实路径（解决../解析）
                    Directory.CreateDirectory(dbDir); // 确保目录存在
                    // 注册数据库上下文
                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseSqlite($"Data Source={Path.Combine(AppContext.BaseDirectory,
                        context.Configuration.GetConnectionString("DefaultConnection") ?? "DataBase/WpfUiTest.db;Cache=Shared")}"),
                        ServiceLifetime.Singleton);

                    // 注册工作单元
                    services.AddSingleton<IUnitOfWork, UnitOfWork>();

                    // 注册仓储
                    services.AddSingleton(typeof(IRepository<>), typeof(Repository<>));  // 泛型类需要这样注册
                    services.AddSingleton<IUserRepository, UserRepository>();
                    services.AddSingleton<IToDoRepository, ToDoRepository>();
                    services.AddSingleton<IMemoRepository, MemoRepository>();

                    // 注册页面
                    services.AddSingleton<UserView>();
                    services.AddSingleton<MainView>();
                    services.AddSingleton<SettingsView>();

                    // 注册ViewModel
                    services.AddSingleton<UserViewModel>();
                    services.AddSingleton<UserRegisterViewModel>();
                    services.AddSingleton<UserLoginViewModel>();
                    services.AddSingleton<MainViewModel>();
                    services.AddSingleton<MainFooterMenuViewModel>();
                    services.AddSingleton<SettingsViewModel>();

                    // 注册服务
                    services.AddSingleton<ISnackbarService, SnackbarService>();  // 注册提示信息框
                    services.AddSingleton<IMessenger, WeakReferenceMessenger>();  // 消息提示器（弱引用）
                    services.AddSingleton<IUserService, UserService>();
                    services.AddNavigationViewPageProvider();  // WPFUI中导航
                    services.AddSingleton<INavigationService, NavigationService>();  // 导航服务
                    services.AddSingleton<IContentDialogService, ContentDialogService>();  // 对话框服务

                    // 注册工具
                    // services.AddSingleton<CredentialUtility>();  // 已删除此工具类，由LoginCredentialHelper替代


                    // ========== 后续扩展：各类服务注册示例（带注释说明） ==========
                    // 1. 数据库上下文注册（EF Core）：通常用AddDbContext，生命周期默认Scoped
                    // AddDbContext<ApplicationDbContext>：注册数据库上下文，连接数据库的核心对象
                    // options => options.UseSqlite(...)：配置数据库提供程序（如SQLite/MySQL/SQL Server）
                    // 示例：services.AddDbContext<ApplicationDbContext>(options => 
                    //          options.UseSqlite(context.Configuration.GetConnectionString("DefaultConnection")));

                    // 2. 业务服务注册：按业务场景选择生命周期
                    // AddScoped<IMyService, MyService>：注册「作用域生命周期」服务
                    // 生命周期说明：每次请求/每次操作创建一个新实例（WPF中可理解为“每次窗口打开创建一个”），适合业务服务
                    // AddTransient<ITempService, TempService>：注册「瞬时生命周期」服务
                    // 生命周期说明：每次从DI容器获取时都创建新实例，适合轻量级、无状态的临时服务
                    // 示例：services.AddScoped<IMemoService, MemoService>(); // 备忘录业务服务
                    // 示例：services.AddTransient<IToDoService, ToDoService>(); // 待办事项业务服务

                    // 3. 视图模型(ViewModel)注册：通常用Transient/Scoped
                    // WPF中ViewModel建议和对应的View（窗口/页面）生命周期一致
                    // 示例：services.AddTransient<MainWindowViewModel>(); // 主窗口视图模型
                    // 示例：services.AddScoped<MemoViewModel>(); // 备忘录页面视图模型

                    // 4. 模块化注册（进阶）：按功能模块批量注册服务
                    // 将不同功能模块的服务注册封装为扩展方法，简化ConfigureServices代码
                    // 示例：services.AddMemoModule(); // 注册备忘录模块所有服务
                    // 示例：services.AddToDoModule(); // 注册待办事项模块所有服务
                    Log.Information("DI容器初始化完成!");
                })
                .Build();
        }

        // ========== 新增：日志级别转换辅助方法 ==========
        /// <summary>
        /// 将字符串形式的日志级别名称转换为 LogEventLevel 枚举值
        /// </summary>
        /// <param name="levelStr">从配置文件读取的日志级别字符串，如 "Information"、"Error" 等</param>
        /// <returns>转换后的 LogEventLevel 枚举值；如果转换失败，则返回默认的 Information 级别</returns>
        private LogEventLevel ParseLogLevel(string levelStr)
        {
            // 使用 Enum.TryParse 尝试将字符串解析为 LogEventLevel 枚举
            // Enum.TryParse 是泛型方法，这里指定目标枚举类型为 LogEventLevel
            // ignoreCase: true 表示在解析时忽略大小写，例如 "information"、"INFORMATION" 都能正确匹配到 Information
            // out var level 是 C# 7.0 引入的内联变量声明，用于接收解析结果，如果解析成功，level 会被赋值为对应的枚举值；
            // 如果解析失败，level 会被赋值为该枚举的默认值（通常是 0，但这里不依赖它）
            // 方法返回 bool 值，表示解析是否成功，此处用 ! 取反，所以条件成立表示解析失败
            if (!Enum.TryParse<LogEventLevel>(levelStr, ignoreCase: true, out var level))
            {
                // 解析失败时的处理：记录警告日志，提醒配置有问题，并指定使用默认值
                // 这里假设存在静态的 Log 对象（如 Serilog 的 Log），如果实际项目中无此对象，可替换为其他日志输出方式
                Log.Warning("日志级别配置无效：{LevelStr}，使用默认级别 Information", levelStr);

                // 将 level 变量赋值为默认的 Information 级别，确保方法始终返回一个有效的枚举值
                level = LogEventLevel.Information;
            }

            // 返回最终确定的枚举值（可能是成功解析的值，也可能是默认的 Information）
            return level;
        }

        /// <summary>
        /// 应用程序启动入口方法，重写基类方法以自定义启动流程
        /// 此方法在构造函数执行完成后调用
        /// </summary>
        /// <param name="e">启动事件参数，包含命令行参数</param>
        protected override async void OnStartup(StartupEventArgs e)
        {
            // 调用基类方法，确保WPF基础初始化完成
            base.OnStartup(e);

            try
            {
                // 自动创建数据库（如果不存在）
                using (IServiceScope scope = ApplicationHost.Services.CreateScope())
                {
                    ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    // 方法1：直接创建数据库（不推荐用于生产）
                    // dbContext.Database.EnsureCreated();

                    // 方法2：使用迁移创建数据库（推荐）
                    dbContext.Database.Migrate();
                }

                // StartAsync: 异步启动主机，开始托管所有已注册的服务
                // 此方法会触发所有后台服务的启动
                await ApplicationHost.StartAsync();

                // GetRequiredService: 从服务容器中获取MainWindow服务实例
                // 如果服务未注册，此方法会抛出InvalidOperationException异常
                UserView loginView = this.ApplicationHost.Services.GetRequiredService<UserView>();
                // Show: 显示主窗口，启动WPF应用程序的用户界面
                loginView.Show();
                // Log.Information: 记录应用程序成功启动的日志
                Log.Information("应用程序启动成功! 主窗口已显示");
                
            }
            catch (Exception ex)
            {
                // MessageBox.Show: 显示错误消息对话框给用户
                // MessageBoxButton.OK: 对话框只包含"确定"按钮
                // MessageBoxImage.Error: 显示错误图标，提醒用户这是错误消息
                MessageBox.Show($"应用程序启动失败: {ex.Message}", "错误",
                    MessageBoxButton.OK, MessageBoxImage.Error);

                // Log.Fatal: 记录致命错误日志，通常用于应用程序启动失败等严重错误
                // ex: 捕获的异常对象，会记录完整的异常堆栈信息
                Log.Fatal("应用程序启动失败!\n错误日志: {errorMessage}", ex);

                // Shutdown: 强制关闭应用程序，参数1表示非正常退出代码
                // 非零退出代码通常表示应用程序异常终止
                Shutdown(1);
            }
        }

        /// <summary>
        /// 应用程序退出方法，重写基类方法以优雅关闭主机和服务
        /// 此方法在应用程序关闭时自动调用
        /// </summary>
        /// <param name="e">退出事件参数，包含应用程序退出代码</param>
        protected override async void OnExit(ExitEventArgs e)
        {
            // 调用基类方法，确保WPF基础清理工作完成
            base.OnExit(e);

            // 检查ApplicationHost是否已成功初始化
            if (ApplicationHost != null)
            {
                // using语句确保IHost实例正确释放资源，即使发生异常也会调用Dispose
                using (ApplicationHost)
                {
                    try
                    {
                        // StopAsync: 优雅停止主机，给所有后台服务时间完成正在进行的操作
                        // TimeSpan.FromSeconds(5): 设置停止超时时间为5秒
                        // 如果服务在5秒内未停止，主机会强制终止
                        await ApplicationHost.StopAsync(TimeSpan.FromSeconds(5));

                        // Log.Information: 记录主机已成功停止的日志
                        Log.Information("应用程序主机已成功停止");
                    }
                    catch (Exception ex)
                    {
                        // Log.Error: 记录主机停止过程中发生的错误
                        Log.Error("停止应用程序主机时发生错误", ex);
                    }
                }
            }

            // 关闭Serilog日志系统，确保所有日志都写入目标
            Log.CloseAndFlush();
        }
    }
}
