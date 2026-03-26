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
using WpfUiTest.Core.DTOs.Memo;
using WpfUiTest.Core.DTOs.ToDo;
using WpfUiTest.Core.Services.Interfaces;
using WpfUiTest.Modules.Memo.Mappers;
using WpfUiTest.Shared.Base;
using WpfUiTest.Shared.Enums;
using WpfUiTest.Shared.Extensions;
using WpfUiTest.Shared.Messages;
using WpfUiTest.Shared.Utilities;

namespace WpfUiTest.Modules.Memo.ViewModels
{
    // 备忘录主VM（因此处不像首页复杂，故不再将CRUD操作拆分成多个子VM）
    public class MemoViewModel : BaseViewModel
    {
        // ==================== 字段、属性和命令 ====================
        // 字段：备忘录服务
        private readonly IMemoService _memoService;
        // 字段：IMessenger服务
        private readonly IMessenger _messenger;
        // 字段：ILogger服务
        private readonly ILogger<MemoViewModel> _logger;
        // 字段：IUserServcei服务
        private readonly IUserService _userService;
        // 字段：ContentDialog服务
        private readonly IContentDialogService _contentDialogService;

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

        // 属性：新增备忘录抽屉是否显示
        private bool _isAddDrawerOpen = false;
        public bool IsAddDrawerOpen
        {
            get { return _isAddDrawerOpen; }
            set { SetProperty(ref _isAddDrawerOpen, value); }
        }
        // 属性：修改备忘录抽屉是否显示
        private bool _isEditDrawerOpen = false;
        public bool IsEditDrawerOpen
        {
            get { return _isEditDrawerOpen; }
            set { SetProperty(ref _isEditDrawerOpen, value); }
        }
        // 属性：删除备忘录抽屉是否显示
        private bool isDeleteDrawerOpen = false;
        public bool IsDeleteDrawerOpen
        {
            get { return isDeleteDrawerOpen; }
            set { SetProperty(ref isDeleteDrawerOpen, value); }
        }

        // 属性：用于显示备忘录数据集合
        private ObservableCollection<MemoItemViewModel> _memoItems = new ObservableCollection<MemoItemViewModel>();
        public ObservableCollection<MemoItemViewModel> MemoItems
        {
            get { return _memoItems; }
            set { SetProperty(ref _memoItems, value); }
        }
        // 属性：用于添加、修改或删除的数据项
        private MemoItemViewModel _memoItem = new MemoItemViewModel();
        public MemoItemViewModel MemoItem
        {
            get { return _memoItem; }
            set { SetProperty(ref _memoItem, value); }
        }

        // 命令：打开新增备忘录抽屉
        public RelayCommand OpenAddDrawerCommand { get; private set; }
        // 命令：关闭新增备忘录抽屉
        public RelayCommand CloseAddDrawerCommand { get; private set; }
        // 命令：打开修改备忘录抽屉
        public RelayCommand<MemoItemViewModel> OpenEditDrawerCommand { get; private set; }
        // 命令：关闭修改备忘录抽屉
        public RelayCommand CloseEditDrawerCommand { get; private set; }
        // 命令：打开删除备忘录抽屉
        public RelayCommand<MemoItemViewModel> OpenDeleteDrawerCommand { get; private set; }
        // 命令：关闭删除备忘录抽屉
        public RelayCommand CloseDeleteDrawerCommand { get; private set; }

        // 命令：搜索Command
        public AsyncRelayCommand SearchCommand { get; private set; }
        // 命令：搜索重置Command
        public AsyncRelayCommand SearchResetCommand { get; private set; }
        // 命令：添加备忘录Command
        public AsyncRelayCommand AddMemoItemCommand { get; private set; }
        // 命令：修改备忘录Command
        public AsyncRelayCommand UpdateMemoItemCommand { get; private set; }
        // 命令：删除备忘录Command
        public AsyncRelayCommand DeleteMemoItemCommand { get; private set; }


