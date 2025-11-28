using System;
using System.Collections.Generic;
using System.Drawing;
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
    /// ResourcesBasics.xaml 的交互逻辑
    /// </summary>
    public partial class ResourcesBasics : Window
    {
        public ResourcesBasics()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Resources["SolidColor"] = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 255));

            // 获取资源
            var style = App.Current.FindResource("DefaultButtonStyle");
        }
    }
}
