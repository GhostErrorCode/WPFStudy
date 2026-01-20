using DryIoc;
using DryIoc.FastExpressionCompiler.LightExpression;
using MemoDesktop.Events;
using MemoDesktop.Extensions;
using MemoDesktop.Services.Interfaces;
using MemoDesktop.Views.Components;
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
        // 事件聚合器
        private readonly IEventAggregator _eventAggregator;
        // 预缓存加载动画窗体
        private UpdateLoadingAnimation _updateLoadingAnimation;
        // 自定义对话服务
        private readonly IDialogHostService _dialogHostService;

        public MemoMainView(IEventAggregator eventAggregator, IDialogHostService dialogHostService)
        {
            InitializeComponent();

            // ===================================================================================================================
            // 通过依赖注入获取到事件聚合器
            this._eventAggregator = eventAggregator;
            // 获取自定义对话服务
            this._dialogHostService = dialogHostService;

            // 预创建并缓存LoadingAnimationView实例，避免重复创建
            this._updateLoadingAnimation = new UpdateLoadingAnimation();
            // 订阅加载动画的方法
            this.SubscribeUpdateLoadingEvent();

            // 订阅窗口关闭时的取消订阅方法
            this.Unloaded += UnSubscribeUpdateLoadingEvent;

            // 订阅消息提示方法
            this._eventAggregator.MessageEventSubscribe(message =>
            {
                MainSnackbar.MessageQueue.Enqueue(message);
            });
            // ===================================================================================================================

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

            // 左侧菜单ListBox的选中事件绑定方法，用于自动关闭左侧菜单
            LeftMenuListBox.SelectionChanged += CloseLeftMenu;
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
        private async void WindowClose(object sender, RoutedEventArgs e)
        {
            // 结束程序
            // 结束程序前的提示框
            IDialogResult dialogResult = await this._dialogHostService.ShowMsgDialog("温馨提示", "确认退出?");
            if(dialogResult.Result != ButtonResult.OK) { return; }

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

        // 实现自动关闭左侧菜单
        private void CloseLeftMenu(object sender, SelectionChangedEventArgs e)
        {
            MemoMainViewDrawerHost.IsLeftDrawerOpen = false;
        }

        // 订阅加载动画事件
        private void SubscribeUpdateLoadingEvent()
        {
            this._eventAggregator.GetEvent<UpdateLoadingEvent>().Subscribe(
                HandleUpdateLoadingEventChanged,
                Prism.Events.ThreadOption.UIThread,
                keepSubscriberReferenceAlive:false);
        }

        // 订阅加载动画事件所需要的方法
        private void HandleUpdateLoadingEventChanged(UpdateLoadingEventArgs args)
        {
            // IsOpen（DialogHost 的属性）	“弹窗架子” 的开关（开 = 显示，关 = 隐藏）	控制这个 “架子” 要不要出现在页面上
            // 将主页面的DialogHost的IsOpen属性改成转递过来的参数IsOpen属性
            MemoMainViewDialogHost.IsOpen = args.IsOpen;

            // 判断MemoMainViewDialogHost.IsOpen是否是True
            if (MemoMainViewDialogHost.IsOpen == true)
            {
                // 使用缓存的实例，只更新消息内容
                this._updateLoadingAnimation.Message = args.Message;
                // 展示一个加载的动画页面(通过缓存)
                MemoMainViewDialogHost.DialogContent = this._updateLoadingAnimation;
            }
        }

        // 取消订阅加载动画事件
        private void UnSubscribeUpdateLoadingEvent(object sender, RoutedEventArgs e)
        {
            this._eventAggregator.GetEvent<UpdateLoadingEvent>().Unsubscribe(HandleUpdateLoadingEventChanged);
        }
    }
}
