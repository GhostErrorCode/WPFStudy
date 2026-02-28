using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using WpfUiTest.Core.Services.Interfaces;
using WpfUiTest.Shared.Base;
using WpfUiTest.Shared.Enums;
using WpfUiTest.Shared.Extensions;

namespace WpfUiTest.App.ViewModels.Main
{
    // 设置 - 用户设置
    public class SettingsUserViewModel : BaseViewModel
    {
        // ==================== 字段属性 ====================
        // 字段: 用户服务
        private readonly IUserService _userService;
        // 字段: Window窗口服务管理
        private readonly IWindowService _windowService;
        // 字段: IMessenger服务
        private readonly IMessenger _messenger;
        // 字段; ILogger服务
        private readonly ILogger<SettingsUserViewModel> _logger;

        // 属性: 用户昵称
        public string UserName { get; set; } = string.Empty;

        // Command: 登出当前用户Command
        public RelayCommand UserLogoutCommand { get; private set; }



        // ==================== 构造函数 ====================
        public SettingsUserViewModel(IUserService userService, IWindowService windowService, IMessenger messenger, ILogger<SettingsUserViewModel> logger)
        {
            // 初始化字段
            this._userService = userService;
            this._windowService = windowService;
            this._messenger = messenger;
            this._logger = logger;

            // 初始化属性

            // 初始化命令
            this.UserLogoutCommand = new RelayCommand(UserLogout);
        }

        // ==================== 方法 ====================
        // 方法: 用户登出方法
        private void UserLogout()
        {
            try
            {
                this._logger.LogDebug("SettingsUserViewModel: 用户登出开始");

                // 调用Core层的用户服务
                this._userService.Logout();

                // 隐藏并销毁主页面打开登录页面
                this._windowService.HideMainWindow();
                this._windowService.ShowUserWindow();

                // 输出日志并显示Snackbar
                this._messenger.ShowSuccess(SnackbarTarget.UserView, "用户已登出", $"用户登出成功");
                this._logger.LogInformation("用户 {username} 登出成功", this.UserName);  // 这块先写死，等后面看情况优化
            }
            catch(Exception ex)
            {
                this._logger.LogError("用户登出失败! 发生意外的未处理异常: {ex}", ex);
                this._messenger.ShowDanger(SnackbarTarget.MainView, "用户登出失败!", "系统出现意外的严重错误!");
            }
            finally
            {
                this._logger.LogDebug("SettingsUserViewModel: 用户登出结束");
            }
        }

        // 方法: 导航完成后方法
        public override async Task OnNavigatedToAsync()
        {
            // 给当前用户昵称赋值
            this.UserName = this._userService.CurrentUser != null ? this._userService.CurrentUser.UserName : "[NULL]";

            // 调用父方法
            await base.OnNavigatedToAsync();
        }
    }
}
