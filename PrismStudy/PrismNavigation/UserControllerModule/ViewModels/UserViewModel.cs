using System;
using System.Collections.Generic;
using System.Text;
using Prism.Commands;  // 引入Prism命令相关类
using Prism.Mvvm;  // 引入Prism MVVM相关类

namespace UserControllerModule.ViewModels
{
    /// <summary>
    /// UserViewModel 用户视图模型类
    /// 这个类处理用户视图的业务逻辑和数据
    /// 实现INavigationAware接口以支持导航功能
    /// </summary>
    public class UserViewModel : BindableBase, INavigationAware  // 继承BindableBase并实现INavigationAware接口
    {
        /// <summary>
        /// 区域管理器实例，用于管理区域导航
        /// </summary>
        private readonly IRegionManager _regionManager;

        /// <summary>
        /// 用户姓名字段
        /// </summary>
        private string _userName;

        /// <summary>
        /// 用户邮箱字段
        /// </summary>
        private string _userEmail;

        /// <summary>
        /// 用户年龄字段
        /// </summary>
        private int _userAge;

        /// <summary>
        /// 导航参数字段
        /// </summary>
        private string _navigationParameter;

        /// <summary>
        /// UserViewModel 构造函数
        /// 通过依赖注入接收区域管理器实例
        /// </summary>
        /// <param name="regionManager">区域管理器实例，用于导航管理</param>
        public UserViewModel(IRegionManager regionManager)
        {
            // 将传入的区域管理器实例赋值给私有字段
            this._regionManager = regionManager;
            // 初始化返回命令，绑定到ExecuteGoBackCommand方法
            this.GoBackCommand = new DelegateCommand(this.ExecuteGoBackCommand);
        }

        /// <summary>
        /// UserName 用户姓名属性
        /// 实现属性变更通知，当值改变时通知UI更新
        /// </summary>
        public string UserName
        {
            // 获取用户姓名
            get { return this._userName; }
            // 设置用户姓名，如果值改变则触发属性变更通知
            set { this.SetProperty(ref this._userName, value); }
        }

        /// <summary>
        /// UserEmail 用户邮箱属性
        /// 实现属性变更通知，当值改变时通知UI更新
        /// </summary>
        public string UserEmail
        {
            // 获取用户邮箱
            get { return this._userEmail; }
            // 设置用户邮箱，如果值改变则触发属性变更通知
            set { this.SetProperty(ref this._userEmail, value); }
        }

        /// <summary>
        /// UserAge 用户年龄属性
        /// 实现属性变更通知，当值改变时通知UI更新
        /// </summary>
        public int UserAge
        {
            // 获取用户年龄
            get { return this._userAge; }
            // 设置用户年龄，如果值改变则触发属性变更通知
            set { this.SetProperty(ref this._userAge, value); }
        }

        /// <summary>
        /// NavigationParameter 导航参数属性
        /// 显示接收到的导航参数信息
        /// </summary>
        public string NavigationParameter
        {
            // 获取导航参数
            get { return this._navigationParameter; }
            // 设置导航参数，如果值改变则触发属性变更通知
            set { this.SetProperty(ref this._navigationParameter, value); }
        }

        /// <summary>
        /// GoBackCommand 返回命令属性
        /// 公开的只读属性，用于绑定到UI的返回按钮
        /// </summary>
        public DelegateCommand GoBackCommand { get; private set; }

        /// <summary>
        /// ExecuteGoBackCommand 执行返回命令的方法
        /// 当用户点击返回按钮时调用此方法
        /// </summary>
        private void ExecuteGoBackCommand()
        {
            // 使用区域管理器请求导航回主视图
            this._regionManager.RequestNavigate("MainRegion", "MainView");
        }

        /// <summary>
        /// IsNavigationTarget 判断是否可以作为导航目标的方法
        /// 当导航到该视图时，Prism会调用此方法判断是否重用现有实例
        /// </summary>
        /// <param name="navigationContext">导航上下文，包含导航信息</param>
        /// <returns>返回true表示可以重用现有实例，false表示创建新实例</returns>
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            // 总是返回true，表示重用现有视图模型实例
            return true;
        }

        /// <summary>
        /// OnNavigatedFrom 导航离开时调用的方法
        /// 当从该视图导航到其他视图时调用
        /// </summary>
        /// <param name="navigationContext">导航上下文，包含导航信息</param>
        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            // 可以在这里保存视图状态或清理资源
            // 当前没有需要清理的资源，所以方法体为空
        }

        /// <summary>
        /// OnNavigatedTo 导航到达时调用的方法
        /// 当导航到该视图时调用，用于处理导航参数
        /// </summary>
        /// <param name="navigationContext">导航上下文，包含导航参数信息</param>
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            // 检查参数集合中是否包含userName参数
            if (navigationContext.Parameters.ContainsKey("userName"))
            {
                // 从参数中获取userName的值并设置到UserName属性
                this.UserName = navigationContext.Parameters.GetValue<string>("userName");
            }

            // 检查参数集合中是否包含userEmail参数
            if (navigationContext.Parameters.ContainsKey("userEmail"))
            {
                // 从参数中获取userEmail的值并设置到UserEmail属性
                this.UserEmail = navigationContext.Parameters.GetValue<string>("userEmail");
            }

            // 检查参数集合中是否包含userAge参数
            if (navigationContext.Parameters.ContainsKey("userAge"))
            {
                // 从参数中获取userAge的值并设置到UserAge属性
                this.UserAge = navigationContext.Parameters.GetValue<int>("userAge");
            }

            // 构建导航参数显示信息
            this.NavigationParameter = $"接收到参数: 姓名={this.UserName}, 邮箱={this.UserEmail}, 年龄={this.UserAge}";
        }
    }
}
