using MahApps.Metro.Controls;
using MaterialDesignThemes.Wpf;
using MemoDesktop.Models;
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
        private ObservableCollection<ToDoDataModel> _toDoDataModels;
        public ObservableCollection<ToDoDataModel> ToDoDataModels
        {
            get { return _toDoDataModels; }
            set { _toDoDataModels = value; RaisePropertyChanged(); }
        }
        private ObservableCollection<MemoDataModel> _memoDataModels;
        public ObservableCollection<MemoDataModel> MemoDataModels
        {
            get { return _memoDataModels; }
            set { _memoDataModels = value; RaisePropertyChanged(); }
        }


        // 构造函数
        public IndexViewModel()
        {
            // 初始化任务栏
            this.CreateTaskBars();

            // 创建临时待办事项和备忘录测试数据
            this.CreateToDoAndMemoTestData();
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

        // 创建待办事项和备忘录的测试数据
        private void CreateToDoAndMemoTestData()
        {
            this.ToDoDataModels = new ObservableCollection<ToDoDataModel>();
            this.MemoDataModels = new ObservableCollection<MemoDataModel>();

            for (int i = 0; i < 15; i++)
            {
                ToDoDataModels.Add(new ToDoDataModel() { Id = i, Title = "待办事项" + i, Content = "待办事项:" + i, Status = 0, CreateDate = DateTime.Now, UpdateDate = DateTime.Now });
                MemoDataModels.Add(new MemoDataModel() { Id = i, Title = "备忘录" + i, Content = "备忘录:" + i, Status = 0, CreateDate = DateTime.Now, UpdateDate = DateTime.Now });
            }
        }
    }
}
