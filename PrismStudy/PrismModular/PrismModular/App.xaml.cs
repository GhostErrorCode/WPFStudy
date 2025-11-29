using PrismModular.Views;
using System.Configuration;
using System.Data;
using System.Windows;

namespace PrismModular
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>

    // 这个方法负责创建应用程序的主窗口
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            // 通过依赖注入容器解析并返回主窗口实例
            // Container是Prism提供的依赖注入容器
            return Container.Resolve<PrismModularView>();
        }

        // 这个方法用于注册应用程序中使用的各种类型（服务、视图等）
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // 这里暂时为空，后面我们会在这里注册视图和服务
            // containerRegistry是容器注册接口，用于注册类型到依赖注入容器中[citation:2]
            // containerRegistry.RegisterForNavigation<>("");

        }

        // 重写方法
        // ConfigureModuleCatalog 方法是 Prism 框架中用于配置模块目录的核心方法，它定义了应用程序如何发现和加载模块
        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            // 注册模块
            // 方法1: 直接添加模块类型（适用于模块在同一解决方案中）
            // 主项目必须引用模块项目（强引用）
            // moduleCatalog.AddModule<ModuleAProfile>();
            // moduleCatalog.AddModule<ModuleBProfile>();

            // 方法2: 通过配置文件添加模块（适用于模块在外部程序集）
            // moduleCatalog.AddModule(new ModuleInfo()
            // {
            //     ModuleName = "CustomerModule",
            //     ModuleType = typeof(CustomerModule.CustomerModule).AssemblyQualifiedName,
            //     InitializationMode = InitializationMode.WhenAvailable
            // });
            base.ConfigureModuleCatalog(moduleCatalog);
        }

        // 3. 目录扫描方式（动态加载DLL）
        protected override IModuleCatalog CreateModuleCatalog()
        {
            return new DirectoryModuleCatalog() { ModulePath = @".\Modules" };
        }
    }

}
