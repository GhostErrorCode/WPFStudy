using MemoDesktop.ViewModels;
using MemoDesktop.Views;
using System.Configuration;
using System.Data;
using System.Windows;

namespace MemoDesktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        // 程序的入口,需要返回一个Window窗口类型，负责创建应用程序的主窗口
        protected override Window CreateShell()
        {
            // 通过依赖注入容器解析并返回主窗口实例
            MemoMainView mainWindow = Container.Resolve<MemoMainView>();
            return mainWindow;
        }

        // 注册应用程序中使用的各种类型（服务、视图等）
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // 绑定主界面的View和ViewModel
            containerRegistry.RegisterForNavigation<MemoMainView, MemoMainViewModel>();
        }
    }
}
