using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using Wpf.Ui.Appearance;
using WpfUiTest.Core.Services.Interfaces;
using WpfUiTest.Shared.Enums;
using WpfUiTest.Shared.Extensions;

namespace WpfUiTest.App.Services.Implements
{
    // 自定义应用主题服务接口实现类
    public class CustomThemeService : ICustomThemeService
    {
        // 配置项：实际项目名称
        private string _projectName = "WpfUiTest.App";
        private string _customThemeBasePath = "Resources/Colors/Themes/";

        // 字段: 资源字典上下文
        private Collection<ResourceDictionary> _appResources = Application.Current.Resources.MergedDictionaries;

        public void InitTheme(Theme targetTheme)
        {
            this.SwitchTheme(targetTheme);
        }

        public void SwitchTheme(Theme targetTheme)
        {
            // 1. 删除旧字典，查找资源字典集合里的自定义主题资源
            ResourceDictionary? oldResource = this._appResources.FirstOrDefault(d =>
                d.Source?.ToString().EndsWith("pack://application:,,,/WpfUiTest.App;component/Resources/Colors/Themes/Dark.xaml") == true ||
                d.Source?.ToString().EndsWith("pack://application:,,,/WpfUiTest.App;component/Resources/Colors/Themes/Light.xaml") == true ||
                d.Source?.ToString().EndsWith("pack://application:,,,/WpfUiTest.App;component/Resources/Colors/Themes/HighContrast.xaml") == true);
            // 如果找到了且不为null就移除他
            if (oldResource != null) this._appResources.Remove(oldResource);
            // 删除Controls层（类库）旧字典，查找资源字典集合里的自定义主题资源
            ResourceDictionary? oldControlsResource = this._appResources.FirstOrDefault(d =>
                d.Source?.ToString().EndsWith("pack://application:,,,/WpfUiTest.Controls;component/Resources/Themes/Dark.xaml") == true ||
                d.Source?.ToString().EndsWith("pack://application:,,,/WpfUiTest.Controls;component/Resources/Themes/Light.xaml") == true ||
                d.Source?.ToString().EndsWith("pack://application:,,,/WpfUiTest.Controls;component/Resources/Themes/HighContrast.xaml") == true);
            // 如果找到了且不为null就移除他
            if (oldControlsResource != null) this._appResources.Remove(oldControlsResource);

            // 2. 拼接绝对路径
            // 根据切换后的目标主题找到需要的自定义主题资源字典文件
            string fileName = targetTheme.EnumThemeToEnglishStringTheme() + ".xaml";
            // 完整的路径
            string fullPath = $"pack://application:,,,/{this._projectName};component/{this._customThemeBasePath}{fileName}";
            // 新的资源字典
            ResourceDictionary newResource = new ResourceDictionary { Source = new Uri(fullPath, UriKind.Absolute) };
            // 再添加Controls层（类库）的资源字典（主题）
            ResourceDictionary newControlsResource = new ResourceDictionary() { Source = new Uri($"pack://application:,,,/WpfUiTest.Controls;component/Resources/Themes/{fileName}", UriKind.Absolute) };

            // 3. 添加新字典
            this._appResources.Add(newResource);
            this._appResources.Add(newControlsResource);

            // 4.强制刷新资源字典集合
            // 重置资源字典引用，触发DynamicResource重新解析
            //Application.Current.Resources.MergedDictionaries.Contains(newResource);

            // 5.应用WPFUI主题
            ApplicationThemeManager.Apply(targetTheme.EnumThemeToApplicationTheme());
        }
    }
}
