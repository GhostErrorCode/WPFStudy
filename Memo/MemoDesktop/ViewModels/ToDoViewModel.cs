using MemoDesktop.ApiResponses;
using MemoDesktop.Converters;
using MemoDesktop.Dtos.Memo;
using MemoDesktop.Dtos.ToDo;
using MemoDesktop.Models;
using MemoDesktop.Models.ViewModels;
using MemoDesktop.Services.Implements;
using MemoDesktop.Services.Interfaces;
using MemoDesktop.Utilities;
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
        // 用于显示待办事项的集合
        private ObservableCollection<ToDoDto> _selectedToDoDataCollection;
        public ObservableCollection<ToDoDto> SelectedToDoDataCollection
        {
            get { return _selectedToDoDataCollection; }
            set { _selectedToDoDataCollection = value; RaisePropertyChanged(); }
        }
        // 右侧添加待办事项是否展开
        private bool _isRightDrawerOpen;
        public bool IsRightDrawerOpen
        {
            get { return _isRightDrawerOpen; }
            set { _isRightDrawerOpen = value; RaisePropertyChanged(); }
        }
        // 添加或编辑时存储的对象
        private ToDoDto _addOrUpdateToDoData;
        public ToDoDto AddOrUpdateToDoData
        {
            get { return _addOrUpdateToDoData; }
            set { _addOrUpdateToDoData = value; RaisePropertyChanged(); }
        }
        // 搜索文本
        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set { _searchText = value; RaisePropertyChanged(); }
        }
        // 搜索组合框
        private ObservableCollection<ToDoStatusItem> _searchComboBox;
        public ObservableCollection<ToDoStatusItem> SearchComboBox
        {
            get
            {
                if (_searchComboBox == null)
                {
                    _searchComboBox = ToDoStatusItemUtility.GetToDoStatusItems();
                }
                return _searchComboBox;
            }
            set { _searchComboBox = value; RaisePropertyChanged(); }
        }
        // 搜索组合框选中的值
        private int? _searchStatus;
        public int? SearchStatus
        {
            get { return _searchStatus; }
            set { _searchStatus = value; RaisePropertyChanged(); }
        }

        // 添加选中待办事项Command
        public DelegateCommand<ToDoDto> SelectedToDoDataCommand { get; private set; }
        // 添加待办事项Command
        public DelegateCommand AddToDoDataCommand { get; private set; }
        // 添加/修改待办事项后保存Command（Save）
        public DelegateCommand SaveToDoDataCommand { get; private set; }
        // 带条件查询待办事项Command
        public DelegateCommand SelectToDoDataByConditionCommand {  get; private set; }



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

            // 绑定选中待办事项的Command方法
            this.SelectedToDoDataCommand = new DelegateCommand<ToDoDto>(SelectedToDoData);
            // 绑定添加待办事项Command方法
            this.AddToDoDataCommand = new DelegateCommand(AddToDoData);
            // 绑定添加/修改待办事项后保存Command方法
            this.SaveToDoDataCommand = new DelegateCommand(SaveToDoData);
            // 绑定带条件查询待办事项Command方法
            this.SelectToDoDataByConditionCommand = new DelegateCommand(GetToDoDataByCondition);
        }



        private async Task GetAllToDoData()
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
                this.SelectedToDoDataCollection = new ObservableCollection<ToDoDto>();
                // 通过服务层请求获取全部的待办事项数据
                ApiResponse<List<ToDoDto>> response = await this._toDoApiService.GetAllToDoAsync();
                // 判断是否获取成功
                if (response.IsSuccess)
                {
                    // 先清除集合内所有数据
                    this.SelectedToDoDataCollection.Clear();
                    // 将响应里的Data拿出来，存入 ToDoDataModels 集合内
                    foreach (ToDoDto memoDto in response.Data)
                    {
                        this.SelectedToDoDataCollection.Add(memoDto);
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

        // 选中待办事项方法
        private void SelectedToDoData(ToDoDto toDoDto)
        {
            // 给AddOrUpdateToDoData赋值
            // this.AddOrUpdateToDoData = toDoDto;  // 这样直接写的话会造成双向绑定，哪怕不提交，你修改的部分也会在前端生效
            this.AddOrUpdateToDoData = new ToDoDto()
            {
                Id = toDoDto.Id,
                Content = toDoDto.Content,
                Status = toDoDto.Status,
                Title = toDoDto.Title
            };
            // 并打开右侧栏
            this.IsRightDrawerOpen = true;
        }

        // 添加待办事项方法
        private void AddToDoData()
        {
            // 先判断_addOrUpdateToDoData是不是空的
            if (this._addOrUpdateToDoData == null) { this.AddOrUpdateToDoData = new ToDoDto(); }
            // 再将AddOrUpdateToDoData置空
            this.AddOrUpdateToDoData.Id = default(int);
            this.AddOrUpdateToDoData.Title = default(string);
            this.AddOrUpdateToDoData.Content = default(string);
            this.AddOrUpdateToDoData.Status = default(int);
            this.IsRightDrawerOpen = true;
        }

        // 修改待办事项方法
        private async void SaveToDoData()
        {
            try
            {
                // 先判断待办事项标题、内容、状态是否为空，为空直接结束方法
                if (string.IsNullOrWhiteSpace(this._addOrUpdateToDoData.Title) || string.IsNullOrWhiteSpace(this._addOrUpdateToDoData.Content))
                {
                    return;
                }
                // 不为空的话继续，先显示加载动画
                this.ShowLoading("刷新待办事项...", "ToDoViewModel");
                
                // 用ID区分添加和修改
                if(this._addOrUpdateToDoData.Id > 0)
                {
                    // 修改业务逻辑
                    // 通过服务层请求修改指定待办事项数据
                    ApiResponse<ToDoDto> response = await this._toDoApiService.UpdateToDoAsync(ToDoDtoConverter.ConvertToDoDtoToUpdateToDoDto(this._addOrUpdateToDoData));
                    // 修改成功
                    if (response.IsSuccess)
                    {
                        // 更新前端的状态
                        ToDoDto? toDo = this.SelectedToDoDataCollection.FirstOrDefault((ToDoDto toDoDto) => toDoDto.Id == this._addOrUpdateToDoData.Id);
                        // 如果找到的话
                        if(toDo != null) { toDo.Title=this._addOrUpdateToDoData.Title; toDo.Status=this._addOrUpdateToDoData.Status; toDo.Content = this._addOrUpdateToDoData.Content; }
                    }
                    else
                    {
                        // 修改失败
                        Debug.WriteLine(response);
                    }
                    // 关闭右侧添加/编辑栏
                    this.IsRightDrawerOpen = false;
                }
                else
                {
                    // 添加业务逻辑
                    // 通过服务层请求添加待办事项数据
                    ApiResponse<ToDoDto> response = await this._toDoApiService.AddToDoAsync(ToDoDtoConverter.ConvertToDoDtoToAddToDoDto(this._addOrUpdateToDoData));
                    if (response.IsSuccess)
                    {
                        // 将添加成功的待办事项放到集合中
                        this.SelectedToDoDataCollection.Add(response.Data);
                        this._isRightDrawerOpen = false;
                    }
                    else
                    {
                        // 添加失败
                        Debug.WriteLine(response);
                    }
                    // 关闭右侧添加/编辑栏
                    this.IsRightDrawerOpen = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"添加/修改待办事项异常：{ex.Message}");
            }
            finally
            {
                this.HideLoading();
            }
        }

        // 根据条件查询待办事项
        private async void GetToDoDataByCondition()
        {
            // 显示加载动画
            // this 确实指代当前类的实例，但在面向对象继承中，this 可以访问的成员包括从父类继承下来的成员。
            this.ShowLoading("正在查询待办事项...", "ToDoViewModel");

            // 确保资源清理或状态恢复的代码绝对执行，防止程序状态“卡死”。
            // 在加载动画这个场景下，finally 的作用就是：无论成功还是失败，都必须关闭加载动画。
            try
            {
                // ===== 执行业务逻辑 =====
                // 模拟等待时间
                // await Task.Delay(10000);
                // 初始化ObservableCollection对象
                // this.SelectedToDoDataCollection = new ObservableCollection<ToDoDto>();
                // 通过服务层请求获取全部的待办事项数据
                ApiResponse<List<ToDoDto>> response = await this._toDoApiService.GetToDoByConditionAsync(this.SearchText, this.SearchStatus);
                // 判断是否获取成功
                if (response.IsSuccess)
                {
                    // 先清除集合内所有数据
                    this.SelectedToDoDataCollection.Clear();
                    // 将响应里的Data拿出来，存入 ToDoDataModels 集合内
                    foreach (ToDoDto memoDto in response.Data)
                    {
                        this.SelectedToDoDataCollection.Add(memoDto);
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



        // 重写父类OnNavigatedTo方法，用于导航到此ViewModel中时需要什么操作
        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            // 调用父类方法（如果父类有操作）
            base.OnNavigatedTo(navigationContext);

            // 自己写的操作，需要做什么事儿
            // 获取全部待办事项
            // 等待加载完整后进一步操作
            await this.GetAllToDoData();
        }
    }
}
