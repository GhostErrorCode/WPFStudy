using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using WpfUiTest.App.ViewModels.User;
using WpfUiTest.App.Views.Main;
using WpfUiTest.Shared.Base;
using WpfUiTest.Shared.Utilities;

namespace WpfUiTest.App.ViewModels.Main
{
    // 主页面的ViewModel
    public class MainViewModel : BaseViewModel
    {
        // 字段：导航服务
        private readonly INavigationService _navigationService;


        // 属性：主页面Footer区域的菜单
        // 属性：MainView的Footer区域菜单列表
        private MainFooterMenuViewModel _mainFooterMenuViewModel;
        public MainFooterMenuViewModel MainFooterMenuViewModel
        {
            get { return _mainFooterMenuViewModel; }
            set { SetProperty(ref _mainFooterMenuViewModel, value); }
        }

        // 属性：主页面导航区域的菜单
        private MainMenuViewModel _mainMenuViewModel;
        public MainMenuViewModel MainMenuViewModel
        {
            get { return _mainMenuViewModel; }
            set { SetProperty(ref _mainMenuViewModel, value); }
        }

        // 属性：主页面导航区域的自动搜索框
        private MainAutoSuggestBoxViewModel _mainAutoSuggestBoxViewModel;
        public MainAutoSuggestBoxViewModel MainAutoSuggestBoxViewModel
        {
            get { return _mainAutoSuggestBoxViewModel; }
            set { SetProperty(ref _mainAutoSuggestBoxViewModel, value); }
        }



        // ============ 构造函数 ==============
        public MainViewModel(MainFooterMenuViewModel mainFooterMenuViewModel, MainMenuViewModel mainMenuViewModel, MainAutoSuggestBoxViewModel mainAutoSuggestBoxViewModel, INavigationService navigationService)
        {
            // 初始化字段
            this._navigationService = navigationService;

            // 初始化属性
            // this._mainFooterMenuViewModel = this._serviceProvider.GetRequiredService<MainFooterMenuViewModel>();
            this._mainFooterMenuViewModel = mainFooterMenuViewModel;
            this._mainMenuViewModel = mainMenuViewModel;
            this._mainAutoSuggestBoxViewModel = mainAutoSuggestBoxViewModel;

            // 初始化命令

            // 默认导航到首页
        }



        // ================== 方法 ===================
    }
}
