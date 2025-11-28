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
    /// PointAnimationExample.xaml 的交互逻辑
    /// </summary>
    public partial class PointAnimationExample : Window
    {
        public PointAnimationExample()
        {
            InitializeComponent();
        }

        // 点动画按钮点击事件处理方法
        private void PointAnimationButton_Click(object sender, RoutedEventArgs e)
        {
            // 创建X方向移动的DoubleAnimation对象实例
            DoubleAnimation xAnimation = new DoubleAnimation();

            // 设置X方向动画起始值为当前位置（0）
            xAnimation.From = 0.0;

            // 设置X方向动画结束值为250
            xAnimation.To = 250.0;

            // 设置X方向动画持续时间为2秒
            xAnimation.Duration = new Duration(TimeSpan.FromSeconds(2));

            // 在平移变换的X属性上开始动画
            EllipseTranslateTransform.BeginAnimation(TranslateTransform.XProperty, xAnimation);

            // 创建Y方向移动的DoubleAnimation对象实例
            DoubleAnimation yAnimation = new DoubleAnimation();

            // 设置Y方向动画起始值为当前位置（0）
            yAnimation.From = 0.0;

            // 设置Y方向动画结束值为150
            yAnimation.To = 150.0;

            // 设置Y方向动画持续时间为2秒
            yAnimation.Duration = new Duration(TimeSpan.FromSeconds(2));

            // 在平移变换的Y属性上开始动画
            EllipseTranslateTransform.BeginAnimation(TranslateTransform.YProperty, yAnimation);
        }
    }
}
