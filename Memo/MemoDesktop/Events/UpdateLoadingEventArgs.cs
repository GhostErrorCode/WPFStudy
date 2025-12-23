using System;
using System.Collections.Generic;
using System.Text;

namespace MemoDesktop.Events
{
    // 更新加载事件的参数类
    public class UpdateLoadingEventArgs
    {
        // 用于判断是否打开加载窗口
        public bool IsOpen { get; set; }= false;
        
        // 用于说明是那个模块调用的
        public string ModuleName { get; set; } = string.Empty;

        // 用于说明加载时的提示消息
        public string Message { get; set; } = "正在加载...";
    }
}
