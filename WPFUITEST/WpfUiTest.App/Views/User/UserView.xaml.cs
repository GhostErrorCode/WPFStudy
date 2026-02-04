using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Wpf.Ui.Controls;
using WpfUiTest.App.ViewModels.User;

namespace WpfUiTest.App.Views.User
{
    /// <summary>
    /// UserView.xaml 的交互逻辑
    /// </summary>
    public partial class UserView : FluentWindow
    {
        // 依赖注入获取对应ViewModel
        public UserView(UserViewModel userViewModel)
        {
            InitializeComponent();

            this.DataContext = userViewModel;
        }
    }
}
