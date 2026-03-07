using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;
using System.Text;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;
using WpfUiTest.App.ViewModels.Mapping;
using WpfUiTest.Core.DTOs.Memo;
using WpfUiTest.Core.DTOs.ToDo;
using WpfUiTest.Core.Services.Interfaces;
using WpfUiTest.Shared.Base;
using WpfUiTest.Shared.Enums;
using WpfUiTest.Shared.Extensions;
using WpfUiTest.Shared.Messages;
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
            try
            {
                // 保存ContentDialogService服务结果
                ContentDialogResult addToDoContentDialogResult = await this._contentDialogService.ShowSimpleDialogAsync(new SimpleContentDialogCreateOptions()
                {
                    Title = "添加待办",
                    Content = ContentPresenterHelper.Build(this, content),
                    PrimaryButtonText = "添加",
                    SecondaryButtonText = "暂不添加",
                    CloseButtonText = "取消",
                });

                // 判断是否保存填写的待办事项数据
                // 如果是点击的添加,就写入数据库变添加到当前的IndexToDoItems集合
                if (addToDoContentDialogResult == ContentDialogResult.Primary)
                {
                    // 调用后台服务添加待办事项
                    ServiceResult<ToDoDto> addToDoResult = await this._toDoService.AddToDoAsync(this._indexToDoItem.ToAddToDoDto());
                    // 判断添加待办事项是否成功
                    if (addToDoResult != null && addToDoResult.IsSuccess && addToDoResult.Data != null)
                    {
                        // 判断新增的待办事项状态是待办还是已完成
                        if (addToDoResult.Data.Status == TodoStatusEnum.Pending)
                        {
                            // 如果是待办状态，则添加到首页ListView列表中以及刷新首页汇总数据
                            // 添加到 ObservableCollection 集合第一位，这样就符合倒序排序了
                            this.IndexToDoItems.Insert(0, addToDoResult.Data.ToIndexToDoItemViewModel());
                            // this.IndexToDoItems.Add(addToDoResult.Data.ToIndexToDoItemViewModel());
                            this._messenger.Send(new UpdateIndexSummaryMessage() { Type = UpdateIndexSummaryType.AddToDo });
                        }
                        else
                        {
                            // 如果是已完成状态，则只刷新首页汇总数据
                            this._messenger.Send(new UpdateIndexSummaryMessage() { Type = UpdateIndexSummaryType.AddToDoCompleted });
                        }

                        // 清除 IndexToDoItem
                        this.ClearIndexToDoItem();
                        // 日志
                        this._logger.LogInformation("[首页（IndexView）] [用户：{Account}（{Id}）] 添加待办事项成功，已添加至当前UI集合，ID={Id}，标题=\"{Title}\"", this._userService.UserAccount, this._userService.UserId, addToDoResult.Data.Id, addToDoResult.Data.Title);
                        this._messenger.ShowSuccess(SnackbarTarget.MainView, addToDoResult.Message, "添加待办事项成功");
                    }
                    else
                    {
                        this._logger.LogWarning("[首页（IndexView）] [用户：{Account}（{Id}）] 添加待办事项失败，异常信息：{Message}", this._userService.UserAccount, this._userService.UserId, addToDoResult != null ? addToDoResult.Message : "服务返回结果为空");
                        this._messenger.ShowCaution(SnackbarTarget.MainView, "添加待办事项失败", addToDoResult != null ? addToDoResult.Message : "添加待办事项失败");
                    }
                }
                // 如果是点击的暂不添加，就直接关闭对话框，保留已写的数据
                // Warning（保留功能未实现，暂时不考虑保留功能）
                if (addToDoContentDialogResult == ContentDialogResult.Secondary) { return; }
                // 如果是点击的取消，就关闭对话框并清除已写的数据
                if (addToDoContentDialogResult == ContentDialogResult.None) { this.ClearIndexToDoItem(); return; }
            }
            catch (Exception ex)
            {
                this._logger.LogError("[首页（IndexView）] [用户：{Account}（{Id}）] 添加待办事项时出现异常。异常信息：{ex}", this._userService.UserAccount, this._userService.UserId, ex);
                this._messenger.ShowDanger(SnackbarTarget.MainView, "添加待办事项失败", "添加待办事项时出现异常");
            }
        }

        // 方法：清理添加/修改的待办事项列表项IndexMemoItem
        private void ClearIndexToDoItem()
        {
            this.IndexToDoItem.Id = 0;
            this.IndexToDoItem.UserId = 0;
            this.IndexToDoItem.Title = string.Empty;
            this.IndexToDoItem.Content = string.Empty;
            this.IndexToDoItem.Status = TodoStatusEnum.Pending;
            this.IndexToDoItem.CreateDate = DateTime.MinValue;
            this.IndexToDoItem.UpdateDate = DateTime.MinValue;
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
