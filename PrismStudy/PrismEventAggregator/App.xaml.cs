using PrismEventAggregator.Views;
using PrismEventAggregator.ViewModels;

using System.Configuration;
using System.Data;
using System.Windows;

namespace PrismEventAggregator
{
    /// <summary>
    /// 应用程序类
    /// 继承自PrismApplication，负责应用程序的启动和初始化
    /// </summary>
    public partial class App : PrismApplication
    {
        /// <summary>
        /// 创建应用程序主窗口
        /// </summary>
        /// <returns>返回主窗口实例</returns>
        protected override Window CreateShell()
        {
            // 创建并显示主窗口
            return this.Container.Resolve<MainWindow>();
        }

        /// <summary>
        /// 注册依赖类型
        /// 在这个方法中配置依赖注入容器
        /// </summary>
        /// <param name="containerRegistry">容器注册器</param>
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // 注册SubscriberView及其对应的ViewModel
            containerRegistry.RegisterForNavigation<SubscriberView, SubscriberViewModel>("SubscriberView");
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            // 获得区域管理器
            IRegionManager regionManager = Container.Resolve<IRegionManager>();
            // 导航到SubscriberView
            regionManager.RequestNavigate("MainRegion", "SubscriberView");
        }
    }
}

// View与ViewModel生命周期说明
// 1.每次打开View时候会实例化一次ViewModel
// 2.View关闭时会回收ViewModel实例和View实例
// 3.等iew再次被打开时会实例化ViewModel和View
// 4.期间按钮绑定在ViewModel的话，会自动调用对应方法而不是再次实例化ViewModel

// 图解
/* 1. View 打开时的流程
// 当 SubscriberView 被创建时：
SubscriberView view = new SubscriberView();  // View 实例化
    ↓
// Prism ViewModelLocator 自动工作：
SubscriberViewModel viewModel = container.Resolve<SubscriberViewModel>();  // ViewModel 实例化
    ↓
view.DataContext = viewModel;  // 绑定
// 每个 View 实例对应一个 ViewModel 实例
*/

/* View 关闭时的流程
// 当 SubscriberView 关闭时：
view.Close();  // View 关闭
    ↓
// 如果没有其他引用，垃圾回收器可以回收：
view = null;      // View 可被回收
viewModel = null; // ViewModel 可被回收
*/

/* 3.按钮操作时的流程
// 当按钮被点击时：
button.Click → Command.Execute() → ViewModel.Method()
// 只是调用已存在的 ViewModel 实例的方法，不会创建新实例
*/