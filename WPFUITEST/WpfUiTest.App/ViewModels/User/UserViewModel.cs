using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Wpf.Ui;
using Wpf.Ui.Abstractions.Controls;
using Wpf.Ui.Controls;
using WpfUiTest.App.ViewModels.Enums;
using WpfUiTest.App.Views.Main;
using WpfUiTest.App.Views.User;
using WpfUiTest.Core.Services.Interfaces;
using WpfUiTest.Shared.Base;
using WpfUiTest.Shared.Enums;
using WpfUiTest.Shared.Extensions;
using WpfUiTest.Shared.Messages;
using WpfUiTest.Shared.Models;
using WpfUiTest.Shared.Utilities;

namespace WpfUiTest.App.ViewModels.User
{
    // 继承 ObservableObject 实现通知更改等
    public class UserViewModel : BaseViewModel
    {
        // ============================= 字段 属性 ===================================
        // 字段：Snackbar 服务
        private readonly ISnackbarService _snackbarService;
        // 字段：IMessenger服务
        private readonly IMessenger _messenger;
        // 字段：IServiceProvider服务
        private readonly IServiceProvider _serviceProvider;
        // 字段：ILogger服务
        private readonly ILogger<UserViewModel> _logger;
        // 字段: IWindow窗体服务
        private readonly IWindowService _windowService;


        // 属性：当前选中的视图类型 注册或登录
        private UserViewType _selectedUserViewType = UserViewType.Login;
        public UserViewType SelectedUserViewType
        {
            get { return _selectedUserViewType; }
            set { SetProperty(ref _selectedUserViewType, value); }
        }
        // 属性：用户注册ViewModel
        private UserRegisterViewModel _userRegisterViewModel;
        public UserRegisterViewModel UserRegisterViewModel
        {
            get { return _userRegisterViewModel; }
            set { SetProperty(ref _userRegisterViewModel, value); }
        }
        // 属性：用户登录ViewModel
        private UserLoginViewModel _userLoginViewModel;
        public UserLoginViewModel UserLoginViewModel
        {
            get { return _userLoginViewModel; }
            set { SetProperty(ref _userLoginViewModel, value); }
        }

        // ============================= 命令 Command ===================================
        // 切换注册或登录视图类型Command
        public RelayCommand SwitchUserViewTypeCommand { get; private set; }
        // 用户注册Command
        public AsyncRelayCommand UserRegisterCommand { get; private set; }
        // 用户登录Command
        public AsyncRelayCommand UserLoginCommand { get; private set; }


        // ============================= 构造函数 ===================================
        public UserViewModel(IServiceProvider serviceProvider, ISnackbarService snackbarService, IMessenger messenger, ILogger<UserViewModel> logger, IWindowService windowService)
        {
            // 初始化字段
            this._serviceProvider = serviceProvider;
            this._snackbarService = snackbarService;
            this._messenger = messenger;
            this._logger = logger;
            this._windowService = windowService;

            // 初始化属性
            this._userRegisterViewModel = this._serviceProvider.GetRequiredService<UserRegisterViewModel>();
            this._userLoginViewModel = this._serviceProvider.GetRequiredService<UserLoginViewModel>();

            // 初始化命令
            this.SwitchUserViewTypeCommand = new RelayCommand(SwitchUserViewType);
            this.UserRegisterCommand = new AsyncRelayCommand(UserRegister);
            this.UserLoginCommand = new AsyncRelayCommand(UserLogin);
        }


        // ============================= 方法 ===================================
        // 切换注册或登录视图方法
        private void SwitchUserViewType()
        {
            // 切换当前视图，如果是注册就切换成登录，如果是登录就切换成注册
            switch (this._selectedUserViewType)
            {
                case UserViewType.Login: this.SelectedUserViewType = UserViewType.Register; break;
                case UserViewType.Register: this.SelectedUserViewType= UserViewType.Login; break;
            }
        }

        // 用户注册方法
        private async Task UserRegister()
        {
            try
            {
                this._logger.LogDebug("UserViewModel：开始调用注册服务");
                // 调用副VM并接收结果
                ServiceResult<bool> registerResult = await this._userRegisterViewModel.RegisterAsync();
                // 如果结果不为空且注册成功
                if (registerResult != null && registerResult.IsSuccess)
                {
                    // 打印日志
                    this._logger.LogInformation("用户注册成功! {@RegisterResult}", registerResult);
                    // 展示弹窗
                    this._messenger.ShowSuccess(SnackbarTarget.UserView, registerResult.Message, "注册成功! 请返回登录页面进行登录!");
                    // 返回登录页面
                    this.SelectedUserViewType = UserViewType.Login;
                }
                else if (registerResult != null && !registerResult.IsSuccess) // 注册失败的话
                {
                    // 打印日志
                    this._logger.LogWarning("用户注册失败! {@RegisterResult}", registerResult);
                    // 展示弹窗
                    this._messenger.ShowDanger(SnackbarTarget.UserView, registerResult.Message, "注册失败! 请检查输入的内容!");
                }
                else
                {
                    // 打印日志
                    this._logger.LogError("用户注册失败! {@RegisterResult}", registerResult);
                    // 展示弹窗
                    this._messenger.ShowDanger(SnackbarTarget.UserView, "系统异常!", "注册失败! 系统异常!");
                }
                
            }
            catch(Exception ex)
            {
                this._logger.LogError("用户注册失败! 发生意外的未处理异常: {ex}", ex);
                this._messenger.ShowDanger(SnackbarTarget.UserView,"操作失败!", "系统出现意外的严重错误!");
            }
            finally
            {
                this._logger.LogDebug("UserViewModel：调用注册服务完成");
            }
        }

