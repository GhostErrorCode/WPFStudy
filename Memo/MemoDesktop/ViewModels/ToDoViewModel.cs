using MemoDesktop.ApiResponses;
using MemoDesktop.Dtos.Memo;
using MemoDesktop.Dtos.ToDo;
using MemoDesktop.Models;
using MemoDesktop.Services;
using MemoDesktop.Services.Implements;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace MemoDesktop.ViewModels
{
    public class ToDoViewModel : BindableBase
    {
        // 字段和属性
        private ObservableCollection<ToDoDto> _toDoDataModels;
        public ObservableCollection<ToDoDto> ToDoDataModels
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
        // 待办事项Api相关服务Service
        private readonly IToDoApiService _toDoApiService;


        // 构造函数
        public ToDoViewModel(IToDoApiService toDoApiService)
        {
            // 获取备忘录Api服务
            this._toDoApiService = toDoApiService;
            this.GetAllToDo();

            // 绑定添加待办事项Command方法
            this.AddToDoDataCommand = new DelegateCommand(AddToDoData);
        }

        private async Task GetAllToDo()
        {
            this.ToDoDataModels = new ObservableCollection<ToDoDto>();
            ApiResponse<List<ToDoDto>> response = await this._toDoApiService.GetAllToDoAsync();
            if (response.IsSuccess)
            {
                this.ToDoDataModels.Clear();
                foreach (ToDoDto memoDto in response.Data)
                {
                    this.ToDoDataModels.Add(memoDto);
                }
            }
        }

        // 添加待办事项方法
        private void AddToDoData()
        {
            IsRightDrawerOpen = true;
        }
    }
}
