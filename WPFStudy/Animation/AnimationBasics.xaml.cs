using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// AnimationBasics.xaml 的交互逻辑
    /// </summary>
    public partial class AnimationBasics : Window
    {
        public AnimationBasics()
        {
            InitializeComponent();
        }

        private void btu_Click(object sender, RoutedEventArgs e)
        {
            // 新建一个DoubleAnimation动画
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            // 动画初始值
            doubleAnimation.From = btu.Width;
            // 动画结束值
            doubleAnimation.To = btu.Width - 300;
            // 动画初始值和动画结束值可以简写为 doubleAnimation.By = -300;
            // 动画的持续时间
            doubleAnimation.Duration = TimeSpan.FromSeconds(1);
            // 还原成动画前
            doubleAnimation.AutoReverse = true;
            // 执行次数
            // doubleAnimation.RepeatBehavior = RepeatBehavior.Forever;  // 永远执行
            doubleAnimation.RepeatBehavior = new RepeatBehavior(5);  // 执行N次
            // 订阅动画完成的事件(必须在启动之前订阅)
            doubleAnimation.Completed += AnimationCompleted;

            // 启动动画
            btu.BeginAnimation(Button.WidthProperty, doubleAnimation);
        }

        private void AnimationCompleted(object? sender, EventArgs e)
        {
            // Debug.WriteLine("运行了");
            btu.Content = "动画完成!";
        }
    }

    // 基本类型动画
    // 1.DoubleAnimation - 最常用的动画
    /*
    // 用于动画化double类型的属性，如Opacity、Width、Height等
    DoubleAnimation animation = new DoubleAnimation();
    animation.From = 0;           // 起始值
    animation.To = 1;             // 结束值
    animation.Duration = TimeSpan.FromSeconds(2);  // 持续时间
    */

    // 2.ColorAnimation
    /*
    // 用于动画化Color类型的属性
    ColorAnimation colorAnimation = new ColorAnimation();
    colorAnimation.From = Colors.Red;
    colorAnimation.To = Colors.Blue;
    colorAnimation.Duration = TimeSpan.FromSeconds(1);
    */

    // 3.PointAnimation
    /*
    // 用于动画化Point类型的属性
    PointAnimation pointAnimation = new PointAnimation();
    pointAnimation.From = new Point(0, 0);
    pointAnimation.To = new Point(100, 100);
    pointAnimation.Duration = TimeSpan.FromSeconds(1);
    */


    // 关键帧动画
    // 1.DoubleAnimationUsingKeyFrames
    /*
    DoubleAnimationUsingKeyFrames keyFrameAnimation = new DoubleAnimationUsingKeyFrames();
    keyFrameAnimation.Duration = TimeSpan.FromSeconds(3);
    // 线性关键帧
    keyFrameAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(0, TimeSpan.FromSeconds(0)));
    keyFrameAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(100, TimeSpan.FromSeconds(1)));
    keyFrameAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(50, TimeSpan.FromSeconds(2)));
    keyFrameAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(200, TimeSpan.FromSeconds(3)));
    */


    // 常用动画属性
    /*
    DoubleAnimation animation = new DoubleAnimation
    {
        From = 0,                    // 起始值（可选）
        To = 100,                    // 结束值
        By = 50,                     // 相对变化值（与From/To互斥）
        Duration = TimeSpan.FromSeconds(2),           // 持续时间
        BeginTime = TimeSpan.FromSeconds(1),          // 延迟开始时间
        SpeedRatio = 1.5,                             // 速度比率
        AutoReverse = true,                           // 是否自动反转
        RepeatBehavior = new RepeatBehavior(3),       // 重复3次
                                                      // RepeatBehavior = RepeatBehavior.Forever,   // 无限重复
        FillBehavior = FillBehavior.HoldEnd,          // 动画结束后保持状态
        EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
    };
    */
    }
