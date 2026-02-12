using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Wpf.Ui;
using WpfUiTest.App.ViewModels.Mapping;
using WpfUiTest.Core.Services.Interfaces;
using WpfUiTest.Shared.Base;
using WpfUiTest.Shared.Utilities;

namespace WpfUiTest.App.ViewModels.User
{
    // 用户注册ViewModel
    public class UserRegisterViewModel : BaseViewModel
    {
        // ============================= 字段 属性 ===================================
        // 字段：用户服务层
        private readonly IUserService _userService;


        // 用户账户
        private string _account = string.Empty;
        public string Account
        {
            get { return _account; }
            set { SetProperty(ref _account, value); }
        }
        // 用户昵称
        private string _userName = string.Empty;
        public string UserName
        {
            get { return _userName; }
            set { SetProperty(ref _userName, value); }
        }
        // 用户密码
        private string _password = string.Empty;
        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }
        // 用户确认密码
        private string _confirmPassword = string.Empty;
        public string ConfirmPassword
        {
            get { return _confirmPassword; }
            set { SetProperty(ref _confirmPassword, value); }
        }


        // ============================= 构造函数 ===================================
        public UserRegisterViewModel(IUserService userService)
        {
            // 初始化字段
            this._userService = userService;

            // 初始化属性

            // 初始化命令

        }


        // ============================= 方法 ===================================
        // 注册方法
        public async Task<ServiceResult<bool>> RegisterAsync()
        {
            // 调用服务返回结果
            ServiceResult<bool> UserRegisterResult = await this._userService.RegisterAsync(this.ToRegisterUserDto());
            // 如果是注册成功才会清空表单
            if (UserRegisterResult.IsSuccess) { this.Clear(); }
            // 返回服务结果给主VM
            return UserRegisterResult;
        }

        // 清空
        private void Clear()
        {
            this.Account = string.Empty;
            this.UserName = string.Empty;
            this.Password = string.Empty;
            this.ConfirmPassword = string.Empty;
        }
    }
}
