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

        // 左侧菜单列表项点击按钮
        public DelegateCommand<MenuBarModel> NavigateCommand { get; private set; }

        // 后退按钮
        public DelegateCommand GoBackCommand { get; private set; }

        // 前进按钮
        public DelegateCommand GoForwardCommand { get; private set; }

        // 依赖注入获取区域管理器
        private IRegionManager _regionManager;

        // 导航日志（用来保存历史记录）
        private IRegionNavigationJournal _navigationJournal;


        /// <summary>
        /// 构造函数：通过依赖注入获取 IRegionManager
        /// 
        /// 【依赖注入详解】
        /// 1. Prism 容器在创建 ViewModel 时，会自动解析构造函数参数
        /// 2. IRegionManager 已在 Prism 框架中默认注册为单例
        /// 3. 容器会查找已注册的服务并自动注入
        /// 4. 这是构造函数注入（Constructor Injection）模式
        /// </summary>
        /// <param name="regionManager">Prism 自动注入的区域管理器实例</param>
        public MemoMainViewModel(IRegionManager regionManager)
        {
            // 实例化菜单列表
            this.MenuBars = new ObservableCollection<MenuBarModel>();
            this.CreateMenuBar();

            // 左侧菜单列表按钮订阅对应方法
            this.NavigateCommand = new DelegateCommand<MenuBarModel>(Navigate);
            _regionManager = regionManager;

            // 获取区域管理器
            this._regionManager = regionManager;

            // 初始化后退命令
            GoBackCommand = new DelegateCommand(
                ExecuteGoBack,      // 执行后退的方法
                CanExecuteGoBack    // 判断能否后退的方法
            );

            // 初始化前进命令
            GoForwardCommand = new DelegateCommand(
                ExecuteGoForward,   // 执行前进的方法
                CanExecuteGoForward // 判断能否前进的方法
            );
        }

        // 创建动态的菜单栏
        private void CreateMenuBar()
        {
            MenuBars.Add(new MenuBarModel() { Icon = "Home", Name = "首页", NavigationPath = "IndexView" });
            MenuBars.Add(new MenuBarModel() { Icon = "NotebookOutline", Name = "代办事项", NavigationPath = "ToDoView" });
            MenuBars.Add(new MenuBarModel() { Icon = "NotebookPlus", Name = "备忘录", NavigationPath = "MemoView" });
            MenuBars.Add(new MenuBarModel() { Icon = "Cog", Name = "设置", NavigationPath = "SettingsView" });
        }

        /// <summary>
        /// 执行导航到指定视图
        /// 
        /// 【IRegionManager.RequestNavigate 方法详解】
        /// 参数1：regionName - 区域名称（必须与 XAML 中定义的 RegionName 一致）
        /// 参数2：viewName - 要导航的视图名称（在容器中注册的视图键）
        /// 参数3（可选）：navigationCallback - 导航完成后的回调函数
        /// 参数4（可选）：parameters - 传递给目标视图的导航参数
        /// </summary>
        private void Navigate(MenuBarModel menuBarModel)
        {
            // 判断MenuBarModel是不是不为空
            if (menuBarModel == null || string.IsNullOrEmpty(menuBarModel.NavigationPath)) return;
            this._regionManager.Regions[Constants.RegionNames.MainViewRegionName].RequestNavigate(menuBarModel.NavigationPath, OnNavigationCompleted);
        }

        // 导航完成后的回调函数，去存储导航的记录至_navigationJournal
        private void OnNavigationCompleted(NavigationResult result)
        {
            // 检查导航是否成功
            if(result.Success)
            {
                // 从导航结果中获取 NavigationService，进而获取导航日志
                _navigationJournal = result.Context.NavigationService.Journal;
                // 刷新按钮状态
                RefreshNavigationButtonStates();
            }
        }

        // 执行后退
        private void ExecuteGoBack()
        {
            // 如果有导航日志且可以后退
            if (_navigationJournal != null && _navigationJournal.CanGoBack)
            {
                // 执行后退
                _navigationJournal.GoBack();
                // 更新按钮状态
                RefreshNavigationButtonStates();
            }
        }

        // 判断能否后退
        private bool CanExecuteGoBack()
        {
            // 能后退的条件：1.有导航日志 2.日志里有后退记录
            return _navigationJournal != null && _navigationJournal.CanGoBack;
        }

        // 执行前进
        private void ExecuteGoForward()
        {
            // 如果有导航日志且可以前进
            if (_navigationJournal != null && _navigationJournal.CanGoForward)
            {
                // 执行前进
                _navigationJournal.GoForward();
                // 更新按钮状态
                RefreshNavigationButtonStates();
            }
        }

        // 判断能否前进
        private bool CanExecuteGoForward()
        {
            // 能前进的条件：1.有导航日志 2.日志里有前进记录
            return _navigationJournal != null && _navigationJournal.CanGoForward;
        }

        private void RefreshNavigationButtonStates()
        {
            // 告诉按钮重新检查能否执行
            GoBackCommand.RaiseCanExecuteChanged();
            GoForwardCommand.RaiseCanExecuteChanged();
        }
    }
}
