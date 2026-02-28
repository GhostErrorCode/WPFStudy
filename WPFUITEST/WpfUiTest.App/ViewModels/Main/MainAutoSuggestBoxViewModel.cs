using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using Wpf.Ui;
using WpfUiTest.App.Views.Main;
using WpfUiTest.Core.Services.Interfaces;
using WpfUiTest.Modules.Memo.Views;
using WpfUiTest.Modules.ToDo.Views;
using WpfUiTest.Shared.Base;
using WpfUiTest.Shared.Enums;

namespace WpfUiTest.App.ViewModels.Main
{
    // MainView中NavigationView控件的自动搜索框ViewModel
    public class MainAutoSuggestBoxViewModel : BaseViewModel
    {
        // ==================== 字段属性 ====================
        // 字段: 导航服务
        private readonly INavigationService _navigationService;


        // 属性: 导航控件自动搜索框的建议项
        public ObservableCollection<string> AutoSuggestBoxSuggestions { get; set; } = new ObservableCollection<string>
        {
            "首页", "待办事项", "备忘录", "设置"
        };
        // 属性: 导航控件自动搜索框的当前选中项
        private string _currentSearchText = string.Empty;
        public string CurrentSearchText
        {
            get { return _currentSearchText; }
            set { SetProperty(ref _currentSearchText, value); }
        }

        // 命令: 自动搜索建议框选择项改变后Command
        public RelayCommand SuggestionChosenCommand { get; private set; }



        // ==================== 构造函数 ====================
        public MainAutoSuggestBoxViewModel(INavigationService navigationService)
        {
            // 初始化字段
            this._navigationService = navigationService;

            // 初始化属性

            // 初始化命令
            this.SuggestionChosenCommand = new RelayCommand(SuggestionChosen);
        }



        // ==================== 方法 ====================
        // 方法: 自动搜索建议框选择项改变方法
        private void SuggestionChosen()
        {
            // 根据当前选中项进行导航
            this._navigationService.Navigate(this._currentSearchText switch
            {
                "首页" => typeof(IndexView),
                "待办事项" => typeof(ToDoView),
                "备忘录" => typeof(MemoView),
                "设置" => typeof(SettingsView),
                _ => typeof(IndexView)
            });

        }
    }
}
