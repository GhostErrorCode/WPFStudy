using ControlzEx.Standard;
using MaterialDesignThemes.Wpf;
using MemoDesktop.Dtos.Memo;
using MemoDesktop.Dtos.ToDo;
using MemoDesktop.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.Arm;
using System.Text;

namespace MemoDesktop.ViewModels.Dialogs
{
    /*
    特性          IDialogService                          IDialogAware
    角色          调用者（请求显示对话框）	              响应者（对话框本身）
    使用位置      主 ViewModel（比如 MemoListViewModel）  对话框 ViewModel（比如 AddMemoDialogViewModel）
    作用          请求打开、管理、关闭对话框              定义对话框的行为和生命周期
    类比          饭店顾客（点菜）	                      厨师（做菜）
    */
    /// <summary>
    /// IDialogAware
    /// 对话框感知接口
    /// 作用：定义对话框ViewModel的行为和生命周期方法
    /// 使用场景：对话框ViewModel必须实现的接口（如AddMemoDialogViewModel）
    /// 注意：这是对话框本身实现的接口，用于响应调用者的请求
    /// </summary>
    public class AddMemoDialogViewModel : BindableBase, IDialogHostAware
    {
        /*
        /// <summary>
        /// 请求关闭对话框事件
        /// 对话框ViewModel通过触发此事件来请求关闭对话框
        /// 调用方：dialogViewModel.RequestClose?.Invoke(result);
        /// </summary>
        public DialogCloseListener RequestClose { get; set; }

        /// <summary>
        /// 是否可以关闭对话框
        /// 在对话框关闭前调用，用于验证是否可以关闭
        /// </summary>
        /// <returns>
        /// true：允许关闭对话框
        /// false：阻止对话框关闭（可用于数据验证）
        /// </returns>
        public bool CanCloseDialog()
        {
            return true;
        }

        /// <summary>
        /// 对话框关闭时调用
        /// 执行清理操作，如释放资源、取消订阅等
        /// 注意：对话框完全关闭后执行，用于收尾工作
        /// </summary>
        public void OnDialogClosed()
        {

        }

        /// <summary>
        /// 对话框打开时调用
        /// 接收调用者传递的参数，并进行初始化
        /// </summary>
        /// <param name="parameters">从调用方传递过来的参数</param>
        public void OnDialogOpened(IDialogParameters parameters)
        {

        }
        */

        // 要显示在那个容器的名称
        public string DialogHostName { get; set; }

        // 保存命令
        public DelegateCommand SaveCommand { get; set; }
        // 取消命令
        public DelegateCommand CancelCommand { get; set; }

        // 添加/编辑对话框时存储的对象
        private MemoDto _addOrUpdateMemo;
        public MemoDto AddOrUpdateMemo
        {
            get { return _addOrUpdateMemo; }
            set { _addOrUpdateMemo = value; RaisePropertyChanged(); }
        }

        public AddMemoDialogViewModel()
        {
            this.SaveCommand = new DelegateCommand(Save);
            this.CancelCommand = new DelegateCommand(Cancel);
        }

        private void Save()
        {
            // 先判断下用户输入的内容，标题和内容不能为空
            if (string.IsNullOrWhiteSpace(this._addOrUpdateMemo.Title) || string.IsNullOrWhiteSpace(this._addOrUpdateMemo.Content)) { return; }

            // 先判断是否有对话框打开着，有打开着的对话框才运行关闭
            if (DialogHost.IsDialogOpen(DialogHostName))
            {
                // 定义对话框参数并赋值
                DialogParameters param = new DialogParameters();
                param.Add("Value", this._addOrUpdateMemo);

                // 定义对话框结果集
                DialogResult dialogResult = new DialogResult()
                {
                    Result = ButtonResult.OK,
                    Parameters = param
                };
                // DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.OK, param));
                DialogHost.Close(DialogHostName, dialogResult);
            }
        }

        private void Cancel()
        {
            if (DialogHost.IsDialogOpen(DialogHostName))
            {
                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.Cancel));
            }
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            // 用来接收编辑的数据备忘录
            if (parameters.ContainsKey("Value"))
            {
                // 如果有这个值就赋值给AddOrUpdateToDo
                this.AddOrUpdateMemo = parameters.GetValue<MemoDto>("Value");
            }
            else
            {
                // 如果没有找到那个值，就初始化
                this.AddOrUpdateMemo = new MemoDto();
            }
        }
    }
}