        // ==================== 构造函数 ====================
        public MemoViewModel(IMemoService memoService, IMessenger messenger, ILogger<MemoViewModel> logger, IUserService userService, IContentDialogService contentDialogService)
        {
            // 初始化字段
            this._memoService = memoService;
            this._messenger = messenger;
            this._logger = logger;
            this._userService = userService;
            this._contentDialogService = contentDialogService;

            // 初始化属性

            // 初始化命令
            this.SearchCommand = new AsyncRelayCommand(LoadPagedMemosAsync);
            this.SearchResetCommand = new AsyncRelayCommand(async () =>
            {
                this.SearchTitle = string.Empty;
                this.SearchContent = string.Empty;
                await LoadPagedMemosAsync();
            });
            // 抽屉相关命令（使用 null 包容运算符 !，这样可以消除警告）
            this.OpenAddDrawerCommand = new RelayCommand(() => { this.ClearMemoItem(); this.IsAddDrawerOpen = true; });
            this.CloseAddDrawerCommand = new RelayCommand(() => { this.IsAddDrawerOpen = false; });
            this.OpenEditDrawerCommand = new RelayCommand<MemoItemViewModel>((memoItem) => { this.LoadMemoItem(memoItem!); this.IsEditDrawerOpen = true; });
            this.CloseEditDrawerCommand = new RelayCommand(() => { this.IsEditDrawerOpen = false; });
            this.OpenDeleteDrawerCommand = new RelayCommand<MemoItemViewModel>((memoItem) => { this.LoadMemoItem(memoItem!); this.IsDeleteDrawerOpen = true; });
            this.CloseDeleteDrawerCommand = new RelayCommand(() => { this.IsDeleteDrawerOpen = false; });
            // CRUD命令
            this.AddMemoItemCommand = new AsyncRelayCommand(AddMemoItemAsync);
            this.UpdateMemoItemCommand = new AsyncRelayCommand(UpdateMemoItemAsync);
            this.DeleteMemoItemCommand = new AsyncRelayCommand(DeleteMemoItemAsync);
        }



        // ==================== 方法 ====================
        // 方法：分页多条件查询
        private async Task LoadPagedMemosAsync()
        {
            try
            {
                // 显示加载动画
                this.IsLoading = true;
                this.LoadingText = "分页查询中...";

                //  清空当前集合
                this.MemoItems.Clear();

                // 从数据库中根据条件并分页拿取数据集合
                ServiceResult<PagedResult<MemoDto>> result = await this._memoService.GetPagedMemosAsync(new PagedQueryMemoDto()
                {
                    PageIndex = this._pageIndex,
                    PageSize = this._pageSize,
                    Title = this._searchTitle,
                    Content = this._searchContent
                });
                // 判断返回的结果是否成功
                if (result != null && result.IsSuccess)
                {
                    // 判断返回的结果是否存在数据Data，如果存在，则开始将查到的数据添加至数据集合中
                    if (result.Data != null)
                    {
                        // 1.给数据总条数赋值
                        this.TotalCount = result.Data.TotalCount;
                        // 2.给数据集合赋值
                        foreach (MemoDto memoDto in result.Data.Items)
                        {
                            this.MemoItems.Add(memoDto.ToMemoItemViewModel());
                        }
                        this._logger.LogInformation("[MemoView] [用户：{Account}（{Id}）] 分页查询备忘录数据绑定UI成功，页码={PageIndex}，页大小={PageSize}。数据条数={Count}", this._userService.UserAccount, this._userService.UserId, this._pageIndex, this._pageSize, result.Data.Items.Count);
                    }
                    else
                    {
                        // 打印日志
                        this._logger.LogWarning("[MemoView] [用户：{Account}（{Id}）] 分页查询备忘录数据加载成功。无可用数据", this._userService.UserAccount, this._userService.UserId);
                    }
                }
                else
                {
                    // 打印日志
                    this._logger.LogError("[MemoView] [用户：{Account}（{Id}）]！分页查询备忘录数据失败。{Message}", this._userService.UserAccount, this._userService.UserId, result == null ? "调用Service失败" : result.Message);
                    this._messenger.ShowDanger(SnackbarTarget.MainView, "数据加载失败！", "分页查询备忘录数据失败");
                }
            }
            catch (Exception ex)
            {
                // 打印日志
                this._logger.LogError("[MemoView] [用户：{Account}（{Id}）] 分页查询备忘录数据时出现异常，页码={PageIndex}，页大小={PageSize}。异常信息：{ex}", this._userService.UserAccount, this._userService.UserId, this._pageIndex, this._pageSize, ex);
                this._messenger.ShowDanger(SnackbarTarget.MainView, "数据加载失败", "分页查询备忘录数据失败");
            }
            finally
            {
                // 隐藏加载动画
                this.IsLoading = false;
            }
        }

