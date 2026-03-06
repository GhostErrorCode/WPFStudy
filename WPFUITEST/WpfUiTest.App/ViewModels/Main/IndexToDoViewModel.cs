using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;
using System.Text;
using Wpf.Ui;
using Wpf.Ui.Extensions;
using WpfUiTest.App.ViewModels.Mapping;
using WpfUiTest.Core.DTOs.ToDo;
using WpfUiTest.Core.Services.Interfaces;
using WpfUiTest.Shared.Base;
using WpfUiTest.Shared.Enums;
using WpfUiTest.Shared.Extensions;
using WpfUiTest.Shared.Utilities;

namespace WpfUiTest.App.ViewModels.Main
{
    // 首页待办事项ViewModel
    public class IndexToDoViewModel : BaseViewModel
    {
        // ==================== 字段、属性、命令 ====================
        // 字段：待办事项服务
        private readonly IToDoService _toDoService;
        // 字段：IMessenger服务
        private readonly IMessenger _messenger;
        // 字段：ILogger服务
        private readonly ILogger<IndexToDoViewModel> _logger;
        // 字段：IUserServcei服务
        private readonly IUserService _userService;
        // 字段：IContentDialogService对话服务
        private readonly IContentDialogService _contentDialogService;

        // 属性：首页未完成的待办事项列表
        private ObservableCollection<IndexToDoItemViewModel> _indexToDoItems;
        public ObservableCollection<IndexToDoItemViewModel> IndexToDoItems
        {
            get { return _indexToDoItems; }
            set { SetProperty(ref _indexToDoItems, value); }
        }
        // 属性（固定）：添加或修改时候的待办事项状态组合框源数据
        public List<TodoStatusEnum> ToDoStatuses { get; } = new List<TodoStatusEnum>() { TodoStatusEnum.Pending, TodoStatusEnum.Completed };

        // 属性：要添加或修改的待办事项列表项
        private IndexToDoItemViewModel _indexToDoItem;
        public IndexToDoItemViewModel IndexToDoItem
        {
            get { return _indexToDoItem; }
            set { SetProperty(ref _indexToDoItem, value); }
        }

        // 命令：添加待办事项Command
        public AsyncRelayCommand<object> AddToDoItemCommand { get; private set; }


        // ==================== 构造函数 ====================
        public IndexToDoViewModel(IToDoService toDoService, IMessenger messenger, ILogger<IndexToDoViewModel> logger, IUserService userService, IContentDialogService contentDialogService)
        {
            // 初始化字段
            this._indexToDoItems = new ObservableCollection<IndexToDoItemViewModel>();
            this._indexToDoItem = new IndexToDoItemViewModel();

            this._toDoService = toDoService;
            this._messenger = messenger;
            this._logger = logger;
            this._userService = userService;
            this._contentDialogService = contentDialogService;

            // 初始化属性
            this.IndexToDoItem.Status = TodoStatusEnum.Pending;  // 添加或修改的待办事项永远有一个默认值：待办状态

            // 初始化命令
            this.AddToDoItemCommand = new AsyncRelayCommand<object>(AddToDoItem);
        }

        // ==================== 方法 ====================
        // 方法：添加待办事项
        private async Task AddToDoItem(object content)
        {
            await this._contentDialogService.ShowSimpleDialogAsync(new SimpleContentDialogCreateOptions()
            {
                Title = "添加待办",
                Content = ContentPresenterHelper.Build(this, content),
                PrimaryButtonText = "添加",
                SecondaryButtonText = "暂不添加",
                CloseButtonText = "取消",
            });
        }

        // 私有方法：初始化VM
        private async Task Init()
        {
            try
            {
                // 1.从数据库拿到当前用户未完成的待办事项集合，并清空当前集合
                this.IndexToDoItems.Clear();
                ServiceResult<List<ToDoDto>> allPendingToDosResult = await this._toDoService.GetAllPendingToDosAsync();
                // 判断是否获取成功并且数据不为空
                if (allPendingToDosResult != null && allPendingToDosResult.IsSuccess)
                {
                    // 成功获取到数据之后，放到集合
                    if (allPendingToDosResult.Data != null)
                    {
                        foreach (ToDoDto toDoDto in allPendingToDosResult.Data)
                        {
                            this.IndexToDoItems.Add(toDoDto.ToIndexToDoItemViewModel());
                        }
                        // 打印日志
                        this._logger.LogInformation("[首页（IndexView）] [用户：{Account}（{Id}）] 未完成待办事项数据绑定UI成功。数据条数={Count}", this._userService.UserAccount, this._userService.UserId, allPendingToDosResult.Data.Count);
                    }
                    else
                    {
                        // 打印日志
                        this._logger.LogWarning("[首页（IndexView）] [用户：{Account}（{Id}）] 未完成待办事项数据加载成功。无可用数据", this._userService.UserAccount, this._userService.UserId);
                    }
                }
                else
                {
                    // 打印日志
                    this._logger.LogError("[首页（IndexView）] [用户：{Account}（{Id}）]！未完成待办事项数据加载失败。{Message}", this._userService.UserAccount, this._userService.UserId, allPendingToDosResult == null ? "调用Service失败" : allPendingToDosResult.Message);
                    this._messenger.ShowDanger(SnackbarTarget.MainView, "数据加载失败！", "未完成待办事项数据加载失败");
                }
            }
            catch (Exception ex)
            {
                // 打印日志
                this._logger.LogError("[首页（IndexView）] [用户：{Account}（{Id}）]！未完成待办事项数据加载时出现异常。异常信息：{ex}", this._userService.UserAccount, this._userService.UserId, ex);
                this._messenger.ShowDanger(SnackbarTarget.MainView, "数据加载失败！", "未完成待办事项数据加载失败");
            }
        }
        // 私有方法：清理VM
        private async Task Cleanup()
        {
            int toDoItemsCount = this._indexToDoItems.Count;
            // 打印日志
            this._logger.LogInformation("[首页（IndexView）] [用户：{Account}（{Id}）] 未完成待办事项数据清理完成。清理条数={Count}", this._userService.UserAccount, this._userService.UserId, toDoItemsCount);
            this.IndexToDoItems.Clear();
        }


        // 方法：导航进入后执行
        public override async Task OnNavigatedToAsync()
        {
            // 执行初始化方法
            await this.Init();
        }
        // 方法：导航离开后执行
        public override async Task OnNavigatedFromAsync()
        {
            // 执行清理方法
            await this.Cleanup();
        }
    }
}
