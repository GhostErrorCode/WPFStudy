using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace WPFStudy.Command.ViewModels.Pages
{
    // 这个ViewModel使用了官方的Mvvm组件包，免去了很多繁琐重复的步骤

    // ViewModel 类，继承 ObservableObject 以获得属性变更通知功能
    public class MvvmMainViewModel : ObservableObject
    {
        // 私有字段，存储文本框的内容
        private string _name = "初始文本";

        // 公共属性，供界面绑定使用
        public string Name
        {
            // get 访问器，返回 _name 字段的值
            get { return _name; }
            // set 访问器，设置 _name 字段的值
            set
            {
                // 调用 SetProperty 方法，这个方法会：
                // 1. 检查新值是否与旧值相同
                // 2. 如果不同，更新字段值
                // 3. 触发属性变更事件，通知界面更新
                SetProperty(ref _name, value);
            }
        }

        // 公共属性，实现 ICommand 接口，用于按钮命令绑定
        public RelayCommand MvvmCommandStudy { get; private set; }

        // 构造函数，初始化 ViewModel
        public MvvmMainViewModel()
        {
            // 创建 RelayCommand 实例
            // 构造函数参数是一个 Action 委托，指向 ExecuteChangeText 方法
            MvvmCommandStudy = new RelayCommand(ExecuteChangeText);
        }

        // 私有方法，处理按钮点击的执行逻辑
        private void ExecuteChangeText()
        {
            // 显示消息对话框，获取用户输入
            // Show 方法会显示一个对话框，包含输入框和确定/取消按钮
            // 返回值是用户输入的字符串，如果用户点击取消则返回 null
            // string result = Microsoft.VisualBasic.Interaction.InputBox("请输入新的文本:", "修改文本", Name, -1, -1);

            // 检查用户是否输入了内容（不是空字符串且不是 null）
            /*if (!string.IsNullOrEmpty(result))
            {
                // 将 Name 属性设置为用户输入的新文本
                // 这会自动触发属性变更通知，更新界面
                Name = result;
            }*/
            MessageBox.Show("按钮被点击了!");
            Name = "按钮被点击了!";
        }
    }
}
