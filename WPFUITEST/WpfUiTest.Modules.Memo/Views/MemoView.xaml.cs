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
using WpfUiTest.Modules.Memo.ViewModels;

namespace WpfUiTest.Modules.Memo.Views
{
    /// <summary>
    /// MemoView.xaml 的交互逻辑
    /// </summary>
    public partial class MemoView : Page
    {
        public MemoView(MemoViewModel memoViewModel)
        {
            InitializeComponent();

            // 依赖注入VM
            this.DataContext = memoViewModel;
        }
    }
}
