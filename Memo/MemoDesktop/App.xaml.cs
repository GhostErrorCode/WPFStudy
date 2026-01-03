using MemoDesktop.Constants;
using MemoDesktop.Services.Implements;
using MemoDesktop.Services.Interfaces;
using MemoDesktop.ViewModels;
using MemoDesktop.ViewModels.Dialogs;
using MemoDesktop.Views;
using MemoDesktop.Views.Dialogs;
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

        // OnInitialized() 是 PrismApplication 的生命周期方法，在应用程序初始化完成后调用。这是 Prism 框架推荐的执行启动逻辑的地方。
        protected override void OnInitialized()
        {
            base.OnInitialized();

            // 配置默认首页
            Container.Resolve<IRegionManager>().RequestNavigate(RegionNames.MainViewRegionName,"IndexView");
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

            // 注册服务
            containerRegistry.RegisterSingleton<IMemoApiService, MemoApiService>();
            containerRegistry.RegisterSingleton<IToDoApiService, ToDoApiService>();

            // 注册对话框
            containerRegistry.RegisterDialog<AddToDoDialog, AddToDoDialogViewModel>("AddToDoDialog");
            containerRegistry.RegisterDialog<AddMemoDialog, AddMemoDialogViewModel>("AddMemoDialog");
            // ====================================== HttpClient注册开始 ========================================
            // 第一步：创建并配置HTTP客户端实例
            HttpClient httpClient = new HttpClient();

            // 配置HTTP客户端的基本设置
            // 注意：这里的地址需要与后端实际运行地址一致
            httpClient.BaseAddress = new Uri("https://localhost:7084/");

            // 设置超时时间（30秒）
            httpClient.Timeout = TimeSpan.FromSeconds(30);

            // 清除默认的Accept头
            httpClient.DefaultRequestHeaders.Accept.Clear();

            // 添加Accept头，指定接收JSON格式的响应
            httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            // 可选：添加自定义请求头
            httpClient.DefaultRequestHeaders.Add("X-Client", "MemoWpf");
            httpClient.DefaultRequestHeaders.Add("X-Client-Version", "1.0.0");

            // 第二步：将HTTP客户端注册为单例
            // 整个应用程序生命周期内只使用一个HttpClient实例
            containerRegistry.RegisterInstance<HttpClient>(httpClient);
            // ====================================== HttpClient注册结束 ========================================
        }
    }
}
