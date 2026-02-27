using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using WpfUiTest.App.ViewModels.Main;
using WpfUiTest.App.Views.Main;
using WpfUiTest.App.Views.User;
using WpfUiTest.Core.Services.Interfaces;

namespace WpfUiTest.App.Services.Implements
{
    // Window窗体服务接口实现类
    public class WindowService : IWindowService
    {
        // ========== 字段属性 ==========
        // 字段: 用户登录注册页面
        private UserView? _userView;
        // 字段: 主页面
        private MainView? _mainView;
        // 字段: IServiceProvider DI容器服务
        private readonly IServiceProvider _serviceProvider;


        // ========== 构造函数 ==========
        public WindowService(IServiceProvider serviceProvider)
        {
            // ========== 这里不能使用依赖注入，否则会陷入循环依赖，改用IServiceProvider获取 ==========

            // 初始化字段
            this._serviceProvider = serviceProvider;

            // 初始化属性
            // 初始化命令
        }



        // ========== 接口方法 ==========
        // 显示主窗口
        public void ShowMainWindow()
        {
            // 延迟获取MainView（用到时才从DI拿，而非构造时）
            this._mainView = this._serviceProvider.GetRequiredService<MainView>();
            // 设置WPF上下文主窗口
            Application.Current.MainWindow = this._mainView;
            // 打开窗口
            this._mainView.Show();
        }

        // 隐藏登录窗口
        public void HideUserWindow()
        {
            // 隐藏窗口
            this._userView = this._serviceProvider.GetRequiredService<UserView>();
            this._userView.Visibility = Visibility.Hidden;
        }
    }
}