        // 用户自动登录方法
        public async Task UserAutoLogin()
        {
            try
            {
                this._logger.LogDebug("UserViewModel：开始调用自动登录服务");
                // 尝试加载登录凭证
                LoginCredential? loginCredential = await LoginCredentialHelper.LoadLoginCredential();
                // 如果返回的是NULL则视为无效凭证(不进行提示，静默处理)
                if (loginCredential == null) { return; }
                // 登录凭证超过30天
                if (loginCredential.Expires < DateTime.Now)
                {
                    this._logger.LogWarning("自动登录失败: 登录凭证已超时失效，当前需要手动登录");
                    this._messenger.ShowDanger(SnackbarTarget.UserView, "登录凭证失效!", "登录凭证已超时失效，当前需要手动登录");
                }
                // ====== 以下为登录凭证有效的情况 ======
                // 尝试登录凭证
                ServiceResult<bool> autoLoginResult = await this.UserLoginViewModel.AutoLoginAsync(loginCredential.UserId, loginCredential.Account);
                // 自动登录成功
                if (autoLoginResult.IsSuccess)
                {
                    // 显示欢迎语，短暂停留后跳转主页
                    // 打印日志
                    this._logger.LogInformation("自动登录成功: 用户自动登录成功! {@RegisterResult}", autoLoginResult);
                    // 展示弹窗
                    this._messenger.ShowSuccess(SnackbarTarget.UserView, autoLoginResult.Message, "自动登录成功! 欢迎回来!");

                    // 延迟打开主窗口
                    await Task.Delay(2000);
                    // 发送打开主窗口的消息（UserView已订阅，会自动执行打开主页面并关闭自己）
                    // this._messenger.Send(new LoginSuccessMessage()); 已废弃

                    // 用IWindowService服务控制窗口
                    this._windowService.ShowMainWindow();
                    this._windowService.HideUserWindow();
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError("自动登录失败: 处理登录凭证过程中出现意外的严重错误! 异常信息: {ex}", ex);
            }
            finally
            {
                this._logger.LogDebug("UserViewModel：调用自动登录服务完成");
            }
            
        }

        // 用户登录方法
        private async Task UserLogin()
        {
            try
            {
                this._logger.LogDebug("UserViewModel：开始调用登录服务");
                // 调用副VM并接收结果
                ServiceResult<bool> loginResult = await this._userLoginViewModel.LoginAsync();

                // 如果结果不为空且登录成功
                if (loginResult != null && loginResult.IsSuccess)
                {
                    // 打印日志
                    this._logger.LogInformation("用户登录成功! {@RegisterResult}", loginResult);
                    // 展示弹窗
                    this._messenger.ShowSuccess(SnackbarTarget.UserView, loginResult.Message, "登录成功! 欢迎回来!");

                    // 延迟打开主窗口
                    await Task.Delay(2000);
                    // 发送打开主窗口的消息（UserView已订阅，会自动执行打开主页面并关闭自己）
                    // this._messenger.Send(new LoginSuccessMessage()); 已废弃

                    // 用IWindowService服务控制窗口
                    this._windowService.ShowMainWindow();
                    this._windowService.HideUserWindow();
                }
                // 如果结果不为空但登录失败
                else if(loginResult != null && !loginResult.IsSuccess)
                {
                    // 打印日志
                    this._logger.LogWarning("用户登录失败! {@RegisterResult}", loginResult);
                    // 展示弹窗
                    this._messenger.ShowDanger(SnackbarTarget.UserView, loginResult.Message, "登录失败! 请检查输入的内容!");
                }
                // 其他可能的异常
                else
                {
                    // 打印日志
                    this._logger.LogError("用户登录失败! {@RegisterResult}", loginResult);
                    // 展示弹窗
                    this._messenger.ShowDanger(SnackbarTarget.UserView, "系统异常!", "登录失败! 系统异常!");
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError("用户登录失败! 发生意外的未处理异常, 异常信息: {ex}", ex);
                this._messenger.ShowDanger(SnackbarTarget.UserView, "操作失败!", "系统出现意外的严重错误!");
            }
            finally
            {
                this._logger.LogDebug("UserViewModel：调用登录服务完成");
            }
        }
    }
}
