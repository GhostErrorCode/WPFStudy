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
    /// DoubleAnimationUsingKeyFramesExample.xaml 的交互逻辑
    /// </summary>
    public partial class DoubleAnimationUsingKeyFramesExample : Window
    {
        public DoubleAnimationUsingKeyFramesExample()
        {
            InitializeComponent();
        }

        // 弹跳动画按钮点击事件处理方法
        private void BounceAnimationButton_Click(object sender, RoutedEventArgs e)
        {
            // 创建使用关键帧的DoubleAnimation对象实例
            DoubleAnimationUsingKeyFrames bounceAnimation = new DoubleAnimationUsingKeyFrames();

            // 设置动画总持续时间为2秒
            bounceAnimation.Duration = new Duration(TimeSpan.FromSeconds(2));

            // 创建第一个关键帧：起始位置（顶部位置0），时间点0秒
            LinearDoubleKeyFrame keyFrame1 = new LinearDoubleKeyFrame();
            keyFrame1.Value = 0.0;  // 关键帧值：位置0
            keyFrame1.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0));  // 关键帧时间：0秒

            // 创建第二个关键帧：下落到底部（位置150），时间点0.5秒
            LinearDoubleKeyFrame keyFrame2 = new LinearDoubleKeyFrame();
            keyFrame2.Value = 150.0;  // 关键帧值：位置150
            keyFrame2.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.5));  // 关键帧时间：0.5秒

            // 创建第三个关键帧：弹起一半（位置50），时间点1秒
            LinearDoubleKeyFrame keyFrame3 = new LinearDoubleKeyFrame();
            keyFrame3.Value = 50.0;  // 关键帧值：位置50
            keyFrame3.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1));  // 关键帧时间：1秒

            // 创建第四个关键帧：再次下落（位置120），时间点1.5秒
            LinearDoubleKeyFrame keyFrame4 = new LinearDoubleKeyFrame();
            keyFrame4.Value = 120.0;  // 关键帧值：位置120
            keyFrame4.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1.5));  // 关键帧时间：1.5秒

            // 创建第五个关键帧：最终位置（位置80），时间点2秒
            LinearDoubleKeyFrame keyFrame5 = new LinearDoubleKeyFrame();
            keyFrame5.Value = 80.0;  // 关键帧值：位置80
            keyFrame5.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(2));  // 关键帧时间：2秒

            // 将所有关键帧添加到动画的关键帧集合中
            bounceAnimation.KeyFrames.Add(keyFrame1);
            bounceAnimation.KeyFrames.Add(keyFrame2);
            bounceAnimation.KeyFrames.Add(keyFrame3);
            bounceAnimation.KeyFrames.Add(keyFrame4);
            bounceAnimation.KeyFrames.Add(keyFrame5);

            // 在矩形的Canvas.Top属性上开始弹跳动画
            BouncingRectangle.BeginAnimation(Canvas.TopProperty, bounceAnimation);
        }
    }
}
