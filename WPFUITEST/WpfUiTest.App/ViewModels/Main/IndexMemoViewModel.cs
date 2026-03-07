using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
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
                        this._logger.LogWarning("[首页（IndexView）] [用户：{Account}（{Id}）] 添加备忘录失败，异常信息：{ex}", this._userService.UserAccount, this._userService.UserId, addMemoResult != null ? addMemoResult.Message : "服务返回结果为空");
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
