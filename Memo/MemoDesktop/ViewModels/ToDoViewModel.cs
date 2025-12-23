using MemoDesktop.ApiResponses;
using MemoDesktop.Dtos.Memo;
using MemoDesktop.Dtos.ToDo;
using MemoDesktop.Models;
using MemoDesktop.Services.Implements;
using MemoDesktop.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace MemoDesktop.ViewModels
{
    public class ToDoViewModel : NavigationViewModel
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
        // 事件聚合器
        private readonly IEventAggregator _eventAggregator;

        // 构造函数
        public ToDoViewModel(IToDoApiService toDoApiService, IEventAggregator eventAggregator) : base(eventAggregator)
        {
            // 获取备忘录Api服务
            this._toDoApiService = toDoApiService;
            // 获取事件聚合器
            this._eventAggregator = eventAggregator;
            // 绑定添加待办事项Command方法
            this.AddToDoDataCommand = new DelegateCommand(AddToDoData);
        }

        private async Task GetAllToDo()
        {
            // 显示加载动画
            // this 确实指代当前类的实例，但在面向对象继承中，this 可以访问的成员包括从父类继承下来的成员。
            this.ShowLoading("正在加载待办事项...", "ToDoViewModel");

            // 确保资源清理或状态恢复的代码绝对执行，防止程序状态“卡死”。
            // 在加载动画这个场景下，finally 的作用就是：无论成功还是失败，都必须关闭加载动画。
            try
            {
                // ===== 执行业务逻辑 =====
                // 模拟等待时间
                // await Task.Delay(10000);
                // 初始化ObservableCollection对象
                this.ToDoDataModels = new ObservableCollection<ToDoDto>();
                // 通过服务层请求获取全部的待办事项数据
                ApiResponse<List<ToDoDto>> response = await this._toDoApiService.GetAllToDoAsync();
                // 判断是否获取成功
                if (response.IsSuccess)
                {
                    // 先清除集合内所有数据
                    this.ToDoDataModels.Clear();
                    // 将响应里的Data拿出来，存入 ToDoDataModels 集合内
                    foreach (ToDoDto memoDto in response.Data)
                    {
                        this.ToDoDataModels.Add(memoDto);
                    }
                }
                else
                {
                    // 响应体为失败
                    Debug.WriteLine(response);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"加载待办事项异常：{ex.Message}");
            }
            finally
            {
                // 隐藏加载动画
                this.HideLoading();
            }
        }

        // 添加待办事项方法
        private void AddToDoData()
        {
            IsRightDrawerOpen = true;
        }


        // 重写父类OnNavigatedTo方法，用于导航到此ViewModel中时需要什么操作
        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            // 调用父类方法（如果父类有操作）
            base.OnNavigatedTo(navigationContext);

            // 自己写的操作，需要做什么事儿
            // 获取全部待办事项
            // 等待加载完整后进一步操作
            await this.GetAllToDo();
        }
    }
}
