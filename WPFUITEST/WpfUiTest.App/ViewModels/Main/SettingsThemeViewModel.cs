using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using WpfUiTest.App.Views.Main;
using WpfUiTest.Core.Services.Interfaces;
using WpfUiTest.Shared.Enums;
using WpfUiTest.Shared.Extensions;
using WpfUiTest.Shared.Models;
using WpfUiTest.Shared.Utilities;

namespace WpfUiTest.App.ViewModels.Main
{
    // 设置 - 主题设置
    public class SettingsThemeViewModel : ObservableObject
    {
        // 属性: ComboBox子项
        // 暴露所有枚举值，用于 ComboBox 的 ItemsSource, IEnumerable只读符合当前环境, 可用List集合
        // public IEnumerable<Theme> ThemeOptions { get; } = Enum.GetValues(typeof(Theme)).Cast<Theme>();

        // 属性: 设置主题ComboBox子项
        public List<Theme> ThemeOptions { get; }

        // 字段属性: 当前应用主题
        private Theme _currentTheme;
        public Theme CurrentTheme
        {
            get { return _currentTheme; } 
            set
            {
                // 验证字段是否变更
                if(SetProperty(ref _currentTheme, value))
                {
                    // 变更成功后，改变当前主题并保存设置
                    this.ThemeChanged();
                }
            }
        }

        // 字段: 应用配置
        private AppConfiguration _appConfiguration;
        // 字段: 日志服务
        private ILogger<SettingsThemeViewModel> _logger;
        // 字段: IMessenger服务
        private IMessenger _messenger;
        // 字段: ICustomThemeService服务
        private ICustomThemeService _customThemeService;



        // ========== 构造函数 ==========
        public SettingsThemeViewModel(ILogger<SettingsThemeViewModel> logger, IMessenger messenger, ICustomThemeService customThemeService)
        {
            // 初始化字段
            this._logger = logger;
            this._messenger = messenger;
            this._customThemeService = customThemeService;

            this._appConfiguration = AppConfigurationHelper.LoadMergeSettings();
            this._currentTheme = this._appConfiguration.ThemeSettings.Theme.StringThemeToEnumTheme();

            // 初始化属性
            // this._mainFooterMenuViewModel = this._serviceProvider.GetRequiredService<MainFooterMenuViewModel>();
            this.ThemeOptions = new List<Theme>()
            {
                Theme.Light,
                Theme.Dark,
                Theme.HighContrast
            };
            // 初始化命令
        }



        // ========== 方法 ==========
        // 方法: 切换当前主题
        private void ThemeChanged()
        {
            try
            {
                // 日志
                this._logger.LogDebug("SettingsThemeViewModel：应用主题切换开始");

                // 切换当前应用主题
                // ApplicationThemeManager.Apply(this._currentTheme.EnumThemeToApplicationTheme());
                this._customThemeService.SwitchTheme(this._currentTheme);

                // === 保存当前主题至用户JSON文件 ===
                // 先读取应用配置并保存到当前VM的字段中
                this._appConfiguration = AppConfigurationHelper.LoadMergeSettings();
                // 改变应用配置中的主题
                this._appConfiguration.ThemeSettings.Theme = this._currentTheme.ToString();
                // 保存配置
                AppConfigurationHelper.SaveUserSettings(this._appConfiguration);

                // 输出日志并显示Snackbar
                this._messenger.ShowSuccess(SnackbarTarget.MainView, "应用主题已切换", $"应用主题已成功切换至 {this._currentTheme.EnumThemeToStringTheme()}");
                this._logger.LogInformation("应用主题已切换至 {theme}", this._currentTheme.EnumThemeToStringTheme());
            }
            catch(Exception ex)
            {
                this._logger.LogError("应用主题切换失败! 发生意外的未处理异常: {ex}", ex);
                this._messenger.ShowDanger(SnackbarTarget.MainView, "应用主题切换失败!", "系统出现意外的严重错误!");
            }
            finally
            {
                this._logger.LogDebug("SettingsThemeViewModel：应用主题切换结束");
            }
        }
    }
}
