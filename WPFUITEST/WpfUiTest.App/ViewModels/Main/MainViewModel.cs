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
using WpfUiTest.Shared.Base;
using WpfUiTest.Shared.Utilities;

namespace WpfUiTest.App.ViewModels.Main
{
    // 主页面的ViewModel
    public class MainViewModel : BaseViewModel
    {
        // 字段：IServiceProvider服务
        // private readonly IServiceProvider _serviceProvider;


        // 属性：主页面Footer区域的菜单
        // 属性：MainView的Footer区域菜单列表
        private MainFooterMenuViewModel _mainFooterMenuViewModel;
        public MainFooterMenuViewModel MainFooterMenuViewModel
        {
            get { return _mainFooterMenuViewModel; }
            set { SetProperty(ref _mainFooterMenuViewModel, value); }
        }

        // 属性：主页面Footer区域的菜单
        // 属性：MainView的Footer区域菜单列表
        private MainMenuViewModel _mainMenuViewModel;
        public MainMenuViewModel MainMenuViewModel
        {
            get { return _mainMenuViewModel; }
            set { SetProperty(ref _mainMenuViewModel, value); }
        }



        // ============ 构造函数 ==============
        public MainViewModel(MainFooterMenuViewModel mainFooterMenuViewModel, MainMenuViewModel mainMenuViewModel)
        {
            // 初始化字段
            // this._serviceProvider = serviceProvider;

            // 初始化属性
            // this._mainFooterMenuViewModel = this._serviceProvider.GetRequiredService<MainFooterMenuViewModel>();
            this._mainFooterMenuViewModel = mainFooterMenuViewModel;
            this._mainMenuViewModel = mainMenuViewModel;

            // 初始化命令

        }



        // ================== 方法 ===================
    }
}
