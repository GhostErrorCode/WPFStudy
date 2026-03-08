using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection.Metadata;
using System.Text;
using System.Windows;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;
using WpfUiTest.App.ViewModels.Mapping;
using WpfUiTest.Core.DTOs.Memo;
using WpfUiTest.Core.Services.Interfaces;
using WpfUiTest.Shared.Base;
using WpfUiTest.Shared.Enums;
using WpfUiTest.Shared.Extensions;
using WpfUiTest.Shared.Messages;
using WpfUiTest.Shared.Utilities;

namespace WpfUiTest.App.ViewModels.Main
{
    // 首页备忘录ViewModel
    public class IndexMemoViewModel : BaseViewModel
    {
        // ==================== 字段、属性、命令 ====================
        // 字段：备忘录服务
        private readonly IMemoService _memoService;
        // 字段：IMessenger服务
        private readonly IMessenger _messenger;
        // 字段：ILogger服务
        private readonly ILogger<IndexMemoViewModel> _logger;
        // 字段：IUserServcei服务
        private readonly IUserService _userService;
        // 字段：IContentDialogService对话服务
        private readonly IContentDialogService _contentDialogService;

        // 属性：首页的备忘录列表
        private ObservableCollection<IndexMemoItemViewModel> _indexMemoItems;
        public ObservableCollection<IndexMemoItemViewModel> IndexMemoItems
        {
            get { return _indexMemoItems; }
            set { SetProperty(ref _indexMemoItems, value); }
        }
        // 属性：要添加或修改的备忘录列表项
        private IndexMemoItemViewModel _indexMemoItem;
        public IndexMemoItemViewModel IndexMemoItem
        {
            get { return _indexMemoItem; }
            set { SetProperty(ref _indexMemoItem, value); }
        }

        // 命令：添加待办事项Command
        public AsyncRelayCommand<object> AddMemoItemCommand { get; private set; }
        // 命令：修改待办事项Command
        public AsyncRelayCommand<IndexMemoItemViewModel> UpdateMemoItemCommand { get; private set; }
        // 命令：删除待办事项Command
        public AsyncRelayCommand<IndexMemoItemViewModel> DeleteMemoItemCommand { get; private set; }


        // ==================== 构造函数 ====================
        public IndexMemoViewModel(IMemoService MemoService, IMessenger messenger, ILogger<IndexMemoViewModel> logger, IUserService userService, IContentDialogService contentDialogService)
        {
            // 初始化字段
            this._indexMemoItems = new ObservableCollection<IndexMemoItemViewModel>();
            this._indexMemoItem = new IndexMemoItemViewModel();

            this._memoService = MemoService;
            this._messenger = messenger;
            this._logger = logger;
            this._userService = userService;
            this._contentDialogService = contentDialogService;

            // 初始化属性

            // 初始化命令
            this.AddMemoItemCommand = new AsyncRelayCommand<object>(AddMemoItem);
            this.UpdateMemoItemCommand = new AsyncRelayCommand<IndexMemoItemViewModel>(UpdateMemoItem);
            this.DeleteMemoItemCommand = new AsyncRelayCommand<IndexMemoItemViewModel>(DeleteMemoItem);
        }

