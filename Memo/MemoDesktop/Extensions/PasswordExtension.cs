using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace MemoDesktop.Extensions
{
    // 密码框的附件属性
    public class PasswordExtension
    {
        // 1. 定义 “获取器” 和 “设置器”
        // 作用：这是WPF附加属性的标准写法。GetPassword 和 SetPassword 是静态方法，用来从任意控件（这里是PasswordBox）上读取或写入我们定义的附加属性的值。
        public static string GetPassword(DependencyObject obj)
        {
            return (string)obj.GetValue(PasswordProperty);
        }
        public static void SetPassword(DependencyObject obj, string value)
        {
            obj.SetValue(PasswordProperty, value);
        }

        // 2. 核心：声明 “附加属性”
        // 作用：这是注册附加属性。PasswordProperty 是一个 DependencyProperty 对象，它是WPF依赖属性系统的核心标识。
        // "Password"：在XAML中会这样使用：pass:PasswordExtension.Password="..."。
        // OnPasswordPropertyChanged：最关键的部分。它指定了一个方法（回调函数），当这个附加属性的值通过绑定发生改变时，WPF会自动调用这个方法。
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached(
                "Password",                         // 属性在XAML中使用的名字
                typeof(string),                     // 属性值的类型（字符串）
                typeof(PasswordExtension),          // 拥有这个属性的类（本类）
                new PropertyMetadata(               // 属性的元数据配置
                    string.Empty,                   // 默认值（空字符串）
                    OnPasswordPropertyChanged       // 属性值改变时的回调方法
                )
            );

        // 3. 属性变更回调方法
        // 作用：这是数据从ViewModel流向UI的桥梁。
        // 触发时机：当你的ViewModel里的 Password 属性变化，导致XAML中 pass:PasswordExtension.Password="{Binding Password}" 这个绑定更新时。
        // 它做了什么：它接收绑定的新值（e.NewValue），然后找到对应的真实 PasswordBox 控件，手动将其 Password 属性设置为这个新值。这样就绕过了 PasswordBox.Password 不能直接绑定的限制。
        private static void OnPasswordPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            // sender 就是拥有这个附加属性的控件，即我们的 PasswordBox
            var Password = sender as PasswordBox;
            // e.NewValue 是附加属性被赋予的新值（来自ViewModel的绑定）
            string newPassword = (string)e.NewValue;

            // 判断：如果控件存在，且它当前的密码与新值不同，则更新它
            if (Password != null && Password.Password != newPassword)
            {
                Password.Password = newPassword; // 这里：将绑定的新值，手动赋给真实密码框
            }
        }
    }


    // PasswordBehavior 类（行为）
    // 这个类的核心作用是监听密码框本身的输入事件，并把用户输入的值“推”回到我们上面创建的附加属性中，从而反向通知ViewModel。
    public class PasswordBehavior : Behavior<PasswordBox>
    {
        // 1. 当此行为被附加到 PasswordBox 上时执行
        protected override void OnAttached()
        {
            base.OnAttached();
            // AssociatedObject 就是被附加的 PasswordBox 控件本身
            // 订阅它的 PasswordChanged 事件
            AssociatedObject.PasswordChanged += AssociatedObject_PasswordChanged;
            // 作用：建立监听。当你在XAML中加入 <pass:PasswordBehavior />，这个行为对象就会“附着”到 PasswordBox 上。
            // OnAttached 是它的初始化方法，在这里它订阅了密码框的 PasswordChanged 事件（用户每次按键都会触发这个事件）。
        }

        // 2. 事件处理方法：当用户在密码框中输入时触发
        private void AssociatedObject_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // sender 就是触发事件的 PasswordBox
            PasswordBox passwordBox = sender as PasswordBox;
            // 从附加属性中获取当前的绑定值
            string password = PasswordExtension.GetPassword(passwordBox);

            // 判断：如果控件存在，且密码框的真实密码与附加属性的值不同
            if (passwordBox != null && passwordBox.Password != password)
            {
                // 则将密码框的真实密码，设置回附加属性
                PasswordExtension.SetPassword(passwordBox, passwordBox.Password);
            }
        }

        // 3. 当行为从 PasswordBox 上分离时执行（如控件被销毁）
        protected override void OnDetaching()
        {
            base.OnDetaching();
            // 取消事件订阅，防止内存泄漏
            AssociatedObject.PasswordChanged -= AssociatedObject_PasswordChanged;
        }
    }
}
