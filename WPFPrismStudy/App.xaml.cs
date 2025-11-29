using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Windows;
using WPFPrismStudy.PrismRegion.Views;
using WPFPrismStudy.Views;

namespace WPFPrismStudy
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    /// 

    // 项目结构：
    // Views：存放所有视图文件（XAML文件）
    // ViewModels：存放所有视图模型文件（CS文件）
    // Models：存放数据模型
    // Services：存放业务逻辑服务

    // 安装了Prism.Wpf和Prism.Unity组件包
    // Prism.Wpf：Prism框架的核心功能，包含MVVM、导航、区域管理等
    // Prism.Unity：提供Unity容器支持，用于依赖注入
    public partial class App : PrismApplication
    {

        // PrismApplication：Prism应用的基类，它封装了应用程序的启动逻辑
        // CreateShell()：必须重写的方法，用于创建应用程序的主窗口（Shell）
        // Container.Resolve<MainWindow>()：通过依赖注入容器创建MainWindow实例，这样MainWindow也可以享受依赖注入的好处
        // RegisterTypes()：用于注册所有需要通过IoC容器管理的类型

        // 这个方法负责创建应用程序的主窗口
        protected override Window CreateShell()
        {
            // 通过依赖注入容器解析并返回主窗口实例
            // Container是Prism提供的依赖注入容器
            return Container.Resolve<MainWindow>();
        }

        // 这个方法用于注册应用程序中使用的各种类型（服务、视图等）
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // 这里暂时为空，后面我们会在这里注册视图和服务
            // containerRegistry是容器注册接口，用于注册类型到依赖注入容器中[citation:2]

            // 注册HomeView用于导航
            // "Home"是导航时使用的标识符
            containerRegistry.RegisterForNavigation<HomeView>("Home");

            // 注册PrismRegion中的View
            containerRegistry.RegisterForNavigation<PrismRegionUserControlA>("PrismRegionUserControlA");
            containerRegistry.RegisterForNavigation<PrismRegionUserControlB>("PrismRegionUserControlB");
            containerRegistry.RegisterForNavigation<PrismRegionUserControlC>("PrismRegionUserControlC");
        }
    }
}