        // ==================== 方法 ====================
        // 方法：添加备忘录
        private async Task AddMemoItem(object content)
        {
            try
            {
                // 调用清理要添加/修改的备忘录列表项IndexMemoItem
                // this.ClearIndexMemoItem();
                // 设置对话框内容DataTemplate模板的DataContent数据上下文，如果转换DataTemplate失败，则依旧使用传入的content赋值
                // 保存ContentDialogService服务结果
                ContentDialogResult addMemoContentDialogResult = await this._contentDialogService.ShowSimpleDialogAsync(new SimpleContentDialogCreateOptions()
                {
                    Title = "添加备忘",
                    Content = ContentPresenterHelper.Build(this.IndexMemoItem, content),
                    PrimaryButtonText = "添加",
                    SecondaryButtonText = "暂不添加",
                    CloseButtonText = "取消",
                });

                // 判断是否保存填写的备忘录数据
                // 如果是点击的添加,就写入数据库变添加到当前的IndexMemoItems集合种
                if (addMemoContentDialogResult == ContentDialogResult.Primary)
                {
                    // 调用后台服务添加备忘录
                    ServiceResult<MemoDto> addMemoResult = await this._memoService.AddMemoAsync(this._indexMemoItem.ToAddMemoDto());
                    // 判断添加备忘录是否成功
                    if (addMemoResult != null && addMemoResult.IsSuccess && addMemoResult.Data != null)
                    {
                        // this.IndexMemoItems.Add(addMemoResult.Data.ToIndexMemoItemViewModel());
                        // 添加到 ObservableCollection 集合第一位，这样就符合倒序排序了
                        this.IndexMemoItems.Insert(0, addMemoResult.Data.ToIndexMemoItemViewModel());
                        // 通知首页汇总数据更新
                        this._messenger.Send(new UpdateIndexSummaryMessage() { Type = UpdateIndexSummaryType.AddMemo });
                        this.ClearIndexMemoItem();
                        this._logger.LogInformation("[首页（IndexView）] [用户：{Account}（{Id}）] 添加备忘录成功，已添加至当前UI集合，ID={Id}，标题=\"{Title}\"", this._userService.UserAccount, this._userService.UserId, addMemoResult.Data.Id, addMemoResult.Data.Title);
                        this._messenger.ShowSuccess(SnackbarTarget.MainView, addMemoResult.Message, "添加备忘录成功");
                    }
                    else
                    {
                        this._logger.LogWarning("[首页（IndexView）] [用户：{Account}（{Id}）] 添加备忘录失败，原因：{Reason}", this._userService.UserAccount, this._userService.UserId, addMemoResult != null ? addMemoResult.Message : "服务返回结果为空");
                        this._messenger.ShowCaution(SnackbarTarget.MainView, "添加备忘录失败", addMemoResult != null ? addMemoResult.Message : "添加备忘录失败");
                    }
                }
                // 如果是点击的暂不添加，就直接关闭对话框，保留已写的数据
                // Warning（保留功能未实现，暂时不考虑保留功能）
                if (addMemoContentDialogResult == ContentDialogResult.Secondary) { return; }
                // 如果是点击的取消，就关闭对话框并清除已写的数据
                if (addMemoContentDialogResult == ContentDialogResult.None) { this.ClearIndexMemoItem(); return; }
            }
            catch (Exception ex)
            {
                this._logger.LogError("[首页（IndexView）] [用户：{Account}（{Id}）] 添加备忘录时出现异常。异常信息：{ex}", this._userService.UserAccount, this._userService.UserId, ex);
                this._messenger.ShowDanger(SnackbarTarget.MainView, "添加备忘录失败", "添加备忘录时出现异常");
            }
        }

