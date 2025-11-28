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
    /// StoryboardExample.xaml 的交互逻辑
    /// </summary>
    public partial class StoryboardExample : Window
    {
        public StoryboardExample()
        {
            InitializeComponent();
        }

        // 组合动画按钮点击事件处理方法
        private void CompositeAnimationButton_Click(object sender, RoutedEventArgs e)
        {
            // 从窗口资源字典中获取故事板资源
            Storyboard compositeStoryboard = (Storyboard)this.Resources["CompositeAnimationStoryboard"];

            // 开始播放故事板中的所有动画
            compositeStoryboard.Begin();
        }
    }
}
