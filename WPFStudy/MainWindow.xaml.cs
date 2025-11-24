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

// 以下using为自己的代码
using WPFStudy.CodeFile;

using WPFStudy.LayoutBasics;  // 布局基础
using WPFStudy.StyleBasics;  // 样式基础
using WPFStudy.Template;  // 模板 - 控件/数据模板


namespace WPFStudy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CodeFiles.CodeFileOutput();  // 输出代码说明和代码文件路径

            /********** 需要调用的代码写在下面 **********/
            this.Hide();
            new Template.ControlTemplate().Show();
        }
    }
}