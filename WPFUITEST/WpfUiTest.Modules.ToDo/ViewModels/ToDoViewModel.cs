using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml.XPath;
using Wpf.Ui;
using WpfUiTest.Core.DTOs.ToDo;
using WpfUiTest.Core.Services.Interfaces;
using WpfUiTest.Modules.ToDo.Mappers;
using WpfUiTest.Shared.Base;
using WpfUiTest.Shared.Enums;
using WpfUiTest.Shared.Extensions;
using WpfUiTest.Shared.Utilities;

namespace WpfUiTest.Modules.ToDo.ViewModels
{
    // 待办事项主VM（因此处不像首页复杂，故不再将CRUD操作拆分成多个子VM）
    public class ToDoViewModel : BaseViewModel
    {
        // ==================== 字段、属性和命令 ====================
        // 字段：待办事项服务
        private readonly IToDoService _toDoService;
        // 字段：IMessenger服务
        private readonly IMessenger _messenger;
        // 字段：ILogger服务
        private readonly ILogger<ToDoViewModel> _logger;
        // 字段：IUserServcei服务
        private readonly IUserService _userService;

        // 属性：待办状态ComboBox源（ComboBox不支持选择NULL项，会出现选不中的情况，这里使用字符串All代替NULL）
        public ObservableCollection<object> TodoStatusOptions { get; } = new ObservableCollection<object>() { "All", TodoStatusEnum.Pending, TodoStatusEnum.Completed };
        //  属性：搜索标题
        private string _searchTitle = string.Empty;
        public string SearchTitle
        {
            get { return _searchTitle; }
            set { SetProperty(ref _searchTitle, value); }
        }
        //  属性：搜索内容
        private string _searchContent = string.Empty;
        public string SearchContent
        {
            get { return _searchContent; }
            set { SetProperty(ref _searchContent, value); }
        }
        //  属性：搜索状态
        private TodoStatusEnum? _searchStatus = null;
        public TodoStatusEnum? SearchStatus
        {
            get { return _searchStatus; }
            set { SetProperty(ref _searchStatus, value); }
        }

        // 属性：分页数据结果总条数
        private int _totalCount = 0;
        public int TotalCount
        {
            get { return _totalCount; }
            set { SetProperty(ref _totalCount, value); }
        }
        // 属性：分页大小ComboBox源
        public List<int> PageSizeOptions { get; } = new List<int>() { 10, 20, 30, 40, 50, 80, 100 };
        // 属性：分页大小
        private int _pageSize = 20;
        public int PageSize
        {
            get { return _pageSize; }
            set { SetProperty(ref _pageSize, value); }
        }
        // 属性：当前页码
        private int _pageIndex = 1;
        public int PageIndex
        {
            get { return _pageIndex; }
            set { SetProperty(ref _pageIndex, value); }
        }

        // 属性：加载动画是否显示
        private bool _isLoading = false;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
        }
        // 属性：加载提示文本
        private string _loadingText = "加载中...";
        public string LoadingText
        {
            get { return _loadingText; }
            set { SetProperty(ref _loadingText, value); }
        }

        // 属性：用于显示待办事项数据集合
        private ObservableCollection<ToDoItemViewModel> _ToDoItems = new ObservableCollection<ToDoItemViewModel>();
        public ObservableCollection<ToDoItemViewModel> ToDoItems
        {
            get { return _ToDoItems; }
            set { SetProperty(ref _ToDoItems, value); }
        }
        // 属性：用于添加、修改或删除的数据项
        private ToDoItemViewModel _toDoItem = new ToDoItemViewModel();
        public ToDoItemViewModel ToDoItem
        {
            get { return _toDoItem; }
            set { SetProperty(ref _toDoItem, value); }
        }


        // 命令：搜索Command
        public AsyncRelayCommand SearchCommand { get; private set; }
        // 命令：搜索重置Command
        public AsyncRelayCommand SearchResetCommand { get; private set; }
        // ==================== 构造函数 ====================
        public ToDoViewModel(IToDoService toDoService, IMessenger messenger, ILogger<ToDoViewModel> logger, IUserService userService)
        {
            // 初始化字段
            this._toDoService = toDoService;
            this._messenger = messenger;
            this._logger = logger;
            this._userService = userService;

            // 初始化属性

            // 初始化命令
            this.SearchCommand = new AsyncRelayCommand(LoadPagedToDosAsync);
            this.SearchResetCommand = new AsyncRelayCommand(async () => 
            {
                this.SearchTitle = string.Empty;
                this.SearchContent = string.Empty;
                this.SearchStatus = null;
                await LoadPagedToDosAsync();
            });
        }

