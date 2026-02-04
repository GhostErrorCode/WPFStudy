using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;
using WpfUiTest.App.ViewModels.Enums;

namespace WpfUiTest.App.ViewModels.User
{
    // 继承 ObservableObject 实现通知更改等
    public class UserViewModel : ObservableObject
    {
        // ============================= 字段 属性 ===================================
        // 当前选中的视图类型 注册或登录
        private UserViewType _selectedUserViewType = UserViewType.Login;
        public UserViewType SelectedUserViewType
        {
            get { return _selectedUserViewType; }
            set { SetProperty(ref _selectedUserViewType, value); }
        }

        // ============================= 命令 Command ===================================
        // 切换注册或登录视图类型
        public RelayCommand SwitchUserViewTypeCommand { get; private set; }


        // ============================= 构造函数 ===================================
        public UserViewModel()
        {
            // 初始化字段、属性、命令
            this.SwitchUserViewTypeCommand = new RelayCommand(SwitchUserViewType);
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
    }
}