        // 方法：修改备忘录
        private async Task UpdateMemoItem(IndexMemoItemViewModel? item)
        {
            try
            {
                // 如果传入的 IndexMemoItemViewModel 参数是NULL，就记录日志并直接返回
                if (item == null)
                {
                    this._logger.LogWarning("[首页（IndexView）] [用户：{Account}（{Id}）] 修改备忘录时失败。传入的备忘录数据参数为NULL", this._userService.UserAccount, this._userService.UserId);
                    this._messenger.ShowCaution(SnackbarTarget.MainView, "修改备忘录失败", "传入的备忘录数据参数为空");
                    return;
                }

                // 如果传入的参数不为NULL，就正常进行业务流程
                // 给属性 IndexMemoItem 赋值
                this.LoadIndexMemoItem(item);
                // 打开对话框并保存结果
                ContentDialogResult updateMemoContentDialogResult = await this._contentDialogService.ShowSimpleDialogAsync(new SimpleContentDialogCreateOptions()
                {
                    Title = "修改备忘",
                    Content = ContentPresenterHelper.Build(this.IndexMemoItem, Application.Current.Resources["MemoContentDialog"]),
                    PrimaryButtonText = "修改",
                    SecondaryButtonText = "暂不修改",
                    CloseButtonText = "取消",
                });

                // 判断是否保存修改的备忘录数据
                // 如果是点击的修改,就将变更信息写入数据库并修改当前的IndexMemoItems集合
                if (updateMemoContentDialogResult == ContentDialogResult.Primary)
                {
                    // 调用后台服务修改备忘录
                    ServiceResult<MemoDto> updateMemoResult = await this._memoService.UpdateMemoAsync(this._indexMemoItem.ToUpdateMemoDto());
                    // 判断修改备忘录是否成功
                    if(updateMemoResult != null && updateMemoResult.IsSuccess && updateMemoResult.Data != null)
                    {
                        // 从当前的集合列表中找到需要修改的那个备忘录
                        IndexMemoItemViewModel? indexMemoItemViewModel = this.IndexMemoItems.FirstOrDefault(m=>m.Id == updateMemoResult.Data.Id && m.UserId == updateMemoResult.Data.UserId);
                        // 判断是否在当前集合中找到，如果找到就修改它
                        if(indexMemoItemViewModel != null)
                        {
                            // 修改
                            indexMemoItemViewModel.Title = updateMemoResult.Data.Title;
                            indexMemoItemViewModel.Content = updateMemoResult.Data.Content;
                            indexMemoItemViewModel.UpdateDate = updateMemoResult.Data.UpdateDate;
                            // 清理IndexMemoItem
                            this.ClearIndexMemoItem();
                            this._logger.LogInformation("[首页（IndexView）] [用户：{Account}（{Id}）] 修改备忘录成功，已更新当前UI集合，ID={Id}，标题=\"{Title}\"", this._userService.UserAccount, this._userService.UserId, updateMemoResult.Data.Id, updateMemoResult.Data.Title);
                            this._messenger.ShowSuccess(SnackbarTarget.MainView, updateMemoResult.Message, "修改备忘录成功");
                        }
                        else
                        {
                            this.ClearIndexMemoItem();
                            this._logger.LogWarning("[首页（IndexView）] [用户：{Account}（{Id}）] 修改备忘录成功，但更新当前UI集合失败，ID={Id}，标题=\"{Title}\"", this._userService.UserAccount, this._userService.UserId, updateMemoResult.Data.Id, updateMemoResult.Data.Title);
                            this._messenger.ShowCaution(SnackbarTarget.MainView, updateMemoResult.Message, "修改备忘录成功，但更新当前集合失败");
                        }
                    }
                    else
                    {
                        this._logger.LogWarning("[首页（IndexView）] [用户：{Account}（{Id}）] 修改备忘录失败，原因：{Reason}", this._userService.UserAccount, this._userService.UserId, updateMemoResult != null ? updateMemoResult.Message : "服务返回结果为空");
                        this._messenger.ShowCaution(SnackbarTarget.MainView, "修改备忘录失败", updateMemoResult != null ? updateMemoResult.Message : "修改备忘录失败");
                    }
                }
                // 如果是点击的暂不修改，就直接关闭对话框，保留已写的数据
                // Warning（保留功能未实现，暂时不考虑保留功能）
                if (updateMemoContentDialogResult == ContentDialogResult.Secondary) { return; }
                // 如果是点击的取消，就关闭对话框并清除已写的数据
                if (updateMemoContentDialogResult == ContentDialogResult.None) { this.ClearIndexMemoItem(); return; }
            }
            catch (Exception ex)
            {
                this._logger.LogError("[首页（IndexView）] [用户：{Account}（{Id}）] 修改备忘录时出现异常。异常信息：{ex}", this._userService.UserAccount, this._userService.UserId, ex);
                this._messenger.ShowDanger(SnackbarTarget.MainView, "修改备忘录失败", "修改备忘录时出现异常");
            }
        }

