using MaterialDesignThemes.Wpf;
using MemoDesktop.Constants;
using MemoDesktop.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MemoDesktop.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        // 字段和属性
        // 全局区域管理器
        private IRegionManager _regionManager;
        // 定义菜单栏列表项（动态）
        private ObservableCollection<MenuBarModel> _settingsMenuBars;
        public ObservableCollection<MenuBarModel> SettingsMenuBars
        {
            get { return _settingsMenuBars; }
            set { _settingsMenuBars = value; }
        }
        // 左侧菜单列表项点击按钮
        public DelegateCommand<MenuBarModel> SettingsNavigateCommand { get; private set; }

        // 构造函数
        public SettingsViewModel(IRegionManager regionManager)
        {
            // 初始化区域管理器
            this._regionManager = regionManager;
            // 创建设置的菜单栏
            this.CreateSettingsMenuBars();
            // 设置页面的导航Command
            this.SettingsNavigateCommand = new DelegateCommand<MenuBarModel>(SettingsNavigate);
        }

        private void SettingsNavigate(MenuBarModel model)
        {
            // 判断MenuBarModel是不是不为空
            if (model == null || string.IsNullOrEmpty(model.NavigationPath)) return;
            this._regionManager.Regions[RegionNames.SettingsViewRegionName].RequestNavigate(model.NavigationPath);
        }

        private void CreateSettingsMenuBars()
        {
            this.SettingsMenuBars = new ObservableCollection<MenuBarModel>();
            SettingsMenuBars.Add(new MenuBarModel() { Icon = PackIconKind.Palette, Name = "个性化", NavigationPath = "SkinView" });
            SettingsMenuBars.Add(new MenuBarModel() { Icon = PackIconKind.Cog, Name = "系统设置", NavigationPath = "" });
            SettingsMenuBars.Add(new MenuBarModel() { Icon = PackIconKind.Information, Name = "关于我们", NavigationPath = "AboutView" });
        }
    }
}
