using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using WpfUiTest.App.ViewModels.Mapping;
using WpfUiTest.Core.DTOs.Memo;
using WpfUiTest.Core.Services.Interfaces;
using WpfUiTest.Shared.Base;
using WpfUiTest.Shared.Enums;
using WpfUiTest.Shared.Extensions;
using WpfUiTest.Shared.Utilities;

namespace WpfUiTest.App.ViewModels.Main
{
    // 首页备忘录ViewModel
    public class IndexMemoViewModel : BaseViewModel
    {
        // ==================== 字段、属性、命令 ====================
        // 字段：备忘录服务
        private readonly IMemoService _MemoService;
        // 字段：IMessenger服务
        private readonly IMessenger _messenger;
        // 字段：ILogger服务
        private readonly ILogger<IndexMemoViewModel> _logger;
        // 字段：IUserServcei服务
        private readonly IUserService _userService;

        // 属性：首页的备忘录列表
        private ObservableCollection<IndexMemoItemViewModel> _indexMemoItems;
        public ObservableCollection<IndexMemoItemViewModel> IndexMemoItems
        {
            get { return _indexMemoItems; }
            set { SetProperty(ref _indexMemoItems, value); }
        }

        // ==================== 构造函数 ====================
        public IndexMemoViewModel(IMemoService MemoService, IMessenger messenger, ILogger<IndexMemoViewModel> logger, IUserService userService)
        {
            // 初始化字段
            this._indexMemoItems = new ObservableCollection<IndexMemoItemViewModel>();

            this._MemoService = MemoService;
            this._messenger = messenger;
            this._logger = logger;
            this._userService = userService;

            // 初始化属性

            // 初始化命令
        }

        // ==================== 方法 ====================
        // 私有方法：初始化VM
        private async Task Init()
        {
            try
            {
                // 1.从数据库拿到当前用户备忘录集合，并清空当前集合
                this._indexMemoItems.Clear();
                ServiceResult<List<MemoDto>> allMemosResult = await this._MemoService.GetAllMemosAsync();
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