        // 方法：删除备忘录
        private async Task DeleteMemoItem(IndexMemoItemViewModel? item)
        {
            try
            {
                // 如果传入的 IndexMemoItemViewModel 参数是NULL，就记录日志并直接返回
                if (item == null)
                {
                    this._logger.LogWarning("[首页（IndexView）] [用户：{Account}（{Id}）] 删除备忘录时失败。传入的备忘录数据参数为NULL", this._userService.UserAccount, this._userService.UserId);
                    this._messenger.ShowCaution(SnackbarTarget.MainView, "删除备忘录失败", "传入的备忘录数据参数为空");
                    return;
                }

                // 如果传入的参数不为NULL，就正常进行业务流程
                // 给属性 IndexMemoItem 赋值
                this.LoadIndexMemoItem(item);
                // 打开对话框并保存结果
                ContentDialogResult deleteMemoContentDialogResult = await this._contentDialogService.ShowAsync(new ContentDialog()
                {
                    Title = "删除备忘？",
                    Content = new TextBlock
                    {
                        Text = $"是否删除此备忘录，此备忘录信息如下：{Environment.NewLine}" +
                               $"├─ 标题：{item.Title}{Environment.NewLine}" +
                               $"└─ 内容：{item.Content}",
                        TextWrapping = TextWrapping.Wrap
                    },
                    PrimaryButtonAppearance = ControlAppearance.Danger,
                    PrimaryButtonText = "删除",
                    SecondaryButtonText = "暂不删除",
                    CloseButtonText = "取消",
                    DialogMaxWidth=650
                }, CancellationToken.None);


                // 判断是否删除的备忘录数据
                // 如果是点击的删除,就将此备忘录信息从数据库中删除并在当前的IndexMemoItems集合删除指定项
                if (deleteMemoContentDialogResult == ContentDialogResult.Primary)
                {
                    // 调用后台服务删除备忘录
                    ServiceResult<bool> deleteMemoResult = await this._memoService.DeleteMemoAsync(this._indexMemoItem.ToDeleteMemoDto());
                    // 判断删除备忘录是否成功
                    if (deleteMemoResult != null && deleteMemoResult.IsSuccess && deleteMemoResult.Data == true)
                    {
                        // 如果从数据库中删除成功就从当前集合中清除指定项
                        // 从当前的集合列表中找到需要删除的那个备忘录
                        IndexMemoItemViewModel? indexMemoItemViewModel = this.IndexMemoItems.FirstOrDefault(m => m.Id == item.Id && m.UserId == item.UserId && m.Title == item.Title);
                        // 判断是否在当前集合中找到，如果找到就删除它
                        if (indexMemoItemViewModel != null)
                        {
                            // 删除
                            this.IndexMemoItems.Remove(indexMemoItemViewModel);
                            // 清理IndexMemoItem
                            this.ClearIndexMemoItem();
                            // 重新计算汇总数据
                            this._messenger.Send(new UpdateIndexSummaryMessage() { Type = UpdateIndexSummaryType.DeleteMemo });
                            this._logger.LogInformation("[首页（IndexView）] [用户：{Account}（{Id}）] 删除备忘录成功，已从当前UI集合中清除，ID={Id}，标题=\"{Title}\"", this._userService.UserAccount, this._userService.UserId, item.Id, item.Title);
                            this._messenger.ShowSuccess(SnackbarTarget.MainView, deleteMemoResult.Message, "删除备忘录成功");
                        }
                        else
                        {
                            this.ClearIndexMemoItem();
                            this._logger.LogWarning("[首页（IndexView）] [用户：{Account}（{Id}）] 删除备忘录成功，但当前UI集合中清除失败，ID={Id}，标题=\"{Title}\"", this._userService.UserAccount, this._userService.UserId, item.Id, item.Title);
                            this._messenger.ShowCaution(SnackbarTarget.MainView, deleteMemoResult.Message, "删除备忘录成功，但从当前集合中清除失败");
                        }
                    }
                    else
                    {
                        this._logger.LogWarning("[首页（IndexView）] [用户：{Account}（{Id}）] 删除备忘录失败，原因：{Reason}", this._userService.UserAccount, this._userService.UserId, deleteMemoResult != null ? deleteMemoResult.Message : "服务返回结果为空");
                        this._messenger.ShowCaution(SnackbarTarget.MainView, "删除备忘录失败", deleteMemoResult != null ? deleteMemoResult.Message : "删除备忘录失败");
                    }
                }
                // 如果是点击的暂不删除，就直接关闭对话框，保留已写的数据
                // Warning（保留功能未实现，暂时不考虑保留功能）
                if (deleteMemoContentDialogResult == ContentDialogResult.Secondary) { return; }
                // 如果是点击的取消，就关闭对话框并清除已写的数据
                if (deleteMemoContentDialogResult == ContentDialogResult.None) { this.ClearIndexMemoItem(); return; }
            }
            catch(Exception ex)
            {
                this._logger.LogError("[首页（IndexView）] [用户：{Account}（{Id}）] 删除备忘录时出现异常。异常信息：{ex}", this._userService.UserAccount, this._userService.UserId, ex);
                this._messenger.ShowDanger(SnackbarTarget.MainView, "删除备忘录失败", "删除备忘录时出现异常");
            }
        }


