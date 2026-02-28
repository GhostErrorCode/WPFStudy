using System;
using System.Collections.Generic;
using System.Text;
using WpfUiTest.Shared.Enums;

namespace WpfUiTest.Core.Services.Interfaces
{
    // 自定义应用主题服务接口
    public interface ICustomThemeService
    {
        // 初始主题
        public void InitTheme(Theme targetTheme);

        // 切换主题
        public void SwitchTheme(Theme targetTheme);
    }
}
