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
    /// EasingFunctionExample.xaml 的交互逻辑
    /// </summary>
    public partial class EasingFunctionExample : Window
    {
        public EasingFunctionExample()
        {
            InitializeComponent();
        }

        // 按钮点击事件处理方法
        private void BounceMoveButton_Click(object sender, RoutedEventArgs e)
        {
            // 从资源中获取故事板
            System.Windows.Media.Animation.Storyboard storyboard =
                (System.Windows.Media.Animation.Storyboard)this.Resources["BounceAnimation"];

            // 开始动画
            storyboard.Begin(MovingRectangle);
        }


        // 缓动函数（Easing Functions）用于使动画更加自然:
        // 例如模拟真实世界的物理效果（如弹跳、加速、减速等）。WPF提供了一系列内置的缓动函数，你可以将它们应用于动画。

        // WPF提供了多种缓动函数，以下是一些常用的缓动函数：
        /*
        BackEase：在动画开始之前稍后退一点，然后开始动画。
        BounceEase：创建弹跳效果。
        CircleEase：使用圆函数创建加速或减速的动画。
        CubicEase：使用三次函数创建加速或减速的动画。
        ElasticEase：创建类似于弹簧振荡的动画。
        ExponentialEase：使用指数函数创建加速或减速的动画。
        PowerEase：使用幂函数创建加速或减速的动画，可以指定幂次。
        QuadraticEase：使用二次函数创建加速或减速的动画。
        QuarticEase：使用四次函数创建加速或减速的动画。
        QuinticEase：使用五次函数创建加速或减速的动画。
        SineEase：使用正弦函数创建加速或减速的动画。
        */

        // 每个缓动函数都有一些属性可以调整，并且都有一个EasingMode属性，它可以是：
        /*
        EaseIn：缓动效果在动画开始时明显。
        EaseOut：缓动效果在动画结束时明显。
        EaseInOut：缓动效果在开始和结束时都明显。
        */


        /*
        下面，我们将介绍WPF中内置的缓动函数类型，它们都可以用在XAML中而无需编写代码。

        缓动函数通常用于以下类型的动画：
            DoubleAnimation
            DoubleAnimationUsingKeyFrames（关键帧动画中的缓动关键帧）
            ColorAnimation
            PointAnimation
            等（只要动画的属性是数值类型或可以使用插值的类型）

        以下是WPF中内置的缓动函数列表：
            BackEase：在动画开始之前先稍微后退，然后再开始动画。
            BounceEase：创建弹跳效果。
            CircleEase：使用圆函数创建加速或减速的动画。
            CubicEase：使用三次函数创建加速或减速的动画。
            ElasticEase：创建类似于弹簧的振荡效果。
            ExponentialEase：使用指数函数创建加速或减速的动画。
            PowerEase：使用幂函数创建加速或减速的动画，可以指定幂次。
            QuadraticEase：使用二次函数创建加速或减速的动画。
            QuarticEase：使用四次函数创建加速或减速的动画。
            QuinticEase：使用五次函数创建加速或减速的动画。
            SineEase：使用正弦函数创建加速或减速的动画。

        此外，还有三个缓动函数模式：
            EaseIn：缓动效果在动画开始时明显。
            EaseOut：缓动效果在动画结束时明显。
            EaseInOut：缓动效果在开始和结束时都明显。

        每个缓动函数通常都有一些属性可以调整，例如：
            BackEase：Amplitude（后退的幅度）
            BounceEase：Bounces（弹跳次数）、Bounciness（弹跳程度）
            ElasticEase：Oscillations（振荡次数）、Springiness（弹性程度）
            PowerEase：Power（幂次，用于PowerEase，表示幂函数的指数）
        */
    }
}
