using MemoDesktop.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MemoDesktop.Extensions
{
    // 对话框的扩展方法
    public static class DialogExtensions
    {
        /// <summary>
        /// 用于简易显示消息提示框
        /// </summary>
        /// 
        /// <param name="dialogHostService">
        /// 扩展方法主体
        /// </param>
        /// 
        /// <param name="title">
        /// 消息提示框的名称Title
        /// </param>
        /// 
        /// <param name="content">
        /// 消息提示框的内容Content
        /// </param>
        /// 
        /// <param name="dialogHostName">
        /// 消息提示框的显示位置，默认为主窗口的RootDialog
        /// </param>
        /// 
        /// <returns>
        /// 返回结果
        /// </returns>
        public static async Task<IDialogResult> ShowMsgDialog(this IDialogHostService dialogHostService, string title, string content, string dialogHostName = "RootDialog")
        {
            // 组装对话参数
            DialogParameters dialogParameters = new DialogParameters();
            dialogParameters.Add("Title", title);
            dialogParameters.Add("Content", content);
            dialogParameters.Add("dialogHostName", dialogHostName);

            IDialogResult result = await dialogHostService.ShowDialog("MsgDialog", dialogParameters, dialogHostName);
            return result;
        }
    }
}
