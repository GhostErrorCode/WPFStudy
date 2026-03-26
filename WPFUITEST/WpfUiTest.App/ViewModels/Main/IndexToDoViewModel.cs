using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;
using System.Text;
using System.Windows;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;
using WpfUiTest.App.ViewModels.Mapping;
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
        // 命令：修改待办事项Command
        public AsyncRelayCommand<IndexToDoItemViewModel> UpdateToDoItemCommand { get; private set; }
        // 命令：删除待办事项Command
        public AsyncRelayCommand<IndexToDoItemViewModel> DeleteToDoItemCommand { get; private set; }
        // 命令：一键完成待办事项Command
        public AsyncRelayCommand<IndexToDoItemViewModel> CompletedToDoItemCommand { get; private set; }


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
            this.UpdateToDoItemCommand = new AsyncRelayCommand<IndexToDoItemViewModel>(UpdateToDoItem);
            this.DeleteToDoItemCommand = new AsyncRelayCommand<IndexToDoItemViewModel>(DeleteToDoItem);
            this.CompletedToDoItemCommand = new AsyncRelayCommand<IndexToDoItemViewModel>(CompletedToDoItem);
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

        // 方法：修改待办事项
        private async Task UpdateToDoItem(IndexToDoItemViewModel? item)
        {
            try
            {
                // 如果传入的 IndexToDoItemViewModel 参数是NULL，就记录日志并直接返回
                if (item == null)
                {
                    this._logger.LogWarning("[首页（IndexView）] [用户：{Account}（{Id}）] 修改待办事项时失败。传入的待办事项数据参数为NULL", this._userService.UserAccount, this._userService.UserId);
                    this._messenger.ShowCaution(SnackbarTarget.MainView, "修改待办事项失败", "传入的待办事项数据参数为空");
                    return;
                }

                // 如果传入的参数不为NULL，就正常进行业务流程
                // 给属性 IndexToDoItem 赋值
                this.LoadIndexToDoItem(item);
                // 打开对话框并保存结果
                ContentDialogResult updateToDoContentDialogResult = await this._contentDialogService.ShowSimpleDialogAsync(new SimpleContentDialogCreateOptions()
                {
                    Title = "修改待办",
                    Content = ContentPresenterHelper.Build(this, Application.Current.Resources["ToDoContentDialog"]),
                    PrimaryButtonText = "修改",
                    SecondaryButtonText = "暂不修改",
                    CloseButtonText = "取消",
                });

                // 如果是点击的暂不修改，就直接关闭对话框，保留已写的数据
                // Warning（保留功能未实现，暂时不考虑保留功能）
                if (updateToDoContentDialogResult == ContentDialogResult.Secondary) { return; }
                // 如果是点击的取消，就关闭对话框并清除已写的数据
                if (updateToDoContentDialogResult == ContentDialogResult.None) { this.ClearIndexToDoItem(); return; }

                // 如果是点击的修改,就将变更信息写入数据库并修改当前的IndexToDoItems集合，这里不在用if
                // 调用后台服务修改待办事项
                ServiceResult<ToDoDto> updateToDoResult = await this._toDoService.UpdateToDoAsync(this._indexToDoItem.ToUpdateToDoDto());
                // 判断修改待办事项是否成功
                if (updateToDoResult != null && updateToDoResult.IsSuccess && updateToDoResult.Data != null)
                {
                    // 从当前的集合列表中找到需要修改的那个待办事项
                    IndexToDoItemViewModel? indexToDoItem = this.IndexToDoItems.FirstOrDefault(m => m.Id == updateToDoResult.Data.Id && m.UserId == updateToDoResult.Data.UserId);
                    // 判断是否在当前集合中找到，如果找到就修改它
                    if (indexToDoItem != null)
                    {
                        // 如果待办事项状态修改成了已完成，就从首页数据集合移除它，并重新计算汇总数据
                        if (updateToDoResult.Data.Status == TodoStatusEnum.Completed)
                        {
                            this.IndexToDoItems.Remove(indexToDoItem);
                            this._messenger.Send(new UpdateIndexSummaryMessage() { Type = UpdateIndexSummaryType.UpdateToDoCompleted });
                        }
                        else
                        {
                            // 如果只是修改了标题和内容，状态依旧是待办状态，那么只变更集合中的指定项即可
                            // 修改
                            indexToDoItem.Title = updateToDoResult.Data.Title;
                            indexToDoItem.Content = updateToDoResult.Data.Content;
                            indexToDoItem.Status = updateToDoResult.Data.Status;
                            indexToDoItem.UpdateDate = updateToDoResult.Data.UpdateDate;
                        }
                        // 清理IndexToDoItem
                        this.ClearIndexToDoItem();
                        this._logger.LogInformation("[首页（IndexView）] [用户：{Account}（{Id}）] 修改待办事项成功，已更新当前UI集合，ID={Id}，标题=\"{Title}\"", this._userService.UserAccount, this._userService.UserId, updateToDoResult.Data.Id, updateToDoResult.Data.Title);
                        this._messenger.ShowSuccess(SnackbarTarget.MainView, updateToDoResult.Message, "修改待办事项成功");
                    }
                    else
                    {
                        this.ClearIndexToDoItem();
                        this._logger.LogWarning("[首页（IndexView）] [用户：{Account}（{Id}）] 修改待办事项成功，但更新当前UI集合失败，ID={Id}，标题=\"{Title}\"", this._userService.UserAccount, this._userService.UserId, updateToDoResult.Data.Id, updateToDoResult.Data.Title);
                        this._messenger.ShowCaution(SnackbarTarget.MainView, updateToDoResult.Message, "修改待办事项成功，但更新当前集合失败");
                    }
                }
                else
                {
                    this._logger.LogWarning("[首页（IndexView）] [用户：{Account}（{Id}）] 修改待办事项失败，原因：{Reason}", this._userService.UserAccount, this._userService.UserId, updateToDoResult != null ? updateToDoResult.Message : "服务返回结果为空");
                    this._messenger.ShowCaution(SnackbarTarget.MainView, "修改待办事项失败", updateToDoResult != null ? updateToDoResult.Message : "修改待办事项失败");
                }
            }
            catch(Exception ex)
            {
                this._logger.LogError("[首页（IndexView）] [用户：{Account}（{Id}）] 修改待办事项时出现异常。异常信息：{ex}", this._userService.UserAccount, this._userService.UserId, ex);
                this._messenger.ShowDanger(SnackbarTarget.MainView, "修改待办事项失败", "修改待办事项时出现异常");
            }
        }

        // 方法：删除待办事项
        private async Task DeleteToDoItem(IndexToDoItemViewModel? item)
        {
            try
            {
                // 如果传入的 IndexToDoItemViewModel 参数是NULL，就记录日志并直接返回
                if (item == null)
                {
                    this._logger.LogWarning("[首页（IndexView）] [用户：{Account}（{Id}）] 删除待办事项时失败。传入的待办事项数据参数为NULL", this._userService.UserAccount, this._userService.UserId);
                    this._messenger.ShowCaution(SnackbarTarget.MainView, "删除待办事项失败", "传入的待办事项数据参数为空");
                    return;
                }

                // 如果传入的参数不为NULL，就正常进行业务流程
                // 给属性 IndexToDoItem 赋值
                this.LoadIndexToDoItem(item);
                // 打开对话框并保存结果
                ContentDialogResult deleteToDoContentDialogResult = await this._contentDialogService.ShowAsync(new ContentDialog()
                {
                    Title = "删除待办？",
                    Content = new TextBlock
                    {
                        Text = $"是否删除此待办事项，此待办事项信息如下：{Environment.NewLine}" +
                               $"----- 状态 -----{Environment.NewLine}{(item.Status == TodoStatusEnum.Pending ? "待办" : "已完成")}{Environment.NewLine}{Environment.NewLine}" +
                               $"----- 标题 -----{Environment.NewLine}{item.Title}{Environment.NewLine}{Environment.NewLine}" +
                               $"----- 内容 -----{Environment.NewLine}{item.Content}",
                        TextWrapping = TextWrapping.Wrap
                    },
                    PrimaryButtonAppearance = ControlAppearance.Danger,
                    PrimaryButtonText = "删除",
                    SecondaryButtonText = "暂不删除",
                    CloseButtonText = "取消",
                    DialogMaxWidth = 650
                }, CancellationToken.None);

                // 如果是点击的暂不删除，就直接关闭对话框，保留已写的数据
                // Warning（保留功能未实现，暂时不考虑保留功能）
                if (deleteToDoContentDialogResult == ContentDialogResult.Secondary) { return; }
                // 如果是点击的取消，就关闭对话框并清除已写的数据
                if (deleteToDoContentDialogResult == ContentDialogResult.None) { this.ClearIndexToDoItem(); return; }

                // 调用后台服务删除待办事项
                ServiceResult<bool> deleteToDoResult = await this._toDoService.DeleteToDoAsync(this._indexToDoItem.ToDeleteToDoDto());
                // 判断删除待办事项是否成功
                if (deleteToDoResult != null && deleteToDoResult.IsSuccess && deleteToDoResult.Data == true)
                {
                    // 如果从数据库中删除成功就从当前集合中清除指定项
                    // 从当前的集合列表中找到需要删除的那个待办事项
                    IndexToDoItemViewModel? indexToDoItemViewModel = this.IndexToDoItems.FirstOrDefault(m => m.Id == item.Id && m.UserId == item.UserId && m.Title == item.Title);
                    // 判断是否在当前集合中找到，如果找到就删除它
                    if (indexToDoItemViewModel != null)
                    {
                        // 删除
                        this.IndexToDoItems.Remove(indexToDoItemViewModel);
                        // 清理IndexToDoItem
                        this.ClearIndexToDoItem();
                        // 重新计算汇总数据
                        this._messenger.Send(new UpdateIndexSummaryMessage() { Type = UpdateIndexSummaryType.DeleteToDo });
                        this._logger.LogInformation("[首页（IndexView）] [用户：{Account}（{Id}）] 删除待办事项成功，已从当前UI集合中清除，ID={Id}，标题=\"{Title}\"", this._userService.UserAccount, this._userService.UserId, item.Id, item.Title);
                        this._messenger.ShowSuccess(SnackbarTarget.MainView, deleteToDoResult.Message, "删除待办事项成功");
                    }
                    else
                    {
                        this.ClearIndexToDoItem();
                        this._logger.LogWarning("[首页（IndexView）] [用户：{Account}（{Id}）] 删除待办事项成功，但当前UI集合中清除失败，ID={Id}，标题=\"{Title}\"", this._userService.UserAccount, this._userService.UserId, item.Id, item.Title);
                        this._messenger.ShowCaution(SnackbarTarget.MainView, deleteToDoResult.Message, "删除待办事项成功，但从当前集合中清除失败");
                    }
                }
                else
                {
                    this._logger.LogWarning("[首页（IndexView）] [用户：{Account}（{Id}）] 删除待办事项失败，原因：{Reason}", this._userService.UserAccount, this._userService.UserId, deleteToDoResult != null ? deleteToDoResult.Message : "服务返回结果为空");
                    this._messenger.ShowCaution(SnackbarTarget.MainView, "删除待办事项失败", deleteToDoResult != null ? deleteToDoResult.Message : "删除待办事项失败");
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError("[首页（IndexView）] [用户：{Account}（{Id}）] 删除待办事项时出现异常。异常信息：{ex}", this._userService.UserAccount, this._userService.UserId, ex);
                this._messenger.ShowDanger(SnackbarTarget.MainView, "删除待办事项失败", "删除待办事项时出现异常");
            }
        }

        // 方法：一键完成待办事项
        private async Task CompletedToDoItem(IndexToDoItemViewModel? item)
        {
            try
            {
                // 如果传入的 IndexToDoItemViewModel 参数是NULL，就记录日志并直接返回
                if (item == null)
                {
                    this._logger.LogWarning("[首页（IndexView）] [用户：{Account}（{Id}）] 一键完成待办事项时失败。传入的待办事项数据参数为NULL", this._userService.UserAccount, this._userService.UserId);
                    this._messenger.ShowCaution(SnackbarTarget.MainView, "一键完成待办事项失败", "传入的待办事项数据参数为空");
                    return;
                }

                // 如果传入的参数不为NULL，就正常进行业务流程
                // 给属性 IndexToDoItem 赋值，并直接把待办事项状态改成已完成
                this.LoadIndexToDoItem(item);
                this.IndexToDoItem.Status = TodoStatusEnum.Completed;
                // 打开对话框并保存结果
                ContentDialogResult completedToDoContentDialogResult = await this._contentDialogService.ShowSimpleDialogAsync(new SimpleContentDialogCreateOptions()
                {
                    Title = "一键完成待办？",
                    Content = new TextBlock
                    {
                        Text = $"是否一键完成此待办事项，此待办事项信息如下：{Environment.NewLine}" +
                               $"----- 状态 -----{Environment.NewLine}{(item.Status == TodoStatusEnum.Pending ? "待办" : "已完成")}{Environment.NewLine}{Environment.NewLine}" +
                               $"----- 标题 -----{Environment.NewLine}{item.Title}{Environment.NewLine}{Environment.NewLine}" +
                               $"----- 内容 -----{Environment.NewLine}{item.Content}",
                        TextWrapping = TextWrapping.Wrap
                    },
                    PrimaryButtonText = "一键完成",
                    SecondaryButtonText = "暂不一键完成",
                    CloseButtonText = "取消",
                });

                // 如果是点击的暂不一键完成，就直接关闭对话框，保留已写的数据
                // Warning（保留功能未实现，暂时不考虑保留功能）
                if (completedToDoContentDialogResult == ContentDialogResult.Secondary) { return; }
                // 如果是点击的取消，就关闭对话框并清除已写的数据
                if (completedToDoContentDialogResult == ContentDialogResult.None) { this.ClearIndexToDoItem(); return; }

                // 如果是点击的一键完成,就将变更信息写入数据库并从当前的IndexToDoItems集合去除
                // 调用后台服务修改待办事项
                ServiceResult<ToDoDto> completedToDoResult = await this._toDoService.UpdateToDoAsync(this._indexToDoItem.ToUpdateToDoDto());
                // 判断修改待办事项是否成功
                if (completedToDoResult != null && completedToDoResult.IsSuccess && completedToDoResult.Data != null)
                {
                    // 从当前的集合列表中找到需要一键完成的那个待办事项
                    IndexToDoItemViewModel? indexToDoItem = this.IndexToDoItems.FirstOrDefault(m => m.Id == completedToDoResult.Data.Id && m.UserId == completedToDoResult.Data.UserId);
                    // 判断是否在当前集合中找到，如果找到就从当前集合去除
                    if (indexToDoItem != null)
                    {
  
                        this.IndexToDoItems.Remove(indexToDoItem);
                        this._messenger.Send(new UpdateIndexSummaryMessage() { Type = UpdateIndexSummaryType.UpdateToDoCompleted });

                        // 清理IndexToDoItem
                        this.ClearIndexToDoItem();
                        this._logger.LogInformation("[首页（IndexView）] [用户：{Account}（{Id}）] 一键完成待办事项成功，已更新当前UI集合，ID={Id}，标题=\"{Title}\"", this._userService.UserAccount, this._userService.UserId, completedToDoResult.Data.Id, completedToDoResult.Data.Title);
                        this._messenger.ShowSuccess(SnackbarTarget.MainView, "一键完成待办事项成功", "一键完成待办事项成功");
                    }
                    else
                    {
                        this.ClearIndexToDoItem();
                        this._logger.LogWarning("[首页（IndexView）] [用户：{Account}（{Id}）] 一键完成待办事项成功，但更新当前UI集合失败，ID={Id}，标题=\"{Title}\"", this._userService.UserAccount, this._userService.UserId, completedToDoResult.Data.Id, completedToDoResult.Data.Title);
                        this._messenger.ShowCaution(SnackbarTarget.MainView, completedToDoResult.Message, "一键完成待办事项成功，但更新当前集合失败");
                    }
                }
                else
                {
                    this._logger.LogWarning("[首页（IndexView）] [用户：{Account}（{Id}）] 一键完成待办事项失败，原因：{Reason}", this._userService.UserAccount, this._userService.UserId, completedToDoResult != null ? completedToDoResult.Message : "服务返回结果为空");
                    this._messenger.ShowCaution(SnackbarTarget.MainView, "删除待办事项失败", completedToDoResult != null ? completedToDoResult.Message : "一键完成待办事项失败");
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError("[首页（IndexView）] [用户：{Account}（{Id}）] 一键完成待办事项时出现异常。异常信息：{ex}", this._userService.UserAccount, this._userService.UserId, ex);
                this._messenger.ShowDanger(SnackbarTarget.MainView, "一键完成待办事项失败", "一键完成待办事项时出现异常");
            }
        }

        // 方法：填充修改/删除的待办事项列表项 至 IndexToDoItem
        private void LoadIndexToDoItem(IndexToDoItemViewModel IndexToDoItemViewModel)
        {
            this.IndexToDoItem.Id = IndexToDoItemViewModel.Id;
            this.IndexToDoItem.UserId = IndexToDoItemViewModel.UserId;
            this.IndexToDoItem.Title = IndexToDoItemViewModel.Title;
            this.IndexToDoItem.Content = IndexToDoItemViewModel.Content;
            this.IndexToDoItem.Status = IndexToDoItemViewModel.Status;
            this.IndexToDoItem.CreateDate = IndexToDoItemViewModel.CreateDate;
            this.IndexToDoItem.UpdateDate = IndexToDoItemViewModel.UpdateDate;
        }
        // 方法：清理添加/修改的待办事项列表项IndexToDoItem
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
