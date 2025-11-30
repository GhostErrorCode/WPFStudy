using System;
using System.Collections.Generic;
using System.Text;

namespace PrismDialog.ViewModels
{
    public class ConfirmDialogViewModel : BindableBase, IDialogAware
    {
        // 声明RequestClose事件，用于请求关闭对话框
        public DialogCloseListener RequestClose { get; }

        // 绑定到视图的消息内容属性
        private string _message;
        // 公开的Message属性，用于显示在对话框上
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); } // SetProperty来自BindableBase，通知属性变化
        }

        // 确认命令
        public DelegateCommand ConfirmCommand { get; private set; }
        // 取消命令
        public DelegateCommand CancelCommand { get; private set; }

        // 构造函数，初始化命令
        public ConfirmDialogViewModel()
        {
            // 初始化ConfirmCommand，当执行时调用OnConfirm方法
            ConfirmCommand = new DelegateCommand(OnConfirm);
            // 初始化CancelCommand，当执行时调用OnCancel方法
            CancelCommand = new DelegateCommand(OnCancel);
        }

        // 判断对话框是否可以关闭，这里总是允许关闭
        public bool CanCloseDialog()
        {
            return true;
        }

        // 对话框关闭后的回调方法
        public void OnDialogClosed()
        {
            // 可以在这里进行资源清理等工作
        }

        // 对话框打开时的回调方法，这里是参数传递的关键
        public void OnDialogOpened(IDialogParameters parameters)
        {
            // 从传递的参数中根据键"message"获取值，如果没有则使用默认消息
            // 注意：Prism 9.0中DialogParameters不允许传递null值[citation:5]
            if (parameters.ContainsKey("message"))
            {
                Message = parameters.GetValue<string>("message");
            }
            else
            {
                Message = "你确定要继续吗？";
            }
        }

        // 处理确认按钮点击
        private void OnConfirm()
        {
            // 创建一个对话框结果，设置结果为ButtonResult.OK
            DialogParameters keys = new DialogParameters();
            keys.Add("MESSAGE","确认了！！！");

            //IDialogResult result = new DialogResult(ButtonResult.OK);

            // Prism 9.0 正确方式：使用 ButtonResult.OK 的扩展方法,而不在使用IDialogResult
            // 触发RequestClose事件，传递结果并关闭对话框
            RequestClose.Invoke(keys, ButtonResult.OK);
        }

        // 处理取消按钮点击
        private void OnCancel()
        {
            // 创建一个对话框结果，设置结果为ButtonResult.Cancel
            IDialogResult result = new DialogResult(ButtonResult.Cancel);
            // 触发RequestClose事件，传递结果并关闭对话框
            RequestClose.Invoke(result);
        }
    }
}