        // 方法：填充修改/删除的备忘录列表项 至 IndexMemoItem
        private void LoadIndexMemoItem(IndexMemoItemViewModel indexMemoItemViewModel)
        {
            this.IndexMemoItem.Id = indexMemoItemViewModel.Id;
            this.IndexMemoItem.UserId = indexMemoItemViewModel.UserId;
            this.IndexMemoItem.Title = indexMemoItemViewModel.Title;
            this.IndexMemoItem.Content = indexMemoItemViewModel.Content;
            this.IndexMemoItem.CreateDate = indexMemoItemViewModel.CreateDate;
            this.IndexMemoItem.UpdateDate = indexMemoItemViewModel.UpdateDate;
        }
        // 方法：清理添加/修改的备忘录列表项IndexMemoItem
        private void ClearIndexMemoItem()
        {
            this.IndexMemoItem.Id = 0;
            this.IndexMemoItem.UserId = 0;
            this.IndexMemoItem.Title = string.Empty;
            this.IndexMemoItem.Content = string.Empty;
            this.IndexMemoItem.CreateDate = DateTime.MinValue;
            this.IndexMemoItem.UpdateDate = DateTime.MinValue;
        }

        // 私有方法：初始化VM
        private async Task Init()
        {
            try
            {
                // 1.从数据库拿到当前用户备忘录集合，并清空当前集合
                this._indexMemoItems.Clear();
                ServiceResult<List<MemoDto>> allMemosResult = await this._memoService.GetAllMemosAsync();
                // 判断是否获取成功并且数据不为空
                if (allMemosResult != null && allMemosResult.IsSuccess)
                {
                    // 成功获取到数据之后，放到集合
                    if (allMemosResult.Data != null)
                    {
                        foreach (MemoDto MemoDto in allMemosResult.Data)
                        {
                            this.IndexMemoItems.Add(MemoDto.ToIndexMemoItemViewModel());
                        }
                        // 打印日志
                        this._logger.LogInformation("[首页（IndexView）] [用户：{Account}（{Id}）] 备忘录数据绑定UI成功。数据条数={Count}", this._userService.UserAccount, this._userService.UserId, allMemosResult.Data.Count);
                    }
                    else
                    {
                        // 打印日志
                        this._logger.LogWarning("[首页（IndexView）] [用户：{Account}（{Id}）] 备忘录数据加载成功。无可用数据", this._userService.UserAccount, this._userService.UserId);
                    }
                }
                else
                {
                    // 打印日志
                    this._logger.LogError("[首页（IndexView）] [用户：{Account}（{Id}）]！备忘录数据加载失败。{Message}", this._userService.UserAccount, this._userService.UserId, allMemosResult == null ? "调用Service失败" : allMemosResult.Message);
                    this._messenger.ShowDanger(SnackbarTarget.MainView, "数据加载失败！", "备忘录数据加载失败");
                }
            }
            catch (Exception ex)
            {
                // 打印日志
                this._logger.LogError("[首页（IndexView）] [用户：{Account}（{Id}）]！备忘录数据加载时出现异常。异常信息：{ex}", this._userService.UserAccount, this._userService.UserId, ex);
                this._messenger.ShowDanger(SnackbarTarget.MainView, "数据加载失败！", "备忘录数据加载失败");
            }
        }
        // 私有方法：清理VM
        private async Task Cleanup()
        {
            int memoItemsCount = this._indexMemoItems.Count;
            // 打印日志
            this._logger.LogInformation("[首页（IndexView）] [用户：{Account}（{Id}）] 备忘录数据清理完成。清理条数={Count}", this._userService.UserAccount, this._userService.UserId, memoItemsCount);
            this.IndexMemoItems.Clear();
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
            await this.Cleanup();
        }
    }
}
