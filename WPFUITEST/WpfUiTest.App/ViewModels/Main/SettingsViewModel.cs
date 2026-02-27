using System;
using System.Collections.Generic;
using System.Text;
using WpfUiTest.Shared.Base;
using WpfUiTest.Shared.Models;
using WpfUiTest.Shared.Utilities;

namespace WpfUiTest.App.ViewModels.Main
{
    // SettingsView的主ViewModel
    public class SettingsViewModel : BaseViewModel
    {
        // 属性 - 关于设置ViewModel
        public SettingsAboutViewModel SettingsAboutViewModel { get; set; }
        // 属性 - 主题设置ViewModel
        public SettingsThemeViewModel SettingsThemeViewModel { get; set; }


        // ============ 构造函数 ==============
        public SettingsViewModel(SettingsAboutViewModel settingsAboutViewModel, SettingsThemeViewModel settingsThemeViewModel)
        {
            // 初始化字段

            // 初始化属性
            this.SettingsAboutViewModel = settingsAboutViewModel;
            this.SettingsThemeViewModel = settingsThemeViewModel;

            // 初始化命令
        }


        public override async Task OnNavigatedToAsync()
        {
            await base.OnNavigatedToAsync();
        }
    }
}
