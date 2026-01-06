using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Text;

namespace MemoDesktop.Services.Interfaces
{
    /// <summary>
    /// 带对话框主机的对话框服务接口
    /// 这个接口专门用于在指定位置的对话框主机中显示对话框
    /// 大白话：这是一个"对话框管家"，专门负责打开和关闭对话框窗口
    /// </summary>
    public interface IDialogHostService : IDialogService
    {
        /// <summary>
        /// 在指定对话框主机中显示对话框
        /// 大白话：在指定的"对话框舞台"上，打开一个对话框窗口
        /// </summary>
        /// 
        /// <param name="name">
        /// 对话框名称
        /// 大白话：要打开哪个对话框，比如"确认删除框"、"用户信息编辑框"
        /// </param>
        /// 
        /// <param name="parameters">
        /// 对话框参数
        /// 大白话：打开对话框时带过去的数据，比如要删除的用户ID、要编辑的用户信息
        /// </param>
        /// 
        /// <param name="dialogHostName">
        /// 对话框主机名称（可选，默认为"RootDialog"）
        /// 大白话：在哪个地方显示对话框。比如有多个弹窗区域时，指定在"主区域"或"侧边区域"显示
        /// 默认值"RootDialog"：如果没指定，就显示在主舞台上
        /// 此处的RootDialog是主页面定义的<!-- 在 MainWindow.xaml 中 -->
        /// 
        /// <materialDesign:DialogHost x:Name="RootDialog">
        ///   <!-- 你的页面内容 -->
        ///  </materialDesign:DialogHost>
        ///  
        ///  x:Name：是给 WPF控件本身 起的名字（C#代码中能用）
        ///  Identifier：是给 DialogHost功能 起的名字（DialogHost服务能用）
        ///  Identifier="RootDialog" 此属性可以理解为DialogHost服务专用显示对话框
        /// 
        /// </param>
        /// 
        /// <returns>
        /// 对话框返回的结果
        /// 大白话：用户操作完对话框后返回的结果，比如用户点了"确定"还是"取消"，以及返回了什么数据
        /// </returns>
        /// 
        /// 使用示例：
        /// var result = await dialogHostService.ShowDialog("确认删除", parameters, "MainDialog");
        /// if (result.Result == ButtonResult.OK) {
        ///     // 用户点了确定，执行删除操作
        /// }
        public Task<IDialogResult> ShowDialog(string name, IDialogParameters parameters, string dialogHostName = "RootDialog");
    }
}
