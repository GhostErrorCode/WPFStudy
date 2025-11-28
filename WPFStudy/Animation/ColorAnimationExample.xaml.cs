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
    /// ColorAnimationExample.xaml 的交互逻辑
    /// </summary>
    public partial class ColorAnimationExample : Window
    {
        public ColorAnimationExample()
        {
            InitializeComponent();
        }

        // 颜色动画按钮点击事件处理方法
        private void ColorAnimationButton_Click(object sender, RoutedEventArgs e)
        {
            // 检查矩形填充是否为SolidColorBrush类型
            if (ColorAnimationRectangle.Fill is SolidColorBrush)
            {
                // 将矩形填充转换为SolidColorBrush类型
                // 这里rectangleBrush并不是创建一个新的画刷，而是获取对矩形当前Fill画刷的引用。它们指向内存中的同一个SolidColorBrush对象
                SolidColorBrush rectangleBrush = (SolidColorBrush)ColorAnimationRectangle.Fill;

                // 创建ColorAnimation对象实例，用于控制颜色变化
                ColorAnimation colorChangeAnimation = new ColorAnimation();

                // 设置动画开始颜色为红色
                colorChangeAnimation.From = Colors.Red;

                // 设置动画结束颜色为蓝色
                colorChangeAnimation.To = Colors.Blue;

                // 设置动画持续时间为1.5秒
                colorChangeAnimation.Duration = new Duration(TimeSpan.FromSeconds(1.5));

                // 在画刷上开始动画，目标是ColorProperty（颜色属性）
                rectangleBrush.BeginAnimation(SolidColorBrush.ColorProperty, colorChangeAnimation);
            }
            else
            {
                // 如果矩形填充不是SolidColorBrush，则创建一个新的
                SolidColorBrush newBrush = new SolidColorBrush(Colors.Red);

                // 这里是矩形的填充引用了新画刷
                ColorAnimationRectangle.Fill = newBrush;

                // 创建ColorAnimation对象实例
                ColorAnimation colorChangeAnimation = new ColorAnimation();

                // 设置动画开始颜色为红色
                colorChangeAnimation.From = Colors.Red;

                // 设置动画结束颜色为蓝色
                colorChangeAnimation.To = Colors.Blue;

                // 设置动画持续时间为1.5秒
                colorChangeAnimation.Duration = new Duration(TimeSpan.FromSeconds(1.5));

                // 在新画刷上开始动画
                newBrush.BeginAnimation(SolidColorBrush.ColorProperty, colorChangeAnimation);
            }
        }
    }
}
