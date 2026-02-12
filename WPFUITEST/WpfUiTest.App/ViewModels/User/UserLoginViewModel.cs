using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;
using WpfUiTest.App.ViewModels.Mapping;
using WpfUiTest.Core.Services.Interfaces;
using WpfUiTest.Shared.Base;
using WpfUiTest.Shared.Utilities;

namespace WpfUiTest.App.ViewModels.User
{
    // 用户登录ViewModel
    public class UserLoginViewModel : BaseViewModel
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
        // 用户密码
        private string _password = string.Empty;
        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }
        // 是否记住我
        private bool _isRememberMe = false;
        public bool IsRememberMe
        {
            get { return _isRememberMe; }
            set { SetProperty(ref _isRememberMe, value); }
        }


        // ============================= 构造函数 ===================================
        public UserLoginViewModel(IUserService userService)
        {
            // 初始化字段
            this._userService = userService;

            // 初始化属性

            // 初始化命令

        }


        // ============================= 方法 ===================================
        // 登录方法
        public async Task<ServiceResult<bool>> LoginAsync()
        {
            // 调用服务返回结果
            ServiceResult<bool> UserLoginResult = await this._userService.LoginAsync(this.ToLoginUserDto());
            // 如果是登录成功才会清空表单
            if (UserLoginResult.IsSuccess) { this.Clear(); }
            // 返回服务结果给主VM
            return UserLoginResult;
        }

        // 自动登录方法
        public async Task<ServiceResult<bool>> AutoLoginAsync(int userId, string account)
        {
            // 调用服务返回结果
            ServiceResult<bool> UserAutoLoginResult = await this._userService.AutoLoginAsync(userId, account);
            // 返回服务结果给主VM
            return UserAutoLoginResult;
        }

        // 清空
        private void Clear()
        {
            this.Account = string.Empty;
            this.Password = string.Empty;
            this.IsRememberMe = false;
        }
    }
}
