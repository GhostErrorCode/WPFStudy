using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using WpfUiTest.Core.Services.Interfaces;
using WpfUiTest.Shared.Base;

namespace WpfUiTest.App.ViewModels.Main
{
    // 首页的ViewModel
    public class IndexViewModel : BaseViewModel
    {
        // ==================== 字段属性命令 ====================
        // 字段：ILogger服务
        private readonly ILogger<IndexViewModel> _logger;
        // 字段：IUserService服务
        private readonly IUserService _userService;

        // 属性: 首页标题ViewModel
        private IndexTitleViewModel _indexTitleViewModel;
        public IndexTitleViewModel IndexTitleViewModel
        {
            get { return _indexTitleViewModel; }
            set { SetProperty(ref _indexTitleViewModel, value); }
        }
        // 属性: 首页汇总ViewModel
        private IndexSummaryViewModel _indexSummaryViewModel;
        public IndexSummaryViewModel IndexSummaryViewModel
        {
            get { return _indexSummaryViewModel; }
            set { SetProperty(ref _indexSummaryViewModel, value); }
        }
        // 属性：首页待办事项ViewModel
        private IndexToDoViewModel _indexToDoViewModel;
        public IndexToDoViewModel IndexToDoViewModel
        {
            get { return _indexToDoViewModel; }
            set { SetProperty(ref _indexToDoViewModel, value); }
        }
        // 属性：首页备忘录ViewModel
        private IndexMemoViewModel _memoViewModel;
        public IndexMemoViewModel MemoViewModel
        {
            get { return _memoViewModel; }
            set { SetProperty(ref _memoViewModel, value); }
        }

        // 属性：加载动画是否可见
        private bool _isLoading = false;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
        }
        // 属性：加载时提示文本
        private string _loadingText = "加载中...";
        public string LoadingText
        {
            get { return _loadingText; }
            set { SetProperty(ref _loadingText, value); }
        }

        // ==================== 构造函数 ====================
        public IndexViewModel(ILogger<IndexViewModel> logger, IUserService userService, IndexTitleViewModel indexTitleViewModel, IndexSummaryViewModel indexSummaryViewModel, IndexToDoViewModel indexToDoViewModel, IndexMemoViewModel indexMemoViewModel)
        {
            // 初始化字段
            this._logger = logger;
            this._userService = userService;
            this._indexTitleViewModel = indexTitleViewModel;
            this._indexSummaryViewModel = indexSummaryViewModel;
            this._indexToDoViewModel = indexToDoViewModel;
            this._memoViewModel = indexMemoViewModel;
            // 初始化属性

            // 初始化命令
        }


        // ==================== 方法 ====================
        // 方法：导航进入后执行
        public override async Task OnNavigatedToAsync()
        {
            try
            {
                // 显示加载动画
                this.IsLoading = true;
                this.LoadingText = "首页初始化中...";

                this._logger.LogInformation("[首页（IndexView）] [用户：{Account}（{Id}）] 导航进入，开始初始化...", this._userService.UserAccount, this._userService.UserId);
                // 调用子VM此方法
                await Task.WhenAll(
                    this._indexTitleViewModel.OnNavigatedToAsync(),
                    this._indexSummaryViewModel.OnNavigatedToAsync(),
                    this._indexToDoViewModel.OnNavigatedToAsync(),
                    this._memoViewModel.OnNavigatedToAsync()
                    );
            }
            catch (Exception ex)
            {
                this._logger.LogInformation("[首页（IndexView）] [用户：{Account}（{Id}）] 初始化失败。异常信息：{ex}", this._userService.UserAccount, this._userService.UserId, ex);
            }
            finally
            {
                // 关闭加载动画
                this.IsLoading = false;
            }
        }
        // 方法：导航离开后执行
        public override async Task OnNavigatedFromAsync()
        {
            this._logger.LogInformation("[首页（IndexView）] [用户：{Account}（{Id}）] 导航离开，开始清理...", this._userService.UserAccount, this._userService.UserId);

            // 调用子VM此方法
            await Task.WhenAll(
                this._indexTitleViewModel.OnNavigatedFromAsync(),
                this._indexSummaryViewModel.OnNavigatedFromAsync(),
                this._indexToDoViewModel.OnNavigatedFromAsync(),
                this._memoViewModel.OnNavigatedFromAsync()
                );
        }
    }
}
