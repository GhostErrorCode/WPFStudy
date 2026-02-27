using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Wpf.Ui;
using Wpf.Ui.Controls;
using WpfUiTest.App.ViewModels.User;
using WpfUiTest.App.Views.Main;
using WpfUiTest.Shared.Enums;
using WpfUiTest.Shared.Messages;

namespace WpfUiTest.App.Views.User
{
    /// <summary>
    /// UserView.xaml 的交互逻辑
    /// </summary>
    public partial class UserView : FluentWindow
    {
        // 字段 属性
        // IMessenger 消息转发
        private readonly IMessenger _messenger;
        // IServiceProvider 服务
        private readonly IServiceProvider _serviceProvider;

        // 依赖注入获取对应ViewModel
        public UserView(UserViewModel userViewModel, IMessenger messenger, IServiceProvider serviceProvider)
        {
            InitializeComponent();

            // 获取ViewModel
            this.DataContext = userViewModel;
            // 初始化服务(依赖注入)
            this._messenger = messenger;
            this._serviceProvider = serviceProvider;

            /// <summary>
            /// 窗体Loaded事件：页面加载完成后订阅指定令牌的SnackBar消息
            /// Loaded事件触发时机：窗体可视化并布局完成后，确保控件已初始化（避免操作空控件）
            /// </summary>

            // ContentRendered 事件后执行自动登录方法
            this.ContentRendered += async (s, e) =>
            {
                // 因UserView存在动画切换效果，直接开始代码会造成明显卡顿，这里延迟500ms
                await Task.Delay(500);
                await userViewModel.UserAutoLogin();
            };
                
            // ===== 订阅登录成功后打开主页面(已废弃，由UserViewModel进行管理窗体的打开关闭) =====
            this.Loaded += (Object s, RoutedEventArgs e) =>
            {
                this._messenger.Register<LoginSuccessMessage>(this, (Object a, LoginSuccessMessage b) =>
                {
                    // 打开主页面
                    MainView main = this._serviceProvider.GetRequiredService<MainView>();
                    Application.Current.MainWindow = main;
                    main.Show();
                    // 关闭当前页面
                    this.Close();
                });
            };
            
            // 订阅定向SnackBar投放
            this.Loaded += (Object s, RoutedEventArgs e) =>
            {
                // 订阅IMessenger消息：
                // 泛型1（TargetedSnackbarMessage）：要接收的消息类型
                // 泛型2（string）：消息令牌的类型（此处用枚举转字符串作为令牌）
                this._messenger.Register<TargetedSnackbarMessage, string>(
                    // 订阅者实例：当前窗体（this），用于后续注销时定位订阅关系
                    this,
                    // 消息令牌：仅接收该令牌的消息（SnackbarTarget.UserView枚举转字符串，精准过滤）
                    SnackbarTarget.UserView.ToString(),
                    // 消息接收回调：当收到匹配「消息类型+令牌」的消息时执行
                    // recipient：订阅者实例（即当前窗体this）；message：接收到的消息实体
                    (recipient, message) =>
                    {
                        // 关键：WPF中控件必须在UI线程操作（消息可能从后台线程/类库发布）
                        // Dispatcher.Invoke：强制切换到UI线程执行，避免“线程间操作无效”异常
                        Dispatcher.Invoke(() =>
                        {
                            // 双重验证：防止令牌匹配但消息目标不匹配的误触发（兜底校验）
                            // 比如令牌是UserView，但消息Target误设为LoginView，直接过滤
                            if (message.Target != SnackbarTarget.UserView) return;

                            // 1. 创建SnackBar实例：传入SnackbarPresenter容器，绑定显示载体
                            // 2. AddToQue：将SnackBar加入容器队列（支持多个消息依次显示）
                            UserSnackbarPresenter.AddToQue(new Snackbar(UserSnackbarPresenter)
                            {
                                Title = message.Title,       // SnackBar标题文本（如“操作提示”）
                                Content = message.Message,   // SnackBar正文内容（如“登录成功”）
                                Appearance = message.Appearance, // SnackBar样式（Success/Danger/Info等）
                                Icon = message.Icon,         // SnackBar左侧图标（增强视觉提示）
                                Timeout = message.Timeout    // SnackBar自动关闭时长（TimeSpan类型，如3秒）
                            });
                        });
                    });
            };
            /// <summary>
            /// 窗体Unloaded事件：页面卸载时注销所有消息订阅（核心：防止内存泄漏）
            /// Unloaded触发时机：窗体从可视化树移除时（关闭/隐藏并卸载）
            /// </summary>
            this.Unloaded += (s, e) =>
            {
                // 注销当前窗体（this）的所有IMessenger订阅
                // this._messenger.UnregisterAll(this);
                // 精准注销：匹配「消息类型+令牌类型+订阅者+令牌值」
                this._messenger.Unregister<TargetedSnackbarMessage, string>(
                    this,                  // 订阅者实例（和注册时一致）
                    SnackbarTarget.UserView.ToString()
                    ); // 令牌值（和注册时一致）

                // 注销订阅的登录成功后打开主页面
                this._messenger.Unregister<LoginSuccessMessage>(this);
            };

        }
    }
}

/*
    构造函数：初始化数据、注册服务
    Loaded：设置UI控件、绑定事件、加载数据
    ContentRendered：启动动画、延迟加载
    Activated：刷新数据、检查状态
    Closing：验证保存、确认关闭
    Closed：清理资源、保存状态
*/
// 1. 构造函数 (Constructor)
// - 窗口对象创建，执行初始化
// - 调用InitializeComponent()解析XAML并创建控件
// - 控件对象已存在，但布局未计算

// 2. OnInitialized (初始化完成)
// - 窗口和所有子控件初始化完成
// - 基类初始化逻辑执行完毕

// 3. Loaded事件 (窗口加载完成)
// - 窗口已加入可视化树
// - 布局计算完成，控件位置大小已确定
// - 窗口即将显示但还未显示
// - ★ 设置Presenter的最佳时机

// 4. OnSourceInitialized (句柄创建)
// - 窗口句柄(HWND)已创建
// - 可以处理窗口消息和互操作

// 5. OnContentRendered (内容渲染完成)
// - 窗口内容已完全渲染到屏幕
// - 窗口已显示，所有可视化内容可见
// - 可执行需要窗口完全显示后的操作

// 6. Activated事件 (窗口激活)
// - 窗口获得焦点，变为前台窗口
// - 用户点击窗口或通过代码激活

// --- 运行时重复事件 ---
// 7. Deactivated事件 (窗口失活)
// - 窗口失去焦点，变为后台窗口

// 8. Activated事件 (再次激活)
// - 窗口重新获得焦点

// 9. LocationChanged事件 (位置改变)
// - 窗口位置发生变化

// 10. SizeChanged事件 (大小改变)
// - 窗口大小发生变化

// 11. StateChanged事件 (状态改变)
// - 窗口状态变化(正常/最小化/最大化)

// --- 关闭阶段事件 ---
// 12. Closing事件 (正在关闭)
// - 窗口即将关闭，但可取消
// - 可设置e.Cancel=true阻止关闭

// 13. OnClosed (已关闭)
// - 窗口已关闭，从屏幕移除

// 14. Deactivated事件 (关闭后失活)
// - 窗口关闭后可能触发

// 15. Unloaded事件 (窗口卸载)
// - 窗口从可视化树中移除
// - 可执行清理资源操作