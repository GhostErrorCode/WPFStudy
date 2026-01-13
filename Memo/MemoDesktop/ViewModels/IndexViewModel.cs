using MahApps.Metro.Controls;
using MaterialDesignThemes.Wpf;
using MemoDesktop.ApiResponses;
using MemoDesktop.Converters;
using MemoDesktop.Dtos.Memo;
using MemoDesktop.Dtos.ToDo;
using MemoDesktop.Models;
using MemoDesktop.Services.Interfaces;
using MemoDesktop.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace MemoDesktop.ViewModels
{
    public class IndexViewModel : NavigationViewModel
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
        private readonly IDialogHostService _dialogHostService;
        // Prism中的事件处理器
        private readonly IEventAggregator _eventAggregator;
        // 备忘录服务层
        private readonly IMemoApiService _memoApiService;
        // 待办事项服务层
        private readonly IToDoApiService _toDoApiService;
        // =================== Defined Field End ===================================================================

        // 构造函数
        public IndexViewModel(IDialogHostService dialogHostService, IEventAggregator eventAggregator, IMemoApiService memoApiService, IToDoApiService toDoApiService) : base(eventAggregator)
        {
            // 初始化任务栏
            this.CreateTaskBars();

            // 初始化Command
            // 添加待办事项Command初始化方法
            this.AddToDoCommand = new DelegateCommand(AddToDo);
            // 添加备忘录Command初始化化方法
            this.AddMemoCommand = new DelegateCommand(AddMemo);

            // 初始化内部字段(依赖注入)
            this._dialogHostService = dialogHostService;
            this._eventAggregator = eventAggregator;
            this._memoApiService = memoApiService;
            this._toDoApiService = toDoApiService;
        }

        // 创建任务项
        private void CreateTaskBars()
        {
            this.ToDoCollection = new ObservableCollection<ToDoDto>();
            this.MemoCollection = new ObservableCollection<MemoDto>();
            this.TaskBarModels = new ObservableCollection<TaskBarModel>();
            TaskBarModels.Add(new TaskBarModel() { Icon = "ClockFast", Title = "汇总", Content = "999999", Color = "#FF0CA0FF", Target = "" });
            TaskBarModels.Add(new TaskBarModel() { Icon = "ClockCheckOutline", Title = "已完成", Content = "999999", Color = "#FF1ECA3A", Target = "" });
            TaskBarModels.Add(new TaskBarModel() { Icon = "ChartLineVariant", Title = "完成率", Content = "100%", Color = "#FF02C6DC", Target = "" });
            TaskBarModels.Add(new TaskBarModel() { Icon = "PlaylistStar", Title = "备忘录", Content = "999999", Color = "#FFFFA000", Target = "" });
        }

        // 添加待办事项方法
        private async void AddToDo()
        {
            try
            {
                // 显示对话框并接收返回结果
                IDialogResult dialogResult = await this._dialogHostService.ShowDialog("AddToDoDialog", new DialogParameters());
                // 判断返回结果是否为ButtonResult.OK
                if (dialogResult.Result == ButtonResult.OK)
                {
                    // 接收返回的参数值
                    ToDoDto toDoDto = dialogResult.Parameters.GetValue<ToDoDto>("Value");
                    // 判断是修改待办事项还是添加待办事项
                    if (toDoDto.Id > 0)
                    {
                        // 修改待办事项
                    }
                    else
                    {
                        // 添加待办事项
                        // 通过服务层请求添加待办事项数据
                        ApiResponse<ToDoDto> addToDoResponse = await this._toDoApiService.AddToDoAsync(ToDoDtoConverter.ConvertToDoDtoToAddToDoDto(toDoDto));
                        // 判断是否添加待办事项成功
                        if (addToDoResponse.IsSuccess)
                        {
                            // 将添加成功的待办事项放到集合中
                            this.ToDoCollection.Add(addToDoResponse.Data);
                        }
                        else
                        {
                            // 添加失败
                            Debug.WriteLine(addToDoResponse);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"添加/修改待办事项异常：{ex.Message}");
            }
        }

        // 添加备忘录方法
        private async void AddMemo()
        {
            // 显示对话框
            try
            {
                // 显示对话框并接收返回结果
                IDialogResult dialogResult = await this._dialogHostService.ShowDialog("AddMemoDialog", new DialogParameters());
                // 判断返回结果是否为ButtonResult.OK
                if (dialogResult.Result == ButtonResult.OK)
                {
                    // 接收返回的参数值
                    MemoDto memoDto = dialogResult.Parameters.GetValue<MemoDto>("Value");
                    // 判断是修改备忘录还是添加备忘录
                    if (memoDto.Id > 0)
                    {
                        // 修改备忘录
                    }
                    else
                    {
                        // 添加备忘录
                        // 通过服务层请求添加备忘录数据
                        ApiResponse<MemoDto> addMemoResponse = await this._memoApiService.AddMemoAsync(MemoDtoConverter.ConvertMemoDtoToAddMemoDto(memoDto));
                        // 判断是否添加备忘录成功
                        if (addMemoResponse.IsSuccess)
                        {
                            // 将添加成功的备忘录放到集合中
                            this.MemoCollection.Add(addMemoResponse.Data);
                        }
                        else
                        {
                            // 添加失败
                            Debug.WriteLine(addMemoResponse);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"添加/修改待办事项异常：{ex.Message}");
            }
        }
    }
}
