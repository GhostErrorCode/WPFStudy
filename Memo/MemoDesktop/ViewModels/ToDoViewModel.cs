using MemoDesktop.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MemoDesktop.ViewModels
{
    public class ToDoViewModel : BindableBase
    {
        // 字段和属性
        private ObservableCollection<ToDoDataModel> _toDoDataModels;
        public ObservableCollection<ToDoDataModel> ToDoDataModels
        {
            get { return _toDoDataModels; }
            set { _toDoDataModels = value; RaisePropertyChanged(); }
        }
        // 添加待办事项Command
        public DelegateCommand AddToDoDataCommand { get; private set; }
        // 右侧添加待办事项是否展开
        private bool _isRightDrawerOpen;
        public bool IsRightDrawerOpen
        {
            get { return _isRightDrawerOpen; }
            set { _isRightDrawerOpen = value; RaisePropertyChanged(); }
        }



        // 构造函数
        public ToDoViewModel()
        {
            // 创建待办事项的测试数据
            this.CreateToDoDataModels();

            // 绑定添加待办事项Command方法
            this.AddToDoDataCommand = new DelegateCommand(AddToDoData);
        }

        private void CreateToDoDataModels()
        {
            // 初始化待办事项属性
            this.ToDoDataModels = new ObservableCollection<ToDoDataModel>();
            // 循环添加数据
            for(int i = 0; i < 20; i++)
            {
                this.ToDoDataModels.Add(new ToDoDataModel() { Id = i, Title = "待办事项" + i, Content = "待办事项:" + i, Status = 0, CreateDate = DateTime.Now, UpdateDate = DateTime.Now });
            }
        }

        // 添加待办事项方法
        private void AddToDoData()
        {
            IsRightDrawerOpen = true;
        }
    }
}
