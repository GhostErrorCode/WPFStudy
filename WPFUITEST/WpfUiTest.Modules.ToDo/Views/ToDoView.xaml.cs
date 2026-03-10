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
using WpfUiTest.Modules.ToDo.ViewModels;

namespace WpfUiTest.Modules.ToDo.Views
{
    /// <summary>
    /// ToDoView.xaml 的交互逻辑
    /// </summary>
    public partial class ToDoView : Page
    {
        public ToDoView(ToDoViewModel toDoViewModel)
        {
            InitializeComponent();

            // 依赖注入获取ViewModel
            this.DataContext = toDoViewModel;
        }
    }
}
