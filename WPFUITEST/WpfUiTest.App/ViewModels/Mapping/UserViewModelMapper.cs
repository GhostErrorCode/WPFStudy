using System;
using System.Collections.Generic;
using System.Text;
using WpfUiTest.App.ViewModels.User;
using WpfUiTest.Core.DTOs.User;

namespace WpfUiTest.App.ViewModels.Mapping
{
    // User 相关的 ViewModel 与 DTO 转换
    public static class UserViewModelMapper
    {
        // UserRegisterViewModel 转 RegisterUserDto
        public static RegisterUserDto ToRegisterUserDto(this UserRegisterViewModel userRegisterViewModel)
        {
            return new RegisterUserDto()
            {
                Account = userRegisterViewModel.Account,
                UserName = userRegisterViewModel.UserName,
                Password = userRegisterViewModel.Password,
                ConfirmPassword = userRegisterViewModel.ConfirmPassword
            };
        }

        // UserLoginViewModel 转 LoginUserDto
        public static LoginUserDto ToLoginUserDto(this UserLoginViewModel userLoginViewModel)
        {
            return new LoginUserDto()
            {
                Account = userLoginViewModel.Account,
                Password = userLoginViewModel.Password,
                IsRememberMe = userLoginViewModel.IsRememberMe
            };
        }
    }
}
