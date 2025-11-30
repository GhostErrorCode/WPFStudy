using System;
using System.Collections.Generic;
using System.Text;
using Prism.Commands;  // 引入Prism命令相关类
using Prism.Mvvm;  // 引入Prism MVVM相关类
using System.Windows;
using System.Diagnostics;  // 引入WPF基础类

namespace PrismNavigation.ViewModels
{
    /// <summary>
    /// MainViewModel 主视图模型类
    /// 这个类处理主视图的业务逻辑，包括导航控制和日志查看
    /// </summary>
    public class PrismNavigationMainViewModel : BindableBase  // 继承BindableBase基类
    {
        /// <summary>
        /// 区域管理器实例，用于管理区域导航
        /// </summary>
        private readonly IRegionManager _regionManager;

        /// <summary>
        /// 导航服务实例，用于管理导航历史
        /// </summary>
        private IRegionNavigationService _navigationService;

        /// <summary>
        /// MainViewModel 构造函数
        /// 通过依赖注入接收区域管理器实例
        /// </summary>
        /// <param name="regionManager">区域管理器实例，用于导航管理</param>
        public PrismNavigationMainViewModel(IRegionManager regionManager)
        {
            Debug.WriteLine("MainViewModel实例: " + this.GetHashCode());
            // 将传入的区域管理器实例赋值给私有字段
            this._regionManager = regionManager;

            // 初始化导航到用户视图命令，绑定到ExecuteNavigateToUserCommand方法
            this.NavigateToUserCommand = new DelegateCommand(this.ExecuteNavigateToUserCommand);

            // 初始化后退命令，绑定到ExecuteGoBackCommand方法和CanExecuteGoBackCommand方法
            this.GoBackCommand = new DelegateCommand(this.ExecuteGoBackCommand, this.CanExecuteGoBackCommand);

            // 初始化前进命令，绑定到ExecuteGoForwardCommand方法和CanExecuteGoForwardCommand方法
            this.GoForwardCommand = new DelegateCommand(this.ExecuteGoForwardCommand, this.CanExecuteGoForwardCommand);

            // 初始化查看日志命令，绑定到ExecuteViewJournalCommand方法
            // this.ViewJournalCommand = new DelegateCommand(this.ExecuteViewJournalCommand);

            // 获取主区域实例
            // IRegion mainRegion = this._regionManager.Regions["MainRegion"];
            // 订阅导航完成事件，当导航完成时更新命令状态
            // mainRegion.NavigationService.Navigated += this.OnNavigated;
        }


        /// <summary>
        /// 获取导航服务（延迟加载）
        /// </summary>
        private IRegionNavigationService GetNavigationService()
        {
            if (this._navigationService == null)
            {
                try
                {
                    // 在需要的时候才获取区域
                    if (this._regionManager.Regions.ContainsRegionWithName("MainRegion"))
                    {
                        IRegion mainRegion = this._regionManager.Regions["MainRegion"];
                        this._navigationService = mainRegion.NavigationService;

                        // 订阅导航事件
                        this._navigationService.Navigated += this.OnNavigated;

                        System.Diagnostics.Debug.WriteLine("成功获取导航服务并订阅事件");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("MainRegion 区域不存在");
                    }
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"获取导航服务失败: {ex.Message}");
                }
            }

            return this._navigationService;
        }


        /// <summary>
        /// NavigateToUserCommand 导航到用户视图命令属性
        /// 公开的只读属性，用于绑定到UI的导航按钮
        /// </summary>
        public DelegateCommand NavigateToUserCommand { get; private set; }

        /// <summary>
        /// GoBackCommand 后退命令属性
        /// 公开的只读属性，用于绑定到UI的后退按钮
        /// </summary>
        public DelegateCommand GoBackCommand { get; private set; }

        /// <summary>
        /// GoForwardCommand 前进命令属性
        /// 公开的只读属性，用于绑定到UI的前进按钮
        /// </summary>
        public DelegateCommand GoForwardCommand { get; private set; }

        /// <summary>
        /// ViewJournalCommand 查看日志命令属性
        /// 公开的只读属性，用于绑定到UI的查看日志按钮
        /// </summary>
        public DelegateCommand ViewJournalCommand { get; private set; }

