using MaterialDesignThemes.Wpf;
using MemoDesktop.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MemoDesktop.ViewModels
{
    // 主页面的视图模型
    // 继承Prism MVVM模式的BindableBase
    public class MemoMainViewModel : BindableBase
    {
        // 定义菜单栏列表项（动态）
        // 在 ViewModel 中暴露给 View 进行绑定的任何可变集合，99%的情况都应该使用 ObservableCollection<T>。List<T> 通常用于方法内部或作为数据源的临时载体
        // UI自动更新的支持。当集合发生增、删、改、清空时，会自动触发通知，绑定的ListBox会立即刷新。
        private ObservableCollection<MenuBarModel> _menuBars;
        public ObservableCollection<MenuBarModel> MenuBars
        {
            get { return _menuBars; } 
            set { _menuBars = value; }
        }

        // 构造函数
        public MemoMainViewModel()
        {
            // 实例化菜单列表
            MenuBars = new ObservableCollection<MenuBarModel>();
            CreateMenuBar();
        }

        // 创建动态的菜单栏
        private void CreateMenuBar()
        {
            MenuBars.Add(new MenuBarModel() { Icon = PackIconKind.Home, Name = "首页", Nameapce = "IndexView" });
            MenuBars.Add(new MenuBarModel() { Icon = PackIconKind.NotebookOutline, Name = "代办事项", Nameapce = "ToDoView" });
            MenuBars.Add(new MenuBarModel() { Icon = PackIconKind.NotebookPlus, Name = "备忘录", Nameapce = "MemoView" });
            MenuBars.Add(new MenuBarModel() { Icon = PackIconKind.Cog, Name = "设置", Nameapce = "SettingView" });
        }
    }
}
