using MemoDesktop.ViewModels;
using MemoDesktop.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Net.Http;
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
            containerRegistry.RegisterForNavigation<MemoMainView, MemoMainViewModel>("MemoMainView");

            // 注册首页、备忘录、待办事项、设置页面和视图模型，并将View和ViewModel绑定
            containerRegistry.RegisterForNavigation<IndexView, IndexViewModel>("IndexView");
            containerRegistry.RegisterForNavigation<MemoView, MemoViewModel>("MemoView");
            containerRegistry.RegisterForNavigation<SettingsView, SettingsViewModel>("SettingsView");
            containerRegistry.RegisterForNavigation<ToDoView, ToDoViewModel>("ToDoView");

            // 设置的个性化、系统设置、关于更多的页面和视图模型
            containerRegistry.RegisterForNavigation<SkinView, SkinViewModel>("SkinView");
            containerRegistry.RegisterForNavigation<AboutView, AboutViewModel>("AboutView");
        }
    }
}
