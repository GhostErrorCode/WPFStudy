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

namespace WpfUiTest.Controls.Controls.LoadingOverlay
{
    /// <summary>
    /// LoadingRing.xaml 的交互逻辑
    /// </summary>
    public partial class LoadingRing : UserControl
    {
        // ==================== 依赖属性定义 ====================

        /// <summary>
        /// 是否正在加载。true 时显示加载动画，false 时隐藏。
        /// </summary>
        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register(
                nameof(IsLoading),
                typeof(bool),
                typeof(LoadingRing),
                new PropertyMetadata(false)); // 默认不显示

        /// <summary>
        /// 加载时显示的文字，如“正在加载...”。
        /// </summary>
        public static readonly DependencyProperty LoadingTextProperty =
            DependencyProperty.Register(
                nameof(LoadingText),
                typeof(string),
                typeof(LoadingRing),
                new PropertyMetadata("加载中..."));


        // ==================== 属性包装器 ====================

        public bool IsLoading
        {
            get => (bool)GetValue(IsLoadingProperty);
            set => SetValue(IsLoadingProperty, value);
        }

        public string LoadingText
        {
            get => (string)GetValue(LoadingTextProperty);
            set => SetValue(LoadingTextProperty, value);
        }

        public LoadingRing()
        {
            InitializeComponent();
        }
    }
}
