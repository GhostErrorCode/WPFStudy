using System;
using System.Collections.Generic;
using System.Text;

namespace WpfUiTest.Core.Services.Interfaces
{
    // Window窗体服务接口
    public interface IWindowService
    {
        // 显示主窗口
        public void ShowMainWindow();
        // 隐藏主窗口
        public void HideMainWindow();

        // 显示登录窗口
        public void ShowUserWindow();
        // 隐藏登录窗口
        public void HideUserWindow();
    }
}
