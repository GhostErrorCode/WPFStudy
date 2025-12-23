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

namespace MemoDesktop.Views.Components
{
    /// <summary>
    /// UpdateLoadingAnimation.xaml 的交互逻辑
    /// </summary>
    public partial class UpdateLoadingAnimation : UserControl
    {
        // 第1行：声明依赖属性标识符字段
        public static readonly DependencyProperty MessageProperty =
            // 第2-7行：调用静态方法向WPF属性系统注册这个属性
            DependencyProperty.Register(
                // 参数1：属性名称（字符串）
                "Message",
                // 参数2：属性值的CLR类型
                typeof(String),
                // 参数3：拥有此属性的类（所有者类型）
                typeof(UpdateLoadingAnimation),
                // 参数4：属性的元数据，包含默认值和其他行为信息
                new PropertyMetadata("正在加载...")
            );

        // 第8-14行：创建标准的CLR属性包装器
        public String Message
        {
            // 第10行：get访问器，从依赖属性系统中获取当前值
            get { return (String)GetValue(MessageProperty); }
            // 第11行：set访问器，向依赖属性系统设置新值
            set { SetValue(MessageProperty, value); }
        }
        public UpdateLoadingAnimation()
        {
            InitializeComponent();
        }
    }
}
