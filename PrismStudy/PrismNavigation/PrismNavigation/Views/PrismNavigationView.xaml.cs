using PrismNavigation.ViewModels;
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

namespace PrismNavigation.Views
{
    /// <summary>
    /// PrismNavigationView.xaml 的交互逻辑
    /// </summary>
    public partial class PrismNavigationView : Window
    {
        /// <summary>
        /// MainWindow 构造函数
        /// 通过依赖注入接收区域管理器和视图模型实例
        /// </summary>
        /// <param name="regionManager">区域管理器实例，用于导航管理</param>
        /// <param name="mainViewModel">主视图模型实例，用于数据绑定</param>

        private readonly IRegionManager _regionManager;

        public PrismNavigationView(IRegionManager regionManager, PrismNavigationMainViewModel mainViewModel)
        {
            // 调用InitializeComponent方法初始化XAML中定义的组件
            InitializeComponent();

            // 设置窗口的数据上下文为主视图模型实例
            this.DataContext = mainViewModel;
            this._regionManager = regionManager;
        }
    }
}
