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
using Wpf.Ui.Abstractions.Controls;
using WpfUiTest.App.ViewModels.Main;

namespace WpfUiTest.App.Views.Main
{
    /// <summary>
    /// SettingsView.xaml 的交互逻辑
    /// </summary>
    public partial class SettingsView : INavigableView<SettingsViewModel>
    {
        // 依赖注入获取对应ViewModel
        public SettingsView(SettingsViewModel settingsViewModel)
        {
            InitializeComponent();

            // 获取ViewModel
            this.DataContext = settingsViewModel;
            ViewModel = settingsViewModel;
        }

        public SettingsViewModel ViewModel { get; }
    }
}
