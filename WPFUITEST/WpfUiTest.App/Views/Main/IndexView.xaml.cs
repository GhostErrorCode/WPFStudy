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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfUiTest.App.ViewModels.Main;

namespace WpfUiTest.App.Views.Main
{
    /// <summary>
    /// IndexView.xaml 的交互逻辑
    /// </summary>
    public partial class IndexView : Page
    {
        public IndexView(IndexViewModel indexViewModel)
        {
            InitializeComponent();

            // 获取VM
            this.DataContext = indexViewModel;
        }
    }
}
