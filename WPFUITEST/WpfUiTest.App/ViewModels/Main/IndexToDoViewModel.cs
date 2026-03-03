using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;
using System.Text;
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

        // 属性：首页未完成的待办事项列表
        private ObservableCollection<IndexToDoItemViewModel> _indexToDoItems;
        public ObservableCollection<IndexToDoItemViewModel> IndexToDoItems
        {
            get { return _indexToDoItems; }
            set { SetProperty(ref _indexToDoItems, value); }
        }

        // ==================== 构造函数 ====================
        public IndexToDoViewModel(IToDoService toDoService, IMessenger messenger, ILogger<IndexToDoViewModel> logger)
        {
            // 初始化字段
            this._indexToDoItems = new ObservableCollection<IndexToDoItemViewModel>();

            this._toDoService = toDoService;
            this._messenger = messenger;
            this._logger = logger;

            // 初始化属性

            // 初始化命令
        }

        // ==================== 方法 ====================
        // 私有方法：初始化VM
        private async Task Init()
        {
            // 1.从数据库拿到当前用户未完成的待办事项集合，并清空当前集合
            this.IndexToDoItems.Clear();
            ServiceResult<List<ToDoDto>> allPendingToDosResult = await this._toDoService.GetAllPendingToDosAsync();
            // 判断是否获取成功并且数据不为空
            if(allPendingToDosResult != null && allPendingToDosResult.IsSuccess)
            {
                // 成功获取到数据之后，放到集合
                if(allPendingToDosResult.Data != null)
                {
                    foreach (ToDoDto toDoDto in allPendingToDosResult.Data)
                    {
                        this.IndexToDoItems.Add(toDoDto.ToIndexToDoItemViewModel());
                    }
                    // 打印日志
                    this._logger.LogInformation("首页获取当前用户未完成待办事项成功！共{num}条", allPendingToDosResult.Data.Count);
                }
                else
                {
                    // 打印日志
                    this._logger.LogInformation("首页获取当前用户未完成待办事项成功！ 但没有数据！ {@RegisterResult}", allPendingToDosResult);
                }
            }
            else
            {
                // 打印日志
                this._logger.LogError("首页获取当前用户未完成待办事项失败！{@RegisterResult}", allPendingToDosResult);
                this._messenger.ShowDanger(SnackbarTarget.MainView, "获取数据失败！", "首页获取当前用户未完成待办事项失败");
            }
        }
        // 私有方法：清理VM
        private async Task Cleanup()
        {
            // 打印日志
            this._logger.LogInformation("[首页] 未完成待办事项数据已清理！");
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
