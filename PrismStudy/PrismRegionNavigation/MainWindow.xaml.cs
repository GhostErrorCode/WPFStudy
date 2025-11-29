using PrismRegionNavigation.Views;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PrismRegionNavigation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // 声明一个只读的容器扩展接口字段，用于依赖注入解析
        // IContainerExtension 是 Prism 框架提供的依赖注入容器接口
        // 通过这个容器可以解析（创建）注册过的类型实例
        // readonly 关键字表示这个字段只能在构造函数中赋值，之后不能修改
        private readonly IContainerExtension _container;

        // 声明一个只读的区域管理器接口字段
        // IRegionManager 是 Prism 框架中负责管理 UI 区域的接口
        // 区域是 Prism 中用于动态加载和切换视图的容器
        // 通过区域管理器可以在不同区域之间导航和切换视图
        private readonly IRegionManager _regionManager;

        // 构造函数，接收 IContainerExtension 和 IRegionManager 作为参数（通过依赖注入传入）
        // 当 Prism 创建 MainWindow 实例时，会自动注入这两个依赖项
        public MainWindow(IContainerExtension container, IRegionManager regionManager)
        {
            // 将传入的容器实例赋值给类的 _container 字段
            // 这样类的其他方法就可以使用这个容器来解析其他类型
            _container = container;

            // 将传入的区域管理器实例赋值给类的 _regionManager 字段
            // 这样类的其他方法就可以使用这个区域管理器来管理区域
            _regionManager = regionManager;

            // 调用 InitializeComponent 方法，初始化 XAML 中定义的 UI 组件
            // 这个方法是由 WPF 编译器自动生成的，它会：
            // 1. 解析 XAML 文件
            // 2. 创建 XAML 中定义的控件对象树
            // 3. 设置控件的属性和事件处理程序
            // 4. 将控件连接到代码中通过 x:Name 指定的字段
            InitializeComponent();

            // 订阅窗口加载完成事件
            // Loaded 事件在窗口已经完全加载并准备好显示时触发
            // 这里将 ShowView 方法注册为事件处理程序
            // 当窗口加载完成后，会自动调用 ShowView 方法
            this.Loaded += ShowView;
        }

        // 显示 PrismRegionMainView 窗口的方法，同时作为 Loaded 事件的处理程序
        // sender 参数指向触发事件的对象（这里是 MainWindow 本身）
        // e 参数包含事件的相关数据（这里是 RoutedEventArgs，包含路由事件信息）
        private void ShowView(object sender, RoutedEventArgs e)
        {
            // 通过依赖注入容器解析 PrismRegionMainView 窗口实例
            // _container.Resolve<T>() 方法会：
            // 1. 查找容器中注册的 PrismRegionMainView 类型
            // 2. 自动解析该类型的所有依赖项（如 ViewModel、服务等）
            // 3. 创建并返回一个完整的 PrismRegionMainView 实例
            // 这确保了新窗口的所有 Prism 功能（如区域管理、导航等）都能正常工作
            PrismRegionNavigationMainView newWindow = _container.Resolve<PrismRegionNavigationMainView>();

            // 将当前窗口的区域管理器设置给新窗口
            // RegionManager.SetRegionManager 是一个静态方法，它会：
            // 1. 将指定的区域管理器与目标窗口关联
            // 2. 确保新窗口中的区域能够被正确识别和管理
            // 3. 使新窗口能够使用 Prism 的区域功能
            // 这里使用的是当前 MainWindow 的区域管理器实例
            RegionManager.SetRegionManager(newWindow, _regionManager);

            // 注释中提到的另一种方法：创建新的区域管理器
            // 这行代码被注释掉了，但如果取消注释，它会：
            // 1. 创建一个新的、独立的区域管理器实例
            // 2. 这个新实例与原来的区域管理器没有关联
            // 3. 适用于希望新窗口有完全独立的区域管理场景
            // var newRegionManager = _regionManager.CreateRegionManager();
            // RegionManager.SetRegionManager(newWindow, newRegionManager);

            // 隐藏当前窗口（MainWindow）
            // this.Hide() 方法会将当前窗口设置为不可见，但不会关闭它
            // 窗口仍然在内存中运行，只是用户看不到
            // 这样做可以保留窗口状态，以便后续可能需要重新显示
            this.Hide();

            // 显示新解析的 PrismRegionMainView 窗口
            // newWindow.Show() 方法会将新窗口设置为可见状态
            // 用户现在可以看到并与之交互的是新窗口
            // 此时应用程序有两个窗口：一个隐藏的 MainWindow 和一个显示的 PrismRegionMainView
            newWindow.Show();

            // 注意：这里有一个潜在问题
            // 虽然新窗口显示了，但应用程序的主窗口仍然是原来的 MainWindow
            // 这意味着如果用户关闭新窗口，应用程序不会退出，因为主窗口（MainWindow）还在
            // 如果需要新窗口成为主窗口，应该添加：
            // Application.Current.MainWindow = newWindow;
        }
    }
}