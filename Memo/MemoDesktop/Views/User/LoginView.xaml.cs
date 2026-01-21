using MemoDesktop.Events;
using MemoDesktop.Extensions;
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

namespace MemoDesktop.Views.User
{
    /// <summary>
    /// LoginView.xaml 的交互逻辑
    /// </summary>
    public partial class LoginView : UserControl
    {
        public LoginView(IEventAggregator eventAggregator)
        {
            InitializeComponent();

            // 订阅登录页面的消息提示器
            eventAggregator.MessageEventSubscribe("Login", (MessageEventArgs messageEventArgs) =>
            {
                LoginSnackbar.MessageQueue.Enqueue(messageEventArgs.Message);
            });
        }
    }
}