        // 方法：添加备忘录
        private async Task AddMemoItemAsync()
        {
            try
            {
                // 二次确认窗
                ContentDialogResult contentDialogResult = await this._contentDialogService.ShowSimpleDialogAsync(new SimpleContentDialogCreateOptions()
                {
                    Title = "确认添加？",
                    Content = new TextBlock
                    {
                        Text = $"备忘录信息如下：{Environment.NewLine}" +
                               $"----- 标题 -----{Environment.NewLine}{this._memoItem.Title}{Environment.NewLine}" +
                               $"----- 内容 -----{Environment.NewLine}{this._memoItem.Content}",
                        TextWrapping = TextWrapping.Wrap
                    },
                    PrimaryButtonText = "确认添加！提交至数据库",
                    CloseButtonText = "先不提交，我再看看"
                });
                // 如果点击的是CloseButton，直接return终止当前方法，否则继续执行
                if (contentDialogResult == ContentDialogResult.None) return;

                // 调用后台服务添加备忘录
                ServiceResult<MemoDto> addMemoResult = await this._memoService.AddMemoAsync(this._memoItem.ToAddMemoDto());
                // 判断添加备忘录是否成功
                if (addMemoResult != null && addMemoResult.IsSuccess && addMemoResult.Data != null)
                {
                    this._logger.LogInformation("[MemoView] [用户：{Account}（{Id}）] 添加备忘录成功，已添加至当前UI集合，ID={Id}，标题=\"{Title}\"", this._userService.UserAccount, this._userService.UserId, addMemoResult.Data.Id, addMemoResult.Data.Title);
                    this._messenger.ShowSuccess(SnackbarTarget.MainView, addMemoResult.Message, "添加备忘录成功");
                    // 跳转到第一页，因为默认按时间降序，所以新数据会出现在第一页
                    this.PageIndex = 1;
                    // 随后查询查询数据库，确保数据一致性
                    await this.LoadPagedMemosAsync();
                    // 关闭抽屉
                    this.IsAddDrawerOpen = false;
                    // 清除 ToDoItem
                    this.ClearMemoItem();
                }
                else
                {
                    this._logger.LogWarning("[MemoView] [用户：{Account}（{Id}）] 添加备忘录失败，原因：{Reason}", this._userService.UserAccount, this._userService.UserId, addMemoResult != null ? addMemoResult.Message : "服务返回结果为空");
                    this._messenger.ShowCaution(SnackbarTarget.MainView, "添加备忘录失败", addMemoResult != null ? addMemoResult.Message : "添加备忘录失败");
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError("[MemoView] [用户：{Account}（{Id}）] 添加备忘录时出现异常。异常信息：{ex}", this._userService.UserAccount, this._userService.UserId, ex);
                this._messenger.ShowDanger(SnackbarTarget.MainView, "添加备忘录失败", "添加备忘录时出现异常");
            }
        }

        // 方法：修改备忘录
        private async Task UpdateMemoItemAsync()
        {
            try
            {
                // 二次确认窗
                ContentDialogResult contentDialogResult = await this._contentDialogService.ShowSimpleDialogAsync(new SimpleContentDialogCreateOptions()
                {
                    Title = "确认修改？",
                    Content = new TextBlock
                    {
                        Text = $"修改后备忘录信息如下：{Environment.NewLine}" +
                               $"----- ID -----{Environment.NewLine}{this._memoItem.Id}{Environment.NewLine}{Environment.NewLine}" +
                               $"----- 标题 -----{Environment.NewLine}{this._memoItem.Title}{Environment.NewLine}{Environment.NewLine}" +
                               $"----- 内容 -----{Environment.NewLine}{this._memoItem.Content}{Environment.NewLine}{Environment.NewLine}" +
                               $"----- 创建日期 -----{Environment.NewLine}{this._memoItem.CreateDate}{Environment.NewLine}{Environment.NewLine}" +
                               $"----- 修改日期 -----{Environment.NewLine}{this._memoItem.UpdateDate}",
                        TextWrapping = TextWrapping.Wrap
                    },
                    PrimaryButtonText = "确认修改！保存到数据库中",
                    CloseButtonText = "先不保存，我再看看"
                });
                // 如果点击的是CloseButton，直接return终止当前方法，否则继续执行
                if (contentDialogResult == ContentDialogResult.None) return;

                // 调用后台服务修改备忘录
                ServiceResult<MemoDto> updateMemoResult = await this._memoService.UpdateMemoAsync(this._memoItem.ToUpdateMemoDto());
                // 判断修改备忘录是否成功
                if (updateMemoResult != null && updateMemoResult.IsSuccess && updateMemoResult.Data != null)
                {
                    this._logger.LogInformation("[MemoView] [用户：{Account}（{Id}）] 修改备忘录成功，已更新当前UI集合，ID={Id}，标题=\"{Title}\"", this._userService.UserAccount, this._userService.UserId, updateMemoResult.Data.Id, updateMemoResult.Data.Title);
                    this._messenger.ShowSuccess(SnackbarTarget.MainView, updateMemoResult.Message, "修改备忘录成功");
                    // 重新查询数据库，确保数据一致性
                    await this.LoadPagedMemosAsync();
                    // 关闭抽屉
                    this.IsEditDrawerOpen = false;
                    // 清除 ToDoItem
                    this.ClearMemoItem();
                }
                else
                {
                    this._logger.LogWarning("[MemoView] [用户：{Account}（{Id}）] 修改备忘录失败，原因：{Reason}", this._userService.UserAccount, this._userService.UserId, updateMemoResult != null ? updateMemoResult.Message : "服务返回结果为空");
                    this._messenger.ShowCaution(SnackbarTarget.MainView, "修改备忘录失败", updateMemoResult != null ? updateMemoResult.Message : "修改备忘录失败");
                }

            }
            catch (Exception ex)
            {
                this._logger.LogError("[MemoView] [用户：{Account}（{Id}）] 修改备忘录时出现异常。异常信息：{ex}", this._userService.UserAccount, this._userService.UserId, ex);
                this._messenger.ShowDanger(SnackbarTarget.MainView, "修改备忘录失败", "修改备忘录时出现异常");
            }
        }

        // 方法：删除备忘录
        private async Task DeleteMemoItemAsync()
        {
            try
            {
                // 二次确认窗
                ContentDialogResult contentDialogResult = await this._contentDialogService.ShowAsync(new ContentDialog()
                {
                    Title = "确认删除？",
                    Content = new TextBlock
                    {
                        Text = $"需要删除的备忘录信息如下：{Environment.NewLine}" +
                               $"----- ID -----{Environment.NewLine}{this._memoItem.Id}{Environment.NewLine}{Environment.NewLine}" +
                               $"----- 标题 -----{Environment.NewLine}{this._memoItem.Title}{Environment.NewLine}{Environment.NewLine}" +
                               $"----- 内容 -----{Environment.NewLine}{this._memoItem.Content}{Environment.NewLine}{Environment.NewLine}" +
                               $"----- 创建日期 -----{Environment.NewLine}{this._memoItem.CreateDate}{Environment.NewLine}{Environment.NewLine}" +
                               $"----- 修改日期 -----{Environment.NewLine}{this._memoItem.UpdateDate}{Environment.NewLine}{Environment.NewLine}{Environment.NewLine}{Environment.NewLine}" + 
                               $"你真的会失去它，永远",
                        TextWrapping = TextWrapping.Wrap
                    },
                    PrimaryButtonAppearance = ControlAppearance.Danger,
                    PrimaryButtonText = "确认删除！离开我的数据库",
                    CloseButtonText = "先不删除，我再看看"
                }, CancellationToken.None);
                // 如果点击的是CloseButton，直接return终止当前方法，否则继续执行
                if (contentDialogResult == ContentDialogResult.None) return;

                // 调用后台服务删除备忘录
                ServiceResult<bool> deleteMemoResult = await this._memoService.DeleteMemoAsync(this._memoItem.ToDeleteMemoDto());
                // 判断删除备忘录是否成功
                if (deleteMemoResult != null && deleteMemoResult.IsSuccess && deleteMemoResult.Data == true)
                {
                    this._logger.LogInformation("[MemoView] [用户：{Account}（{Id}）] 删除备忘录成功，已从当前UI集合中清除，ID={Id}，标题=\"{Title}\"", this._userService.UserAccount, this._userService.UserId, this._memoItem.Id, this._memoItem.Title);
                    this._messenger.ShowSuccess(SnackbarTarget.MainView, deleteMemoResult.Message, "删除备忘录成功");
                    // 重新查询数据库，确保数据一致性
                    await this.LoadPagedMemosAsync();
                    // 关闭抽屉
                    this.IsDeleteDrawerOpen = false;
                    // 清除 ToDoItem
                    this.ClearMemoItem();
                }
                else
                {
                    this._logger.LogWarning("[MemoView] [用户：{Account}（{Id}）] 删除备忘录失败，原因：{Reason}", this._userService.UserAccount, this._userService.UserId, deleteMemoResult != null ? deleteMemoResult.Message : "服务返回结果为空");
                    this._messenger.ShowCaution(SnackbarTarget.MainView, "删除备忘录失败", deleteMemoResult != null ? deleteMemoResult.Message : "删除备忘录失败");
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError("[MemoView] [用户：{Account}（{Id}）] 删除备忘录时出现异常。异常信息：{ex}", this._userService.UserAccount, this._userService.UserId, ex);
                this._messenger.ShowDanger(SnackbarTarget.MainView, "删除备忘录失败", "删除备忘录时出现异常");
            }
        }

        // 方法：装载需要修改/删除的备忘录数据至MemoItem中
        private void LoadMemoItem(MemoItemViewModel memoItemViewModel)
        {
            this.MemoItem.Id = memoItemViewModel.Id;
            this.MemoItem.UserId = memoItemViewModel.UserId;
            this.MemoItem.Title = memoItemViewModel.Title;
            this.MemoItem.Content = memoItemViewModel.Content;
            this.MemoItem.CreateDate = memoItemViewModel.CreateDate;
            this.MemoItem.UpdateDate = memoItemViewModel.UpdateDate;
        }
        // 方法：清理MemoItem，恢复至默认值，以便添加新的备忘录
        private void ClearMemoItem()
        {
            this.MemoItem.Id = 0;
            this.MemoItem.UserId = 0;
            this.MemoItem.Title = string.Empty;
            this.MemoItem.Content = string.Empty;
            this.MemoItem.CreateDate = DateTime.MinValue;
            this.MemoItem.UpdateDate = DateTime.MinValue;
        }

        // 私有方法：初始化VM
        private async Task InitAsync()
        {
            // 调用分页多条件查询，直接完成页面进入时的数据加载
            await this.LoadPagedMemosAsync();
        }
        // 私有方法：清理VM
        private async Task CleanupASync()
        {
            // 关闭所有抽屉
            this.IsAddDrawerOpen = false;
            this.IsEditDrawerOpen = false;
            this.IsDeleteDrawerOpen = false;
            this._logger.LogInformation("[MemoView] [用户：{Account}（{Id}）] 已重置所有抽屉", this._userService.UserAccount, this._userService.UserId);


            // 清空当前编辑项
            this.ClearMemoItem();  // 假设有方法重置 ToDoItem
            this._logger.LogInformation("[MemoView] [用户：{Account}（{Id}）] 已清空临时编辑项", this._userService.UserAccount, this._userService.UserId);


            // 重置加载状态
            this.IsLoading = false;
            this.LoadingText = "加载中...";
            this._logger.LogInformation("[MemoView] [用户：{Account}（{Id}）] 已重置加载状态", this._userService.UserAccount, this._userService.UserId);

        }

        // 方法：导航进入后执行
        public override async Task OnNavigatedToAsync()
        {
            this._logger.LogInformation("[MemoView] [用户：{Account}（{Id}）] 导航进入，开始初始化...", this._userService.UserAccount, this._userService.UserId);
            // 执行初始化方法
            await this.InitAsync();
        }
        // 方法：导航离开后执行
        public override async Task OnNavigatedFromAsync()
        {
            this._logger.LogInformation("[MemoView] [用户：{Account}（{Id}）] 导航离开，清理缓存...", this._userService.UserAccount, this._userService.UserId);
            // 执行清理方法
            await this.CleanupASync();
        }
    }
}
