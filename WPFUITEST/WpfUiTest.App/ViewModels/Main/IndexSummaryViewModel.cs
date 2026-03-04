using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using WpfUiTest.App.ViewModels.Enums;
using WpfUiTest.Core.DTOs;
using WpfUiTest.Core.Services.Interfaces;
using WpfUiTest.Shared.Base;
using WpfUiTest.Shared.Enums;
using WpfUiTest.Shared.Extensions;
using WpfUiTest.Shared.Utilities;

namespace WpfUiTest.App.ViewModels.Main
{
    // 首页汇总ViewModel
    public class IndexSummaryViewModel : BaseViewModel
    {
        // ==================== 字段、属性、命令 ====================
        // 字段：首页汇总服务
        private readonly ISummaryService _summaryService;
        // 字段：IMessenger服务
        private readonly IMessenger _messenger;
        // 字段：ILogger服务
        private readonly ILogger<IndexSummaryViewModel> _logger;
        // 字段：IUserService服务
        private readonly IUserService _userService;

        // 属性：首页汇总
        private ObservableCollection<IndexSummaryItemViewModel> _indexSummaryItems;
        public ObservableCollection<IndexSummaryItemViewModel> IndexSummaryItems
        {
            get { return _indexSummaryItems; }
            set { SetProperty(ref _indexSummaryItems, value); }
        }

        // ==================== 构造函数 ====================
        public IndexSummaryViewModel(ISummaryService summaryService, IMessenger messenger, ILogger<IndexSummaryViewModel> logger, IUserService userService)
        {
            // 初始化字段
            this._summaryService = summaryService;
            this._messenger = messenger;
            this._logger = logger;
            this._userService = userService;

            // 初始化属性
            this._indexSummaryItems = new ObservableCollection<IndexSummaryItemViewModel>()
            {
                new IndexSummaryItemViewModel(){ Title="待办汇总", Content="999999", Icon="ClipboardTaskListRtl24", SummaryType=IndexSummaryType.ToDoTotal },
                new IndexSummaryItemViewModel(){ Title="已完成", Content="999999", Icon="ClipboardTask24", SummaryType=IndexSummaryType.Completed },
                new IndexSummaryItemViewModel(){ Title="完成率", Content="100%", Icon="DocumentPercent24", SummaryType=IndexSummaryType.CompletionRate },
                new IndexSummaryItemViewModel(){ Title="备忘录", Content="999999", Icon="ClipboardTextLtr24", SummaryType=IndexSummaryType.MemoTotal },
                new IndexSummaryItemViewModel(){ Title="NULL", Content="NULL", Icon="Add24", SummaryType=IndexSummaryType.Custom }
            };

            // 初始化命令
        }
        // 初始化首页汇总数据


        // ==================== 方法 ====================
        // 私有方法：初始化首页汇总数据
        private async Task Init()
        {
            try
            {
                // ========== 初始化汇总数据 ==========
                // 每次导航进入都获取一次汇总数据
                ServiceResult<SummaryDto> summaryResult = await this._summaryService.GetSummaryAsync();
                if (summaryResult.IsSuccess && summaryResult.Data != null)
                {
                    foreach(IndexSummaryItemViewModel item in this.IndexSummaryItems)
                    {
                        item.Content = item.SummaryType switch
                        {
                            IndexSummaryType.ToDoTotal => summaryResult.Data.ToDoTotal.ToString(), // 待办总数转字符串
                            IndexSummaryType.Completed => summaryResult.Data.TodoCompleted.ToString(), // 已完成数
                            IndexSummaryType.CompletionRate => summaryResult.Data.TodoCompletionRate, // 完成率（直接用Dto里的%格式）
                            IndexSummaryType.MemoTotal => summaryResult.Data.MemoTotal.ToString(), // 备忘录总数
                            IndexSummaryType.Custom => summaryResult.Data.Custom.ToString(), // 自定义数值
                            _ => "错误" // 兜底，避免漏枚举
                        };
                    }
                    // 输出日志
                    this._logger.LogInformation("[首页（IndexView）] [用户：{Account}（{Id}）] 汇总数据绑定UI成功。数据：待办总量={ToDoTotal}，待办完成量={TodoCompleted}，待办完成比例={TodoCompletionRate}，备忘总量={MemoTotal}，自定义={Custom}", this._userService.UserAccount, this._userService.UserId, summaryResult.Data.ToDoTotal, summaryResult.Data.TodoCompleted, summaryResult.Data.TodoCompletionRate, summaryResult.Data.MemoTotal, summaryResult.Data.Custom);
                }
                else
                {
                    foreach (IndexSummaryItemViewModel item in this.IndexSummaryItems) { item.Content = "获取失败"; }
                    // 输出日志
                    this._logger.LogInformation("[首页（IndexView）] [用户：{Account}（{Id}）] 汇总数据加载失败。{Message}", this._userService.UserAccount, this._userService.UserId, summaryResult == null ? "调用Service失败" : summaryResult.Message);
                }
            }
            catch (Exception ex)
            {
                // 输出日志
                this._logger.LogError("[首页（IndexView）] [用户：{Account}（{Id}）]！汇总数据加载时出现异常。异常信息：{ex}", this._userService.UserAccount, this._userService.UserId, ex);
                this._messenger.ShowDanger(SnackbarTarget.MainView, "数据加载失败！", "汇总数据加载失败");
            }
        }
        // 私有方法：清理VM
        private async Task Cleanup()
        {
            foreach (IndexSummaryItemViewModel item in this.IndexSummaryItems)
            {
                item.Content = item.SummaryType switch
                {
                    IndexSummaryType.ToDoTotal => "999999", // 待办总数转字符串
                    IndexSummaryType.Completed => "999999", // 已完成数
                    IndexSummaryType.CompletionRate => "100.00%", // 完成率（直接用Dto里的%格式）
                    IndexSummaryType.MemoTotal => "999999", // 备忘录总数
                    IndexSummaryType.Custom => "0", // 自定义数值
                    _ => "错误" // 兜底，避免漏枚举
                };
            }
            // 打印日志
            this._logger.LogInformation("[首页（IndexView）] [用户：{Account}（{Id}）] 汇总数据数据清理完成。清理条数={Count}", this._userService.UserAccount, this._userService.UserId, this._indexSummaryItems.Count);
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
