using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;
using WpfUiTest.App.ViewModels.Main;
using WpfUiTest.App.ViewModels.User;
using WpfUiTest.Shared.Enums;
using WpfUiTest.Shared.Messages;

namespace WpfUiTest.App.Views.Main
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : FluentWindow
    {
        // IMessenger 消息转发
        private readonly IMessenger _messenger;
        // ILogger 日志
        private readonly ILogger<MainView> _logger;

        // 依赖注入获取对应ViewModel
        public MainView(MainViewModel mainViewModel, INavigationService navigationService, IContentDialogService contentDialogService, IMessenger messenger, ILogger<MainView> logger)
        {
            InitializeComponent();

            // 获取ViewModel
            this.DataContext = mainViewModel;

            // 依赖注入服务
            this._messenger = messenger;
            this._logger = logger;

            // 服务初始化
            // 设置导航服务的导航控件，以便能够实现DI注入
            // navigationService.SetNavigationControl(MainNavigationView);
            // 对话框服务设置Host
            contentDialogService.SetDialogHost(MainContentDialogHost);


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
                    SnackbarTarget.MainView.ToString(),
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
                            if (message.Target != SnackbarTarget.MainView) return;

                            // 1. 创建SnackBar实例：传入SnackbarPresenter容器，绑定显示载体
                            // 2. AddToQue：将SnackBar加入容器队列（支持多个消息依次显示）
                            MainSnackbarPresenter.AddToQue(new Snackbar(MainSnackbarPresenter)
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


            // Loaded事件与ContentRendered事件的区别——Loaded事件是在显示窗口之前发生的；
            // ContentRendered事件是在窗口内容呈现后 即被渲染之后发生的。
            /* 已废弃，由IWindowService管理
            this.ContentRendered += (s, e) =>
            {
                // 默认首次打开时导航到首页
                if (navigationService.Navigate(typeof(IndexView)))
                {
                    this._logger.LogInformation("首次导航至默认首页成功!");
                }
                else
                {
                    this._logger.LogInformation("首次导航至默认首页失败!");
                }
            };
            */

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
                    SnackbarTarget.MainView.ToString()
                    ); // 令牌值（和注册时一致）
            };
        }
    }
}
