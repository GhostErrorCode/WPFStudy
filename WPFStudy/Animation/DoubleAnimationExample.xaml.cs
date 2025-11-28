using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPFStudy.Animation
{
    /// <summary>
    /// DoubleAnimationExample.xaml 的交互逻辑
    /// </summary>
    public partial class DoubleAnimationExample : Window
    {
        public DoubleAnimationExample()
        {
            InitializeComponent();
        }

        // 淡入按钮点击事件处理方法
        private void FadeInButton_Click(object sender, RoutedEventArgs e)
        {
            // 创建DoubleAnimation对象实例，用于控制透明度变化
            DoubleAnimation fadeInAnimation = new DoubleAnimation();

            // 设置动画起始值为0（完全透明）
            fadeInAnimation.From = 0.0;

            // 设置动画结束值为1（完全不透明）
            fadeInAnimation.To = 1.0;

            // 设置动画持续时间为2秒
            fadeInAnimation.Duration = new Duration(TimeSpan.FromSeconds(2));

            // 在文本块上开始动画，目标是OpacityProperty（透明度属性）
            FadeInTextBlock.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
        }
    }
}
