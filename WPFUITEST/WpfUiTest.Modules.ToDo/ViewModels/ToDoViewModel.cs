using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection.Metadata;
using System.Text;
using System.Xml.XPath;
using Wpf.Ui;
using WpfUiTest.Core.DTOs.ToDo;
using WpfUiTest.Core.Services.Interfaces;
using WpfUiTest.Modules.ToDo.Mappers;
using WpfUiTest.Shared.Base;
using WpfUiTest.Shared.Enums;
using WpfUiTest.Shared.Extensions;
using WpfUiTest.Shared.Messages;
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
        // 属性：新增待办事项抽屉是否显示
        private bool _isAddDrawerOpen = false;
        public bool IsAddDrawerOpen
        {
            get { return _isAddDrawerOpen; }
            set { SetProperty(ref _isAddDrawerOpen, value); }
        }
        // 属性：修改待办事项抽屉是否显示
        private bool _isEditDrawerOpen = false;
        public bool IsEditDrawerOpen
        {
            get { return _isEditDrawerOpen; }
            set { SetProperty(ref _isEditDrawerOpen, value); }
        }
        // 属性：删除待办事项抽屉是否显示
        private bool isDeleteDrawerOpen = false;
        public bool IsDeleteDrawerOpen
        {
            get { return isDeleteDrawerOpen; }
            set { SetProperty(ref isDeleteDrawerOpen, value); }
        }
        // 属性：抽屉的ComboBox源
        public List<TodoStatusEnum> DrawerTodoStatusOptions { get; } = new List<TodoStatusEnum> { TodoStatusEnum.Pending, TodoStatusEnum.Completed };


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


        // 命令：打开新增待办事项抽屉
        public RelayCommand OpenAddDrawerCommand { get; private set; }
        // 命令：关闭新增待办事项抽屉
        public RelayCommand CloseAddDrawerCommand { get; private set; }
        // 命令：打开修改待办事项抽屉
        public RelayCommand<ToDoItemViewModel> OpenEditDrawerCommand { get; private set; }
        // 命令：关闭修改待办事项抽屉
        public RelayCommand CloseEditDrawerCommand { get; private set; }
        // 命令：打开删除待办事项抽屉
        public RelayCommand<ToDoItemViewModel> OpenDeleteDrawerCommand { get; private set; }
        // 命令：关闭删除待办事项抽屉
        public RelayCommand CloseDeleteDrawerCommand { get; private set; }

        // 命令：搜索Command
        public AsyncRelayCommand SearchCommand { get; private set; }
        // 命令：搜索重置Command
        public AsyncRelayCommand SearchResetCommand { get; private set; }
        // 命令：添加待办事项Command
        public AsyncRelayCommand AddToDoItemCommand { get; private set; }
        // 命令：修改待办事项Command
        public AsyncRelayCommand UpdateToDoItemCommand { get; private set; }
        // 命令：删除待办事项Command
        public AsyncRelayCommand DeleteToDoItemCommand { get; private set; }
        // 命令：完成待办事项Command
        public AsyncRelayCommand<ToDoItemViewModel> CompletedToDoItemCommand { get; private set; }
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
            // 抽屉相关命令（使用 null 包容运算符 !，这样可以消除警告）
            this.OpenAddDrawerCommand = new RelayCommand(() => { this.ClearToDoItem(); this.IsAddDrawerOpen = true; });
            this.CloseAddDrawerCommand = new RelayCommand(() => { this.IsAddDrawerOpen = false; });
            this.OpenEditDrawerCommand = new RelayCommand<ToDoItemViewModel>((toDoItem) => { this.LoadToDoItem(toDoItem!); this.IsEditDrawerOpen = true; });
            this.CloseEditDrawerCommand = new RelayCommand(() => { this.IsEditDrawerOpen = false; });
            this.OpenDeleteDrawerCommand = new RelayCommand<ToDoItemViewModel>((toDoItem) => { this.LoadToDoItem(toDoItem!); this.IsDeleteDrawerOpen = true; });
            this.CloseDeleteDrawerCommand = new RelayCommand(() => { this.IsDeleteDrawerOpen = false; });
            // CRUD命令
            this.AddToDoItemCommand = new AsyncRelayCommand(AddToDoItem);
            this.UpdateToDoItemCommand = new AsyncRelayCommand(UpdateToDoItem);
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

        // 方法：添加待办事项
        private async Task AddToDoItem()
        {
            try
            {
                // 调用后台服务添加待办事项
                ServiceResult<ToDoDto> addToDoResult = await this._toDoService.AddToDoAsync(this._toDoItem.ToAddToDoDto());
                // 判断添加待办事项是否成功
                if (addToDoResult != null && addToDoResult.IsSuccess && addToDoResult.Data != null)
                {
                    /*
                    // 添加到 ObservableCollection 集合第一位，这样就符合倒序排序了
                    this.ToDoItems.Insert(0, addToDoResult.Data.ToToDoItemViewModel());
                    // 当前TotalCount + 1
                    this.TotalCount++;
                    */
                    // 日志
                    this._logger.LogInformation("[ToDoView] [用户：{Account}（{Id}）] 添加待办事项成功，已添加至当前UI集合，ID={Id}，标题=\"{Title}\"", this._userService.UserAccount, this._userService.UserId, addToDoResult.Data.Id, addToDoResult.Data.Title);
                    this._messenger.ShowSuccess(SnackbarTarget.MainView, addToDoResult.Message, "添加待办事项成功");
                    // 跳转到第一页，因为默认按时间降序，所以新数据会出现在第一页
                    this.PageIndex = 1;
                    // 随后查询查询数据库，确保数据一致性
                    await this.LoadPagedToDosAsync();
                    // 关闭抽屉
                    this.IsAddDrawerOpen = false;
                    // 清除 ToDoItem
                    this.ClearToDoItem();
                }
                else
                {
                    this._logger.LogWarning("[ToDoView] [用户：{Account}（{Id}）] 添加待办事项失败，异常信息：{Message}", this._userService.UserAccount, this._userService.UserId, addToDoResult != null ? addToDoResult.Message : "服务返回结果为空");
                    this._messenger.ShowCaution(SnackbarTarget.MainView, "添加待办事项失败", addToDoResult != null ? addToDoResult.Message : "添加待办事项失败");
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError("[ToDoView] [用户：{Account}（{Id}）] 添加待办事项时出现异常。异常信息：{ex}", this._userService.UserAccount, this._userService.UserId, ex);
                this._messenger.ShowDanger(SnackbarTarget.MainView, "添加待办事项失败", "添加待办事项时出现异常");
            }
        }

        // 方法：修改待办事项
        private async Task UpdateToDoItem()
        {
            try
            {
                // 调用后台服务修改待办事项
                ServiceResult<ToDoDto> updateToDoResult = await this._toDoService.UpdateToDoAsync(this._toDoItem.ToUpdateToDoDto());
                // 判断修改待办事项是否成功
                if (updateToDoResult != null && updateToDoResult.IsSuccess && updateToDoResult.Data != null)
                {

                    this._logger.LogInformation("[ToDoView] [用户：{Account}（{Id}）] 修改待办事项成功，已更新当前UI集合，ID={Id}，标题=\"{Title}\"", this._userService.UserAccount, this._userService.UserId, updateToDoResult.Data.Id, updateToDoResult.Data.Title);
                    this._messenger.ShowSuccess(SnackbarTarget.MainView, updateToDoResult.Message, "修改待办事项成功");

                    // 重新查询数据库，确保数据一致性
                    await this.LoadPagedToDosAsync();
                    // 关闭抽屉
                    this.IsEditDrawerOpen = false;
                    // 清除 ToDoItem
                    this.ClearToDoItem();
                }
                else
                {
                    this._logger.LogWarning("[ToDoView] [用户：{Account}（{Id}）] 修改待办事项失败，原因：{Reason}", this._userService.UserAccount, this._userService.UserId, updateToDoResult != null ? updateToDoResult.Message : "服务返回结果为空");
                    this._messenger.ShowCaution(SnackbarTarget.MainView, "修改待办事项失败", updateToDoResult != null ? updateToDoResult.Message : "修改待办事项失败");
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError("[ToDoView] [用户：{Account}（{Id}）] 修改待办事项时出现异常。异常信息：{ex}", this._userService.UserAccount, this._userService.UserId, ex);
                this._messenger.ShowDanger(SnackbarTarget.MainView, "修改待办事项失败", "修改待办事项时出现异常");
            }
        }


        // 方法：装载需要修改/删除的待办事项数据至ToDoItem中
        private void LoadToDoItem(ToDoItemViewModel toDoItemViewModel)
        {
            this.ToDoItem.Id = toDoItemViewModel.Id;
            this.ToDoItem.UserId = toDoItemViewModel.UserId;
            this.ToDoItem.Title = toDoItemViewModel.Title;
            this.ToDoItem.Content = toDoItemViewModel.Content;
            this.ToDoItem.Status = toDoItemViewModel.Status;
            this.ToDoItem.CreateDate = toDoItemViewModel.CreateDate;
            this.ToDoItem.UpdateDate = toDoItemViewModel.UpdateDate;
        }
        // 方法：清理ToDoItem，恢复至默认值，以便添加新的待办事项
        private void ClearToDoItem()
        {
            this.ToDoItem.Id = 0;
            this.ToDoItem.UserId = 0;
            this.ToDoItem.Title = string.Empty;
            this.ToDoItem.Content = string.Empty;
            this.ToDoItem.Status = TodoStatusEnum.Pending;
            this.ToDoItem.CreateDate = DateTime.MinValue;
            this.ToDoItem.UpdateDate = DateTime.MinValue;
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
