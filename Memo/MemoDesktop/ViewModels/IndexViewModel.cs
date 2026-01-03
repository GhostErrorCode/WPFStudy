using MahApps.Metro.Controls;
using MaterialDesignThemes.Wpf;
using MemoDesktop.Dtos.Memo;
using MemoDesktop.Dtos.ToDo;
using MemoDesktop.Models;
using MemoDesktop.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MemoDesktop.ViewModels
{
    public class IndexViewModel : BindableBase
    {
        // 字段及属性
        // 定义任务栏项（动态）
        private ObservableCollection<TaskBarModel> _taskBarModels;
        public ObservableCollection<TaskBarModel> TaskBarModels
        {
            get { return _taskBarModels; } 
            set { _taskBarModels = value; RaisePropertyChanged(); }
        }
        // 定义待办事项和备忘录的数据
        private ObservableCollection<ToDoDto> _toDoCollection;
        public ObservableCollection<ToDoDto> ToDoCollection
        {
            get { return _toDoCollection; }
            set { _toDoCollection = value; RaisePropertyChanged(); }
        }
        private ObservableCollection<MemoDto> _memoCollection;
        public ObservableCollection<MemoDto> MemoCollection
        {
            get { return _memoCollection; }
            set { _memoCollection = value; RaisePropertyChanged(); }
        }

        // =================== Defined Command Start ===================================================================
        // 添加待办事项Command
        public DelegateCommand AddToDoCommand { get; private set; }
        // 添加备忘录Command
        public DelegateCommand AddMemoCommand { get; private set; }
        // =================== Defined Command End =====================================================================


        // =================== Defined Field Start ===================================================================
        // Prism中的对话服务 IDialogService
        private readonly IDialogService _dialogService;
        // =================== Defined Field End ===================================================================

        // 构造函数
        public IndexViewModel(IDialogService dialogService)
        {
            // 初始化任务栏
            this.CreateTaskBars();

            // 初始化Command
            // 添加待办事项Command初始化方法
            this.AddToDoCommand = new DelegateCommand(AddToDo);
            // 添加备忘录Command初始化化方法
            this.AddMemoCommand = new DelegateCommand(AddMemo);

            // 初始化内部字段
            this._dialogService = dialogService;
        }

        // 创建任务项
        private void CreateTaskBars()
        {
            this.TaskBarModels = new ObservableCollection<TaskBarModel>();
            TaskBarModels.Add(new TaskBarModel() { Icon = "ClockFast", Title = "汇总", Content = "9", Color = "#FF0CA0FF", Target = "" });
            TaskBarModels.Add(new TaskBarModel() { Icon = "ClockCheckOutline", Title = "已完成", Content = "9", Color = "#FF1ECA3A", Target = "" });
            TaskBarModels.Add(new TaskBarModel() { Icon = "ChartLineVariant", Title = "完成率", Content = "100%", Color = "#FF02C6DC", Target = "" });
            TaskBarModels.Add(new TaskBarModel() { Icon = "PlaylistStar", Title = "备忘录", Content = "19", Color = "#FFFFA000", Target = "" });
        }

        // 添加待办事项方法
        private void AddToDo()
        {
            // 显示对话框
            this._dialogService.ShowDialog("AddToDoDialog"); 
        }

        // 添加备忘录方法
        private void AddMemo()
        {
            // 显示对话框
            this._dialogService.ShowDialog("AddMemoDialog");
        }
    }
}
