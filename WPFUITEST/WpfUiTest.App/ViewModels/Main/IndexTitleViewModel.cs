using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Threading;
using WpfUiTest.Core.Services.Interfaces;
using WpfUiTest.Shared.Base;
using WpfUiTest.Shared.Extensions;
using WpfUiTest.Shared.Utilities;

namespace WpfUiTest.App.ViewModels.Main
{
    // 首页标题ViewModel
    public class IndexTitleViewModel : BaseViewModel
    {
        // ==================== 字段属性命令 ====================
        // 字段: 用户服务
        private readonly IUserService _userService;
        // 字段: 计时器
        private readonly DispatcherTimer _dispatcherTimer;
        private readonly ILogger<IndexTitleViewModel> _logger;

        // 属性：首页欢迎语
        private string _welcomeMessage = string.Empty;
        public string WelcomeMessage
        {
            get { return _welcomeMessage; }
            set { SetProperty(ref _welcomeMessage, value); }
        }
        // 属性: 当前登录用户的昵称
        private string _userName = string.Empty;
        public string UserName
        {
            get { return _userName; } 
            set { SetProperty(ref _userName, value); }
        }
        // 属性: 当前时间
        private string _currentDateTime = DateTime.Now.ToString("yyyy年MM月dd日 dddd HH:mm");
        public string CurrentDateTime
        {
            get { return _currentDateTime; }
            set { SetProperty(ref _currentDateTime, value); }
        }


        // ==================== 构造函数 ====================
        public IndexTitleViewModel(IUserService userService, ILogger<IndexTitleViewModel> logger)
        {
            // 初始化字段
            this._userService = userService;
            this._logger = logger;

            // 计时器,用于更新首页当前时间
            this._dispatcherTimer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(1) };
            this._dispatcherTimer.Tick += (object? s, EventArgs e) => { this.CurrentDateTime = DateTime.Now.ToString("yyyy年MM月dd日 dddd HH:mm"); this.WelcomeMessage = DateTime.Now.Hour.ToWelcomeMessage(); };
            this._dispatcherTimer.Start();
            // 输出日志
            this._logger.LogInformation("首页（IndexView）标题计时器状态: {IsTimerRunning}", this._dispatcherTimer.IsEnabled ? "已启动" : "已停止");

            // 初始化属性
        }


        // ==================== 方法 ====================
        // 方法: 导航完成
        public override async Task OnNavigatedToAsync()
        {
            // 获取当前登录用户的昵称
            this.UserName = this._userService.CurrentUser != null ? this._userService.CurrentUser.UserName : "[NULL]";
            // 获取当前日期时间
            this.CurrentDateTime = DateTime.Now.ToString("yyyy年MM月dd日 dddd HH:mm");
            // 根据当前日期时间更改欢迎语
            this.WelcomeMessage = DateTime.Now.Hour.ToWelcomeMessage();

            await base.OnNavigatedToAsync();
        }
    }
}
