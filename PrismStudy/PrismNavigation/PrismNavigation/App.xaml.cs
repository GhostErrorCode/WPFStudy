using Prism.Ioc;  // 引入Prism依赖注入容器相关类
using Prism.Modularity;
using PrismNavigation.ViewModels;
using PrismNavigation.Views;  // 引入Prism模块化相关类
using System.Configuration;
using System.Data;
using System.Windows;
using UserControllerModule.ViewModels;
using UserControllerModule.Views;  // 引入WPF基础类

namespace PrismNavigation
{
    /// <summary>
    /// App 应用程序类
    /// 继承自PrismApplication，提供Prism框架的启动和初始化功能
    /// </summary>
    public partial class App : PrismApplication  // 部分类，继承自PrismApplication
    {
        /// <summary>
        /// CreateShell 创建Shell窗口的方法
        /// Prism框架在启动时会调用此方法创建主窗口
        /// </summary>
        /// <returns>返回应用程序的主窗口实例</returns>
        protected override Window CreateShell()
        {
            // 从依赖注入容器中解析MainWindow实例并返回
            return this.Container.Resolve<PrismNavigationView>();
        }

        /// <summary>
        /// RegisterTypes 注册类型方法
        /// 在这个方法中注册应用程序中需要依赖注入的类型
        /// </summary>
        /// <param name="containerRegistry">容器注册器，用于注册类型</param>
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // 注册MainView和MainViewModel的导航映射
            // "MainView"是导航时使用的名称，MainView是视图类型，MainViewModel是视图模型类型
            containerRegistry.RegisterForNavigation<PrismNavigationMainView, PrismNavigationMainViewModel>("PrismNavigationMainView");
            containerRegistry.RegisterForNavigation<UserView, UserViewModel>("UserView");

            // 注册主窗口
            containerRegistry.Register<PrismNavigationView>();
        }

        /// <summary>
        /// ConfigureModuleCatalog 配置模块目录的方法
        /// 在这个方法中注册应用程序需要加载的模块
        /// </summary>
        /// <param name="moduleCatalog">模块目录，用于管理模块</param>
        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            // 添加用户控制器模块到模块目录中
            // 这样Prism框架在启动时会自动加载这个模块
            moduleCatalog.AddModule<UserControllerModule.UserControllerModule>();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            // 确保在应用初始化完成后执行初始导航
            var regionManager = Container.Resolve<IRegionManager>();

            // 初始导航到主视图
            regionManager.RequestNavigate("MainRegion", "PrismNavigationMainView");
        }
    }
}