        /// <summary>
        /// ExecuteNavigateToUserCommand 执行导航到用户视图命令的方法
        /// 当用户点击导航到用户视图按钮时调用此方法
        /// </summary>
        private void ExecuteNavigateToUserCommand()
        {
            // 创建导航参数对象
            NavigationParameters parameters = new NavigationParameters();
            // 添加userName参数，值为"张三"
            parameters.Add("userName", "张三");
            // 添加userEmail参数，值为"zhangsan@example.com"
            parameters.Add("userEmail", "zhangsan@example.com");
            // 添加userAge参数，值为30
            parameters.Add("userAge", 30);

            // 使用区域管理器请求导航到UserView，并传递参数
            this._regionManager.RequestNavigate("MainRegion", "UserView", parameters);
        }

        /// <summary>
        /// ExecuteGoBackCommand 执行后退命令的方法
        /// 当用户点击后退按钮时调用此方法
        /// </summary>
        private void ExecuteGoBackCommand()
        {
            // 获取主区域的导航服务
            IRegionNavigationService navigationService = this.GetNavigationService();
            // 检查导航日志是否可以后退
            if (navigationService.Journal.CanGoBack)
            {
                // 执行后退操作，导航到历史记录中的上一个视图
                navigationService.Journal.GoBack();
            }
        }

        /// <summary>
        /// CanExecuteGoBackCommand 判断是否可以执行后退命令的方法
        /// 这个方法用于控制后退按钮的可用状态
        /// </summary>
        /// <returns>返回true表示可以后退，false表示不能后退</returns>
        private bool CanExecuteGoBackCommand()
        {
            // 获取主区域的导航服务
            IRegionNavigationService navigationService = this.GetNavigationService();
            // 返回导航日志是否可以后退
            return navigationService.Journal.CanGoBack;
        }

        /// <summary>
        /// ExecuteGoForwardCommand 执行前进命令的方法
        /// 当用户点击前进按钮时调用此方法
        /// </summary>
        private void ExecuteGoForwardCommand()
        {
            // 获取主区域的导航服务
            IRegionNavigationService navigationService = this.GetNavigationService();
            // 检查导航日志是否可以前进
            if (navigationService.Journal.CanGoForward)
            {
                // 执行前进操作，导航到历史记录中的下一个视图
                navigationService.Journal.GoForward();
            }
        }

        /// <summary>
        /// CanExecuteGoForwardCommand 判断是否可以执行前进命令的方法
        /// 这个方法用于控制前进按钮的可用状态
        /// </summary>
        /// <returns>返回true表示可以前进，false表示不能前进</returns>
        private bool CanExecuteGoForwardCommand()
        {
            // 获取主区域的导航服务
            IRegionNavigationService navigationService = this.GetNavigationService();
            // 返回导航日志是否可以前进
            return navigationService.Journal.CanGoForward;
        }

        /// <summary>
        /// ExecuteViewJournalCommand 执行查看日志命令的方法
        /// 当用户点击查看导航日志按钮时调用此方法
        /// </summary>
        /*private void ExecuteViewJournalCommand()
        {
            // 获取主区域的导航服务
            IRegionNavigationService navigationService = this._regionManager.Regions["MainRegion"].NavigationService;
            // 构建日志信息字符串，显示后退栈和前进栈的数量
            string journalInfo = $"后退栈数量: {navigationService.Journal.BackStack.Count}\n" +
                               $"前进栈数量: {navigationService.Journal.ForwardStack.Count}";

            // 显示消息框，展示导航日志信息
            MessageBox.Show(journalInfo, "导航日志信息", MessageBoxButton.OK, MessageBoxImage.Information);
        }*/

        /// <summary>
        /// OnNavigated 导航完成时的事件处理方法
        /// 当区域导航完成时调用此方法更新命令状态
        /// </summary>
        /// <param name="sender">事件发送者，通常是导航服务</param>
        /// <param name="e">导航事件参数，包含导航相关信息</param>
        private void OnNavigated(object sender, RegionNavigationEventArgs e)
        {
            // 将发送者转换为导航服务实例
            this._navigationService = sender as IRegionNavigationService;
            // 这行代码的作用是：
            // 1. 从事件发送者中提取导航服务实例
            // 2. 保存到类的私有字段中，以便其他地方使用
            // 3. 确保我们有正确的导航服务引用

            // 触发后退命令的CanExecuteChanged事件，更新按钮状态
            this.GoBackCommand.RaiseCanExecuteChanged();
            // 触发前进命令的CanExecuteChanged事件，更新按钮状态
            this.GoForwardCommand.RaiseCanExecuteChanged();
        }
    }
}
