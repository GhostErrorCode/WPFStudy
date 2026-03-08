using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using WpfUiTest.App.ViewModels.Enums;
using WpfUiTest.App.Views.Main;
using WpfUiTest.Core.DTOs;
using WpfUiTest.Core.Services.Interfaces;
using WpfUiTest.Shared.Base;
using WpfUiTest.Shared.Enums;
using WpfUiTest.Shared.Extensions;
using WpfUiTest.Shared.Messages;
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
                new IndexSummaryItemViewModel(){ Title="待办汇总", Content="0", Icon="ClipboardTaskListRtl24", SummaryType=IndexSummaryType.ToDoTotal },
                new IndexSummaryItemViewModel(){ Title="已完成", Content="0", Icon="ClipboardTask24", SummaryType=IndexSummaryType.ToDoCompleted },
                new IndexSummaryItemViewModel(){ Title="完成率", Content="0.00%", Icon="DocumentPercent24", SummaryType=IndexSummaryType.ToDoCompletionRate },
                new IndexSummaryItemViewModel(){ Title="备忘录", Content="0", Icon="ClipboardTextLtr24", SummaryType=IndexSummaryType.MemoTotal },
                new IndexSummaryItemViewModel(){ Title="NULL", Content="0", Icon="Add24", SummaryType=IndexSummaryType.Custom }
            };

            // 初始化命令

        }
        // 初始化首页汇总数据


        // ==================== 方法 ====================
        // 方法：首页添加/修改数据后更新汇总数据
        private void UpdateSummary(Object source, UpdateIndexSummaryMessage message)
        {
            // 如果传入的参数是添加备忘录就将汇总数据加一
            if (message.Type == UpdateIndexSummaryType.AddMemo)
            {
                // 查询汇总数据列表中备忘录总量的列表项
                IndexSummaryItemViewModel? memoTotalItem = this.IndexSummaryItems.FirstOrDefault(t => t.SummaryType == IndexSummaryType.MemoTotal);
                // 如果找到了对应的列表项
                if (memoTotalItem != null)
                {
                    memoTotalItem.Content = $"{int.Parse(memoTotalItem.Content) + 1}";
                    this._logger.LogInformation("[首页（IndexView）] [用户：{Account}（{Id}）] 更新首页汇总数据（备忘录总量）成功。备忘录总量={memoTotal}", this._userService.UserAccount, this._userService.UserId, memoTotalItem.Content);
                }
                else
                {
                    this._logger.LogWarning("[首页（IndexView）] [用户：{Account}（{Id}）] 更新首页汇总数据（备忘录总量）失败。未找到此汇总项或其他原因", this._userService.UserAccount, this._userService.UserId);
                }
            }

            // 如果传入的参数是删除备忘录就将汇总数据备忘录数量减一
            if (message.Type == UpdateIndexSummaryType.DeleteMemo)
            {
                // 查询汇总数据列表中备忘录总量的列表项
                IndexSummaryItemViewModel? memoTotalItem = this.IndexSummaryItems.FirstOrDefault(t => t.SummaryType == IndexSummaryType.MemoTotal);
                // 如果找到了对应的列表项
                if (memoTotalItem != null)
                {
                    memoTotalItem.Content = $"{int.Parse(memoTotalItem.Content) - 1}";
                    this._logger.LogInformation("[首页（IndexView）] [用户：{Account}（{Id}）] 更新首页汇总数据（备忘录总量）成功。备忘录总量={memoTotal}", this._userService.UserAccount, this._userService.UserId, memoTotalItem.Content);
                }
                else
                {
                    this._logger.LogWarning("[首页（IndexView）] [用户：{Account}（{Id}）] 更新首页汇总数据（备忘录总量）失败。未找到此汇总项或其他原因", this._userService.UserAccount, this._userService.UserId);
                }
            }

            // 如果传入的参数是添加待办事项（未完成）就将加一，并计算比例
            if (message.Type == UpdateIndexSummaryType.AddToDo)
            {
                // 查询汇总数据列表中待办事项总量的列表项
                IndexSummaryItemViewModel? toDoTotalItem = this.IndexSummaryItems.FirstOrDefault(t => t.SummaryType == IndexSummaryType.ToDoTotal);
                // 如果找到了对应的列表项
                if (toDoTotalItem != null)
                {
                    toDoTotalItem.Content = $"{int.Parse(toDoTotalItem.Content) + 1}";
                    this.UpdateToDoCompletionRate();
                    this._logger.LogInformation("[首页（IndexView）] [用户：{Account}（{Id}）] 更新首页汇总数据（待办事项总量、待办事项完成比例）成功。待办事项总量={toDoTotal}", this._userService.UserAccount, this._userService.UserId, toDoTotalItem.Content);
                }
                else
                {
                    this._logger.LogWarning("[首页（IndexView）] [用户：{Account}（{Id}）] 更新首页汇总数据（待办事项总量）失败。未找到此汇总项或其他原因", this._userService.UserAccount, this._userService.UserId);
                }
            }

            // 如果传入的参数是添加待办事项（已完成）就将总量和完成量都加一，并计算比例
            if (message.Type == UpdateIndexSummaryType.AddToDoCompleted)
            {
                // 查询汇总数据列表中待办事项总量、待办事项完成量的列表项
                IndexSummaryItemViewModel? toDoTotalItem = this.IndexSummaryItems.FirstOrDefault(t => t.SummaryType == IndexSummaryType.ToDoTotal);
                IndexSummaryItemViewModel? toDoCompletedItem = this.IndexSummaryItems.FirstOrDefault(t => t.SummaryType == IndexSummaryType.ToDoCompleted);

                // 如果找到了对应的列表项
                if (toDoTotalItem != null && toDoCompletedItem != null)
                {
                    toDoTotalItem.Content = $"{int.Parse(toDoTotalItem.Content) + 1}";
                    toDoCompletedItem.Content = $"{int.Parse(toDoCompletedItem.Content) + 1}";
                    this.UpdateToDoCompletionRate();
                    this._logger.LogInformation("[首页（IndexView）] [用户：{Account}（{Id}）] 更新首页汇总数据（待办事项总量、待办事项完成量）成功。待办事项总量={toDoTotal}，待办事项完成量={toDoCompleted}", this._userService.UserAccount, this._userService.UserId, toDoTotalItem.Content, toDoCompletedItem.Content);
                }
                else
                {
                    this._logger.LogWarning("[首页（IndexView）] [用户：{Account}（{Id}）] 更新首页汇总数据（待办事项总量、待办事项完成量）失败。未找到相关汇总项或其他原因", this._userService.UserAccount, this._userService.UserId);
                }
            }

            // 如果传入的参数是修改待办事项（已完成）就将完成量加一，并计算比例
            if (message.Type == UpdateIndexSummaryType.UpdateToDoCompleted)
            {
                // 查询汇总数据列表中待办事项完成量的列表项
                IndexSummaryItemViewModel? toDoCompletedItem = this.IndexSummaryItems.FirstOrDefault(t => t.SummaryType == IndexSummaryType.ToDoCompleted);

                // 如果找到了对应的列表项
                if (toDoCompletedItem != null)
                {
                    toDoCompletedItem.Content = $"{int.Parse(toDoCompletedItem.Content) + 1}";
                    this.UpdateToDoCompletionRate();
                    this._logger.LogInformation("[首页（IndexView）] [用户：{Account}（{Id}）] 更新首页汇总数据（待办事项完成量）成功。待办事项完成量={toDoCompleted}", this._userService.UserAccount, this._userService.UserId, toDoCompletedItem.Content);
                }
                else
                {
                    this._logger.LogWarning("[首页（IndexView）] [用户：{Account}（{Id}）] 更新首页汇总数据（待办事项总量、待办事项完成量）失败。未找到相关汇总项或其他原因", this._userService.UserAccount, this._userService.UserId);
                }
            }

            // 如果传入的参数是删除待办事项（未完成）就减一，并计算比例
            if (message.Type == UpdateIndexSummaryType.DeleteToDo)
            {
                // 查询汇总数据列表中待办事项总量的列表项
                IndexSummaryItemViewModel? toDoTotalItem = this.IndexSummaryItems.FirstOrDefault(t => t.SummaryType == IndexSummaryType.ToDoTotal);
                // 如果找到了对应的列表项
                if (toDoTotalItem != null)
                {
                    toDoTotalItem.Content = $"{int.Parse(toDoTotalItem.Content) - 1}";
                    this.UpdateToDoCompletionRate();
                    this._logger.LogInformation("[首页（IndexView）] [用户：{Account}（{Id}）] 更新首页汇总数据（待办事项总量、待办事项完成比例）成功。待办事项总量={toDoTotal}", this._userService.UserAccount, this._userService.UserId, toDoTotalItem.Content);
                }
                else
                {
                    this._logger.LogWarning("[首页（IndexView）] [用户：{Account}（{Id}）] 更新首页汇总数据（待办事项总量）失败。未找到此汇总项或其他原因", this._userService.UserAccount, this._userService.UserId);
                }
            }
        }

        // 方法：重新计算待办事项完成比例
        private void UpdateToDoCompletionRate()
        {
            // 拿到待办事项的总量
            IndexSummaryItemViewModel? toDoTotalItem = this.IndexSummaryItems.FirstOrDefault(t=>t.SummaryType == IndexSummaryType.ToDoTotal);
            int toDoTotal = toDoTotalItem != null ? int.Parse(toDoTotalItem.Content) : 0;

            // 拿到待办事项的完成量
            IndexSummaryItemViewModel? toDoCompletedItem = this.IndexSummaryItems.FirstOrDefault(t => t.SummaryType == IndexSummaryType.ToDoCompleted);
            int toDoCompleted = toDoCompletedItem != null ? int.Parse(toDoCompletedItem.Content) : 0;

            // 计算待办事项的完成比例
            IndexSummaryItemViewModel? toDoCompletionRateItem = this.IndexSummaryItems.FirstOrDefault(t => t.SummaryType == IndexSummaryType.ToDoCompletionRate);
            if(toDoCompletionRateItem != null)
            {
                // 如果找到了待办事项完成比例项就计算他
                toDoCompletionRateItem.Content = toDoTotal == 0 ? "0.00%" : $"{(double)toDoCompleted / toDoTotal * 100:F2}%";
                this._logger.LogInformation("[首页（IndexView）] [用户：{Account}（{Id}）] 更新首页汇总数据（待办事项完成比例）成功。完成比例={TodoCompletionRate}", this._userService.UserAccount, this._userService.UserId, toDoCompletionRateItem.Content);
            }
            else
            {
                toDoCompletionRateItem?.Content = "0.00%" ;
                this._logger.LogWarning("[首页（IndexView）] [用户：{Account}（{Id}）] 更新首页汇总数据（待办事项完成比例）失败。未找到此汇总项或其他原因", this._userService.UserAccount, this._userService.UserId);
            }
        }


        // 私有方法：初始化首页汇总数据
        private async Task Init()
        {
            try
            {
                // ========== 订阅首页汇总数据更新事件 ==========
                // 订阅更新汇总数据事件
                this._messenger.Register<UpdateIndexSummaryMessage>(this, UpdateSummary);
                this._logger.LogInformation("[首页（IndexView）] [用户：{Account}（{Id}）] 已订阅更新首页汇总数据事件", this._userService.UserAccount, this._userService.UserId);
            }
            catch (Exception ex)
            {
                this._logger.LogError("[首页（IndexView）] [用户：{Account}（{Id}）] 订阅更新首页汇总数据事件时出现异常。异常信息：{ex}", this._userService.UserAccount, this._userService.UserId, ex);
            }

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
                            IndexSummaryType.ToDoCompleted => summaryResult.Data.TodoCompleted.ToString(), // 已完成数
                            IndexSummaryType.ToDoCompletionRate => summaryResult.Data.TodoCompletionRate, // 完成率（直接用Dto里的%格式）
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
                    this._logger.LogWarning("[首页（IndexView）] [用户：{Account}（{Id}）] 汇总数据加载失败。{Message}", this._userService.UserAccount, this._userService.UserId, summaryResult == null ? "调用Service失败" : summaryResult.Message);
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
                    IndexSummaryType.ToDoTotal => "0", // 待办总数转字符串
                    IndexSummaryType.ToDoCompleted => "0", // 已完成数
                    IndexSummaryType.ToDoCompletionRate => "0.00%", // 完成率（直接用Dto里的%格式）
                    IndexSummaryType.MemoTotal => "0", // 备忘录总数
                    IndexSummaryType.Custom => "0", // 自定义数值
                    _ => "错误" // 兜底，避免漏枚举
                };
            }
            // 打印日志
            this._logger.LogInformation("[首页（IndexView）] [用户：{Account}（{Id}）] 汇总数据数据清理完成。清理条数={Count}", this._userService.UserAccount, this._userService.UserId, this._indexSummaryItems.Count);

            // 取消订阅，防止内存泄露
            this._messenger.Unregister<UpdateIndexSummaryMessage>(this);
            this._logger.LogInformation("[首页（IndexView）] [用户：{Account}（{Id}）] 已取消对更新首页汇总数据事件的订阅", this._userService.UserAccount, this._userService.UserId);
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
