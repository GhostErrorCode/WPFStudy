using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace PrismDialog.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IDialogService _dialogService;

        // 通过构造函数注入IDialogService实例
        public MainWindowViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
            // 初始化一个命令，用于触发显示对话框
            ShowDialogCommand = new DelegateCommand(ShowDialog);
        }

        // 公开一个命令，用于绑定到主窗口的按钮
        public DelegateCommand ShowDialogCommand { get; private set; }

        // 显示对话框的方法
        private void ShowDialog()
        {
            // 创建对话框参数，用于传递数据到对话框
            IDialogParameters parameters = new DialogParameters();
            // 向参数中添加一个键值对，键为"message"，值为要显示的具体消息[citation:1]
            // 注意：在Prism 9.0中，DialogParameters不允许添加null值[citation:5]
            parameters.Add("message", "这是一个重要的操作，你确定要执行吗？");

            // 调用对话服务的Show方法显示对话框
            // 第一个参数：注册的对话框名称
            // 第二个参数：传递的参数
            // 第三个参数：对话框关闭后的回调函数，这里使用lambda表达式处理结果
            _dialogService.ShowDialog("ConfirmDialog", parameters, (IDialogResult result) =>
            {
                // 检查对话框返回的结果
                if (result.Result == ButtonResult.OK)
                {
                    // 用户点击了确认按钮，执行相关操作
                    MessageBox.Show("用户选择了确认！" + result.Parameters.GetValue<string>("MESSAGE"));
                }
                else if (result.Result == ButtonResult.Cancel)
                {
                    // 用户点击了取消按钮，执行相关操作
                    MessageBox.Show("用户选择了取消！");
                }
            });
        }
    }
}
