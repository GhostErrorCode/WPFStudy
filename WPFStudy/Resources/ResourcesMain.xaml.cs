using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPFStudy.Resources
{
    /// <summary>
    /// ResourcesMain.xaml 的交互逻辑
    /// </summary>
    public partial class ResourcesMain : Window
    {
        public ResourcesMain()
        {
            InitializeComponent();

            // 示例1：在代码中查找和使用字符串资源
            string applicationTitle = (string)this.FindResource("ApplicationTitle");

            // 示例2：在代码中查找和使用画刷资源
            SolidColorBrush primaryBrush = (SolidColorBrush)this.FindResource("PrimaryBrush");

            // 示例3：在代码中查找和使用数值资源
            double smallFontSize = (double)this.FindResource("SmallFontSize");

            // 示例4：安全地查找资源（如果资源不存在返回null）
            object welcomeMessage = this.TryFindResource("WelcomeMessage");

            // 示例5：在代码中添加新资源
            this.Resources.Add("NewStringResource", "这是在代码中添加的字符串");

            // 示例6：在代码中移除资源
            this.Resources.Remove("NewStringResource");
        }
    }
}
