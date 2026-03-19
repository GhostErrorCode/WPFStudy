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
        // 配置项
        // 字段：PackUrl前缀（固定协议头：声明这是 WPF 的 Pack URI（类似 http:// 是网页协议））
        private string _packUrlPrefix = "pack://application:,,,/";
        // 字段：固定分隔符（表示 “程序集内的资源组件”）
        private string _component = ";component/";
        // 字段：各程序、类库主题存放路径
        private string _themesPath = "Resources/Themes/";
        // 字段: 资源字典上下文
        private Collection<ResourceDictionary> _appResources = Application.Current.Resources.MergedDictionaries;

        public void InitTheme(Theme targetTheme)
        {
            this.SwitchTheme(targetTheme);
        }

        public void SwitchTheme(Theme targetTheme)
        {
            // 移除所有旧主题（查询能匹配主题路径的资源字典）
            List<ResourceDictionary> oldThemes = this._appResources.Where((ResourceDictionary d) => d.Source != null && d.Source.OriginalString.Contains(this._themesPath)).ToList();
            foreach (ResourceDictionary theme in oldThemes)
            {
                this._appResources.Remove(theme);
            }

            // 加载新主题（包括全局和类库中的）
            string[] themeAssemblies = new string[] { "WpfUiTest.App", "WpfUiTest.Controls", "WpfUiTest.Modules.ToDo"};
            foreach(string assemblyName in themeAssemblies)
            {
                this._appResources.Add(new ResourceDictionary()
                {
                    Source = new Uri(this._packUrlPrefix +
                                    assemblyName + 
                                    this._component + 
                                    this._themesPath + 
                                    targetTheme.EnumThemeToEnglishStringTheme() + 
                                    ".xaml")
                });
            }

            // 最后应用WPFUI主题
            ApplicationThemeManager.Apply(targetTheme.EnumThemeToApplicationTheme());

            /*
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
            */
        }
    }
}
