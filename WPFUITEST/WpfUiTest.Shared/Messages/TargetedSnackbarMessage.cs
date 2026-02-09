using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Wpf.Ui.Controls;
using WpfUiTest.Shared.Enums;

namespace WpfUiTest.Shared.Messages
{
    // 定向消息提示
    // 用于指定再某页面提示某消息
    public class TargetedSnackbarMessage
    {
        // 目标View
        public SnackbarTarget Target { get; set; } = SnackbarTarget.MainView;
        // 标题
        public string Title { get; set; } = string.Empty;
        // 内容
        public string Message { get; set; } = string.Empty;
        // 样式 - 默认为Info
        public ControlAppearance Appearance { get; set; } = ControlAppearance.Info;
        // 图标 - 图标可为空，这里为了调用方便，不使用IconElement当作类型，扩展方法中会使用此类型
        public IconElement? Icon { get; set; } = null;
        // 持续时间 - 默认3秒
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(3);
    }
}
