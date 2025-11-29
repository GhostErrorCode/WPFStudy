using System.Diagnostics;
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

namespace WPFPrismStudy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IRegionManager _regionManager;

        // 构造函数注入了IRegionManager，这是Prism的区域管理服务[citation:1]
        public MainWindow(IRegionManager regionManager)
        {
            InitializeComponent();
            _regionManager = regionManager;

            // 应用程序启动时，在MainRegion区域中导航到Home视图
            //_regionManager.RequestNavigate("MainRegion", "Home");
            Debug.WriteLine("导航成功");
        }
    }
}