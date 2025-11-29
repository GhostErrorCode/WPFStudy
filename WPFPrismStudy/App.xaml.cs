using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Windows;
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
            Debug.WriteLine("Home视图已注册到容器");
        }

        protected override void OnInitialized()
        {
            // 输出调试信息，标记OnInitialized方法的开始执行
            // 这有助于我们在输出窗口中看到代码执行的顺序
            Debug.WriteLine("=== OnInitialized开始 ===");

            // 调用基类(PrismApplication)的OnInitialized方法
            // 这是最关键的一步！这个方法会：
            // 1. 调用CreateShell()创建主窗口
            // 2. 显示主窗口(设置Application.Current.MainWindow并调用Show())
            // 3. 完成Prism框架的核心初始化工作
            // 如果不调用base.OnInitialized()，主窗口不会显示，整个应用会卡住
            base.OnInitialized();

            // 获取当前应用程序的主窗口实例，并转换为具体的MainWindow类型
            // Application.Current.MainWindow在base.OnInitialized()中被设置
            // 这里需要强制转换是因为Current.MainWindow是Window基类类型
            MainWindow mainWindow = (MainWindow)Current.MainWindow;

            // 从依赖注入容器中解析(获取)IRegionManager服务实例
            // RegionManager是Prism中负责管理区域和导航的核心服务
            // Container是PrismApplication提供的依赖注入容器
            IRegionManager regionManager = Container.Resolve<IRegionManager>();

            // 使用RegionManager请求在"MainRegion"区域中导航到"Home"视图
            // "MainRegion"必须与MainWindow.xaml中定义的RegionName一致
            // "Home"必须与RegisterTypes中注册的视图名称一致
            // 这行代码会让HomeView显示在MainRegion区域中
            regionManager.RequestNavigate("MainRegion", "Home");

            // 输出调试信息，确认导航请求已完成
            // 注意：这只表示导航请求已发送，不保证视图已完全加载
            Debug.WriteLine("导航到Home完成");

            // 输出调试信息，标记OnInitialized方法执行结束
            Debug.WriteLine("=== OnInitialized结束 ===");
        }
    }
}
