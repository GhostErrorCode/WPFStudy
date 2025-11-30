using PrismDialog.ViewModels;
using PrismDialog.Views;
using System.Configuration;
using System.Data;
using System.Windows;

namespace PrismDialog
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    /// 
    // 应用程序入口点，继承自PrismApplication以使用Prism框架
    public partial class App : PrismApplication
    {
        /// <summary>
        /// 创建Shell主窗口，应用程序启动时调用
        /// </summary>
        /// <returns>返回主窗口实例</returns>
        protected override Window CreateShell()
        {
            // 主窗口实例
            // 从容器中解析主窗口实例
            return Container.Resolve<MainWindow>();
        }

        /// <summary>
        /// 注册类型到依赖注入容器，配置应用程序服务
        /// </summary>
        /// <param name="containerRegistry">容器注册器，用于注册类型</param>
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // 注册View、ViewModel、Dialog
            // 注册确认对话框，使用名称"ConfirmationDialog"进行标识
            // 注册对话框，第一个参数是对话框的注册名称，第二个参数是对话框的视图类型
            containerRegistry.RegisterDialog<ConfirmDialog, ConfirmDialogViewModel>("ConfirmDialog");
        }
    }

}