        // ==================== 方法 ====================
        // 方法：分页多条件查询
        private async Task LoadPagedToDosAsync()
        {
            try
            {
                // 显示加载动画
                this.IsLoading = true;
                this.LoadingText = "分页查询中...";

                //  清空当前集合
                this.ToDoItems.Clear();

                // 从数据库中根据条件并分页拿取数据集合
                ServiceResult<PagedResult<ToDoDto>> result = await this._toDoService.GetPagedToDosAsync(new PagedQueryToDoDto()
                {
                    PageIndex = this._pageIndex,
                    PageSize = this._pageSize,
                    Title = this._searchTitle,
                    Content = this._searchContent,
                    Status = this._searchStatus
                });
                // 判断返回的结果是否成功
                if(result != null && result.IsSuccess)
                {
                    // 判断返回的结果是否存在数据Data，如果存在，则开始将查到的数据添加至数据集合中
                    if(result.Data != null)
                    {
                        // 1.给数据总条数赋值
                        this.TotalCount = result.Data.TotalCount;
                        // 2.给数据集合赋值
                        foreach(ToDoDto toDoDto in result.Data.Items)
                        {
                            this.ToDoItems.Add(toDoDto.ToToDoItemViewModel());
                        }
                        this._logger.LogInformation("[ToDoView] [用户：{Account}（{Id}）] 分页查询待办事项数据绑定UI成功，页码={PageIndex}，页大小={PageSize}。数据条数={Count}", this._userService.UserAccount, this._userService.UserId, this._pageIndex, this._pageSize, result.Data.Items.Count);
                    }
                    else
                    {
                        // 打印日志
                        this._logger.LogWarning("[ToDoView] [用户：{Account}（{Id}）] 分页查询待办事项数据加载成功。无可用数据", this._userService.UserAccount, this._userService.UserId);
                    }
                }
                else
                {
                    // 打印日志
                    this._logger.LogError("[ToDoView] [用户：{Account}（{Id}）]！分页查询待办事项数据失败。{Message}", this._userService.UserAccount, this._userService.UserId, result == null ? "调用Service失败" : result.Message);
                    this._messenger.ShowDanger(SnackbarTarget.MainView, "数据加载失败！", "分页查询待办事项数据失败");
                }
            }
            catch (Exception ex)
            {
                // 打印日志
                this._logger.LogError("[ToDoView] [用户：{Account}（{Id}）] 分页查询待办事项数据时出现异常，页码={PageIndex}，页大小={PageSize}。异常信息：{ex}", this._userService.UserAccount, this._userService.UserId, this._pageIndex, this._pageSize, ex);
                this._messenger.ShowDanger(SnackbarTarget.MainView, "数据加载失败", "分页查询待办事项数据失败");
            }
            finally
            {
                // 隐藏加载动画
                this.IsLoading = false;
            }
        }

        // 私有方法：初始化VM
        private async Task InitAsync()
        {
            // 调用分页多条件查询，直接完成页面进入时的数据加载
            await this.LoadPagedToDosAsync();
        }
        // 私有方法：清理VM
        private async Task CleanupASync()
        {

        }

        // 方法：导航进入后执行
        public override async Task OnNavigatedToAsync()
        {
            this._logger.LogInformation("[ToDoView] [用户：{Account}（{Id}）] 导航进入，开始初始化...", this._userService.UserAccount, this._userService.UserId);
            // 执行初始化方法
            await this.InitAsync();
        }
        // 方法：导航离开后执行
        public override async Task OnNavigatedFromAsync()
        {
            this._logger.LogInformation("[ToDoView] [用户：{Account}（{Id}）] 导航离开，清理缓存...", this._userService.UserAccount, this._userService.UserId);
            // 执行清理方法
            await this.CleanupASync();
        }
    }
}
