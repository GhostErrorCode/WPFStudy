using MemoDesktop.Events;
using MemoDesktop.Extensions;
using MemoDesktop.Models;
using MemoDesktop.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MemoDesktop.ViewModels.Dashboard
{
    // 只处理任务栏卡片的ViewModel
    public class TaskBarViewModel : BindableBase
    {
        // =================== Defined Field Start ===================================================================
        // Prism中的对话服务 IDialogService
        private readonly IDialogHostService _dialogHostService;
        // Prism中的事件处理器
        private readonly IEventAggregator _eventAggregator;
        // 备忘录服务层
        private readonly IMemoApiService _memoApiService;
        // 待办事项服务层
        private readonly IToDoApiService _toDoApiService;
        // =================== Defined Field End ===================================================================

        private ObservableCollection<TaskbarItemViewModel> _taskBars = new ObservableCollection<TaskbarItemViewModel>();
        public ObservableCollection<TaskbarItemViewModel> TaskBars
        {
            get { return _taskBars; }
            set { _taskBars = value; RaisePropertyChanged(); }
        }


        public TaskBarViewModel(IEventAggregator eventAggregator)
        {
            this._eventAggregator = eventAggregator;

            // 清除任务栏
            this.TaskBars.Clear();

            // 初始化任务栏
            /*
            this.TaskBars.Add(new TaskBarModel() { Icon = "ClockFast", Title = "汇总", Color = "#3F51B5", Target = "" });
            this.TaskBars.Add(new TaskBarModel() { Icon = "ClockCheckOutline", Title = "已完成", Color = "#4CAF50", Target = "" });
            this.TaskBars.Add(new TaskBarModel() { Icon = "ChartLineVariant", Title = "完成率", Color = "#FF9800", Target = "" });
            this.TaskBars.Add(new TaskBarModel() { Icon = "PlaylistStar", Title = "备忘录", Color = "#9C27B0", Target = "" });
            */
            this.TaskBars.Add(new TaskbarItemViewModel(new TaskBarModel() { Icon = "ClockFast", Title = "汇总", Color = "#3F51B5", Target = "ToDoView" }));
            this.TaskBars.Add(new TaskbarItemViewModel(new TaskBarModel() { Icon = "ClockCheckOutline", Title = "已完成", Color = "#4CAF50", Target = "ToDoView" }));
            this.TaskBars.Add(new TaskbarItemViewModel(new TaskBarModel() { Icon = "ChartLineVariant", Title = "完成率", Color = "#FF9800", Target = "" }));
            this.TaskBars.Add(new TaskbarItemViewModel(new TaskBarModel() { Icon = "PlaylistStar", Title = "备忘录", Color = "#9C27B0", Target = "MemoView" }));

            // 订阅首页汇总数据更新事件
            this._eventAggregator.SummaryChangedEventSubscribe(this.OnSummaryChanged);

        }

        public void UpdateFromSummary(SummaryViewModel summary)
        {
            this.TaskBars.Clear();

            /*
            this.TaskBars.Add(new TaskBarModel() { Icon = "ClockFast", Title = "汇总", Content = summary.ToDoTotalCount.ToString(), Color = "#FF0CA0FF", Target = "" });
            this.TaskBars.Add(new TaskBarModel() { Icon = "ClockCheckOutline", Title = "已完成", Content = summary.ToDoCompletedCount.ToString(), Color = "#FF1ECA3A", Target = "" });
            this.TaskBars.Add(new TaskBarModel() { Icon = "ChartLineVariant", Title = "完成率", Content = summary.ToDoCompletedProportion, Color = "#FF02C6DC", Target = "" });
            this.TaskBars.Add(new TaskBarModel() { Icon = "PlaylistStar", Title = "备忘录", Content = summary.MemoTotalCount.ToString(), Color = "#FF9800", Target = "" });
            */
            /*
            this.TaskBars.Add(new TaskBarModel() { Icon = "ClockFast", Title = "汇总", Content = summary.ToDoTotalCount.ToString(), Color = "#3F51B5", Target = "" });
            this.TaskBars.Add(new TaskBarModel() { Icon = "ClockCheckOutline", Title = "已完成", Content = summary.ToDoCompletedCount.ToString(), Color = "#4CAF50", Target = "" });
            this.TaskBars.Add(new TaskBarModel() { Icon = "ChartLineVariant", Title = "完成率", Content = summary.ToDoCompletedProportion, Color = "#FF9800", Target = "" });
            this.TaskBars.Add(new TaskBarModel() { Icon = "PlaylistStar", Title = "备忘录", Content = summary.MemoTotalCount.ToString(), Color = "#9C27B0", Target = "" });
            */
        }

        
        // 订阅首页汇总数据更新时的事件
        private void OnSummaryChanged(SummaryChangedEventArgs summaryChangedEventArgs)
        {
            // 更新待办事项总量、完成量、完成率、备忘录总量
            this.TaskBars[0].Content = summaryChangedEventArgs.ToDoTotalCount.ToString();
            this.TaskBars[1].Content = summaryChangedEventArgs.ToDoCompletedCount.ToString();
            this.TaskBars[2].Content = summaryChangedEventArgs.ToDoCompletedProportion;
            this.TaskBars[3].Content = summaryChangedEventArgs.MemoTotalCount.ToString();
        }
    }
}
