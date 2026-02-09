using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace WpfUiTest.Shared.Behaviors
{
    /// <summary>
    /// 【极简版】WPFUI PasswordBox 双向绑定器
    /// 
    /// 核心功能：为 WPFUI 的 PasswordBox 控件提供纯粹的双向数据绑定支持。
    /// 设计目标：以最少的代码，解决 ViewModel 字符串属性与 PasswordBox.Password 之间的同步问题。
    /// 工作原理：创建一个附加属性作为“桥梁”，监听两端变化并同步数据。
    /// 适用场景：用户手动输入、表单初始化等绝大多数业务场景。
    /// 注意事项：如果业务中需要极高频（如定时器）通过代码设置 Password 属性，请使用防递归更严格的完整版。
    /// </summary>
    public static class PasswordBoxBindingBehavior
    {
        // ====================== 【1. 定义附加依赖属性 - 核心桥梁】 ======================
        /// <summary>
        /// 依赖属性标识符。这是在 XAML 中进行绑定的目标属性。
        /// 例如：ext:SimplePasswordBoxBinder.Password="{Binding MyPassword, Mode=TwoWay}"
        /// 
        /// 参数详解：
        /// 1. “Password”: 附加属性在XAML中使用的名称。
        /// 2. typeof(string): 属性类型，即我们要绑定的密码字符串。
        /// 3. typeof(SimplePasswordBoxBinder): 声明此属性的所有者类。
        /// 4. PropertyMetadata(...):
        ///    - string.Empty: 属性的默认值。
        ///    - OnPasswordPropertyChanged: 关键！当此属性的绑定值（来自ViewModel）发生变化时，WPF会自动调用的回调方法。
        ///      这是实现 ViewModel -> UI 方向同步的关键。
        /// </summary>
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached(
                "Password",
                typeof(string),
                typeof(PasswordBoxBindingBehavior),
                new PropertyMetadata("PASSWORD", OnPasswordPropertyChanged));

        // ====================== 【2. 附加属性的CLR包装器 - 标准写法】 ======================
        /// <summary>
        /// 获取附加属性值的方法。WPF属性系统内部使用，开发者通常不直接调用。
        /// </summary>
        public static string GetPassword(DependencyObject obj) => (string)obj.GetValue(PasswordProperty);
        /// <summary>
        /// 设置附加属性值的方法。WPF属性系统内部使用，我们会在事件处理函数中调用它以更新ViewModel。
        /// </summary>
        public static void SetPassword(DependencyObject obj, string value) => obj.SetValue(PasswordProperty, value);


        // ====================== 【3. 回调：ViewModel的数据如何流向UI控件】 ======================
        /// <summary>
        /// 当通过绑定设置到附加属性上的值发生变化时（例如ViewModel的Password属性被赋值），
        /// 此方法被WPF框架自动调用。
        /// 
        /// 职责：将来自数据源（ViewModel）的新值，设置到目标 PasswordBox 控件的真实 Password 属性上。
        /// 这是实现“ViewModel -> UI”单向同步的环节。
        /// </summary>
        /// <param name="d">承载了此附加属性的控件实例，预期是一个 WPFUI PasswordBox。</param>
        /// <param name="e">包含新旧值的事件参数，e.NewValue 即来自ViewModel的新密码字符串。</param>
        private static void OnPasswordPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // 1. 类型检查：确保附加属性被用在了正确的控件上。
            if (d is not Wpf.Ui.Controls.PasswordBox passwordBox)
                return; // 如果不是，安全退出。

            // 2. 关键步骤：临时解除事件监听，防止下面赋值时触发“PasswordChanged”事件，
            //    导致事件处理函数（OnPasswordChanged）被调用，形成无意义的循环。
            passwordBox.PasswordChanged -= OnPasswordChanged;

            // 3. 核心同步：将ViewModel传来的新值（e.NewValue），赋给UI控件的Password属性。
            //    此时，密码框的显示内容（掩码）将更新。
            if (passwordBox.Password != (string)e.NewValue) { passwordBox.Password = (string)e.NewValue; }
                
            // 4. 重新挂接事件监听，以便继续响应用户后续的键盘输入。
            passwordBox.PasswordChanged += OnPasswordChanged;
        }

        // ====================== 【4. 事件处理：UI控件的数据如何流回ViewModel】 ======================
        /// <summary>
        /// 当用户在 PasswordBox 中进行输入（包括键盘输入、粘贴、清空）时，此方法被触发。
        /// 
        /// 职责：获取 PasswordBox 中当前的实时密码，并将其推送回附加属性。
        /// 由于附加属性绑定了ViewModel的属性，此操作会自动更新ViewModel，实现“UI -> ViewModel”单向同步。
        /// 这是实现双向绑定的另一个关键环节。
        /// </summary>
        private static void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            // 1. 类型检查和安全转换。
            if (sender is not Wpf.Ui.Controls.PasswordBox passwordBox)
                return;

            // 2. 核心同步：获取密码框中的当前值，并调用 SetPassword 将其设置回附加属性。
            //    SetPassword 调用会促使WPF绑定引擎去更新ViewModel中对应的绑定源属性。
            //    至此，用户输入的一个字符就通过 控件->附加属性->ViewModel 的路径完成了同步。
            SetPassword(passwordBox, passwordBox.Password);
        }
    }
}
