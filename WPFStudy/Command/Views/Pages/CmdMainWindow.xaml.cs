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
using WPFStudy.Command.ViewModels.Pages;

namespace WPFStudy.Command.Views.Pages
{
    /// <summary>
    /// CmdMainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CmdMainWindow : Window
    {
        public CmdMainWindow()
        {
            InitializeComponent();

            // 载入上下文
            // 载入ViewModel
            this.DataContext = new CmdMainViewModel();
        }
    }
}
