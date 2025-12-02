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

namespace MemoDesktop.Views
{
    /*
     * object sender, RoutedEventArgs e 是事件处理方法的两个标准参数。它们好比一个快递：
     * sender告诉你“谁寄出的”，
     * 而e是包裹本身，里面装着关于这次“邮寄”（事件）的所有具体信息。
     * */
    public partial class MemoMainView : Window
    {
        public MemoMainView()
        {
            InitializeComponent();

            // 最小化按钮单击事件订阅窗口最小化方法
            ButtonWindowMin.Click += WindowMinimizeAsync;

            // 最小化按钮单击事件订阅窗口最小化方法
            ButtonWindowMax.Click += WindowMaximize;

            // 最小化按钮单击事件订阅窗口最小化方法
            ButtonWindowClose.Click += WindowClose;

            // 自定义标题栏订阅移动窗口功能
            ColorZoneTilteBar.MouseLeftButtonDown += WindowMove;

            // 自定义标题栏订阅双击放大/还原
            ColorZoneTilteBar.MouseDoubleClick += WindowMaximizeOrNormal;
        }

        // 实现窗口最小化
        private void WindowMinimizeAsync(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        // 实现窗口最大化
        private void WindowMaximize(object sender, RoutedEventArgs e)
        {
            // 判断当前窗口是不是最大化
            if(this.WindowState == WindowState.Maximized)
            {
                // 如果当前窗口时最大化就还原
                this.WindowState = WindowState.Normal;
            }
            else
            {
                // 如果当前窗口不是最大化就最大化
                this.WindowState = WindowState.Maximized;
            }
        }

        // 实现关闭窗口
        private void WindowClose(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // 实现左键按住标题栏移动窗口
        private void WindowMove(object sender, RoutedEventArgs e)
        {
            this.DragMove();
        }

        // 实现双击标题栏放大或还原
        private void WindowMaximizeOrNormal(object sender, RoutedEventArgs e)
        {
            // 如果当前窗口是最大化就还原
            if(this.WindowState == WindowState.Maximized)
            {
                this.WindowState= WindowState.Normal;
            }
            else
            {
                // 如果当前窗口是普通状态就最大化
                this.WindowState = WindowState.Maximized;
            }
        }
    }
}
