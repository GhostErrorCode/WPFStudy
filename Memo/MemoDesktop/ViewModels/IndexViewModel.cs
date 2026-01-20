using MahApps.Metro.Controls;
using MaterialDesignThemes.Wpf;
using MemoDesktop.ApiResponses;
using MemoDesktop.Constants;
using MemoDesktop.Converters;
using MemoDesktop.Dtos.Dashboard;
using MemoDesktop.Dtos.Memo;
using MemoDesktop.Dtos.ToDo;
using MemoDesktop.Extensions;
using MemoDesktop.Models;
using MemoDesktop.Services.Interfaces;
using MemoDesktop.Utilities;
using MemoDesktop.ViewModels.Dashboard;
using MemoDesktop.Views.Dialogs;
using Prism.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

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

        // 首页的提示信息
        private string _indexTitle = $"欢迎回来！今天是{DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss")}";
        public string IndexTitle
        {
            get { return _indexTitle; }
            set { _indexTitle  = value; RaisePropertyChanged(); }
        }

        // 定义汇总数据的ViewModel
        private SummaryViewModel _summary;
        public SummaryViewModel Summary
        {
            get { return _summary; }
            set { _summary = value; RaisePropertyChanged(); }
        }

        // 定义首页汇总栏的ViewModel
        private TaskBarViewModel _taskBar;
        public TaskBarViewModel TaskBar
        {
            get { return _taskBar; }
            set { _taskBar = value; RaisePropertyChanged(); }
        }

        // =================== Defined Command Start ===================================================================
        // 添加待办事项Command
        public DelegateCommand AddToDoCommand { get; private set; }
        // 添加备忘录Command
        public DelegateCommand AddMemoCommand { get; private set; }
        // 编辑待办事项Command
        public DelegateCommand<ToDoDto> EditToDoCommand { get; private set; }
        // 编辑备忘录Command
        public DelegateCommand<MemoDto> EditMemoCommand { get; private set; }
        // 完成待办事项Command
        public DelegateCommand<ToDoDto> ToDoCompletedCommand { get; private set; }
        // 首页任务栏导航Command
        public DelegateCommand<TaskbarItemViewModel> IndexTaskBarItemNavigateCommand { get; private set; }
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
        // 汇总数据服务层
        private readonly ISummaryApiService _summaryApiService;
        // 全局区域导航
        private readonly IRegionManager _regionManager;
        // =================== Defined Field End ===================================================================

        // 构造函数
        public IndexViewModel(IDialogHostService dialogHostService, IEventAggregator eventAggregator, IMemoApiService memoApiService, IToDoApiService toDoApiService, ISummaryApiService summaryApiService, IRegionManager regionManager) : base(eventAggregator)
        {
            // 初始化任务栏
            // this.CreateTaskBars();

            // 初始化Command
            // 添加待办事项Command初始化方法
            this.AddToDoCommand = new DelegateCommand(AddToDo);
            // 添加备忘录Command初始化化方法
            this.AddMemoCommand = new DelegateCommand(AddMemo);
            // 编辑待办事项Command方法
            this.EditToDoCommand = new DelegateCommand<ToDoDto>(EditToDo);
            // 编辑备忘录Command方法
            this.EditMemoCommand = new DelegateCommand<MemoDto>(EditMemo);
            // 完成待办事项Command方法
            this.ToDoCompletedCommand = new DelegateCommand<ToDoDto>(ToDoCompleted);
            // // 首页任务栏导航Command方法
            this.IndexTaskBarItemNavigateCommand = new DelegateCommand<TaskbarItemViewModel>(IndexTaskBarItemNavigate);

            // 初始化内部字段(依赖注入)
            this._dialogHostService = dialogHostService;
            this._eventAggregator = eventAggregator;
            this._memoApiService = memoApiService;
            this._toDoApiService = toDoApiService;
            this._summaryApiService = summaryApiService;
            this._regionManager = regionManager;

            // 初始化属性
            this.Summary = new SummaryViewModel(this._eventAggregator);
            this.TaskBar = new TaskBarViewModel(this._eventAggregator);
        }

        // 创建任务项
        private async void InitIndex()
        {
            /*
            this.ToDoCollection = new ObservableCollection<ToDoDto>();
            this.MemoCollection = new ObservableCollection<MemoDto>();
            this.TaskBarModels = new ObservableCollection<TaskBarModel>();
            TaskBarModels.Add(new TaskBarModel() { Icon = "ClockFast", Title = "汇总", Content = "999999", Color = "#FF0CA0FF", Target = "" });
            TaskBarModels.Add(new TaskBarModel() { Icon = "ClockCheckOutline", Title = "已完成", Content = "999999", Color = "#FF1ECA3A", Target = "" });
            TaskBarModels.Add(new TaskBarModel() { Icon = "ChartLineVariant", Title = "完成率", Content = "100%", Color = "#FF02C6DC", Target = "" });
            TaskBarModels.Add(new TaskBarModel() { Icon = "PlaylistStar", Title = "备忘录", Content = "999999", Color = "#FFFFA000", Target = "" });
            */
            try
            {
                // 显示加载动画
                this.ShowLoading("初始化首页中...", "IndexViewModel");
                // 先从后端拿出来汇总后的数据
                ApiResponse<SummaryDto> apiResponse = await this._summaryApiService.GetSummaryAsync();
                // 验证
                if (apiResponse.IsSuccess)
                {
                    // 更新子ViewModel
                    this.Summary.LoadFromDto(apiResponse.Data);
                }
                else
                {
                    // 如果后端返回失败的话，就再获取一次
                    apiResponse = await this._summaryApiService.GetSummaryAsync();
                    // 更新下子ViewModel
                    this.Summary.LoadFromDto(apiResponse.Data);
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"初始化首页异常：{ex.Message}");
            }
            finally
            {
                // 隐藏加载动画
                this.HideLoading();
            }

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
                    // 显示加载动画
                    this.ShowLoading("正在添加待办事项...", "IndexViewModel");
                    // 接收返回的参数值
                    ToDoDto toDoDto = dialogResult.Parameters.GetValue<ToDoDto>("Value");
                    // 判断是修改待办事项还是添加待办事项
                    if (toDoDto.Id > 0)
                    {
                        // 修改待办事项
                        // ===== 此if分支已废弃=====
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
                            this.Summary.AddToDo(addToDoResponse.Data);
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
                Debug.WriteLine($"添加待办事项异常：{ex.Message}");
            }
            finally
            {
                // 隐藏加载动画
                this.HideLoading();
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
                    // 显示加载动画
                    this.ShowLoading("正在添加备忘录...", "IndexViewModel");
                    // 接收返回的参数值
                    MemoDto memoDto = dialogResult.Parameters.GetValue<MemoDto>("Value");
                    // 判断是修改备忘录还是添加备忘录
                    if (memoDto.Id > 0)
                    {
                        // 修改备忘录
                        // ===== 此if分支已废弃=====
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
                            this.Summary.AddMemo(addMemoResponse.Data);
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
                Debug.WriteLine($"添加备忘录异常：{ex.Message}");
            }
            finally
            {
                // 隐藏加载动画
                this.HideLoading();
            }
        }

        // 编辑待办事项方法
        private async void EditToDo(ToDoDto toDoDto)
        {
            try
            {
                // 准备传入对话框的参数
                DialogParameters dialogParameters = new DialogParameters();
                // 先判断传入的值是否为NULL
                if (toDoDto == null)
                {
                    return;
                }
                else
                {
                    // toDoDto不为NULL贼赋值给参数
                    dialogParameters.Add("Value", toDoDto);
                }

                // 显示对话框并接收返回结果
                IDialogResult dialogResult = await this._dialogHostService.ShowDialog("AddToDoDialog", dialogParameters);
                // 判断返回结果是否为ButtonResult.OK
                if (dialogResult.Result == ButtonResult.OK)
                {
                    // 显示加载动画
                    this.ShowLoading("正在修改待办事项...", "IndexViewModel");
                    // 接收对话框返回的修改后的待办事项
                    ToDoDto editToDo = dialogResult.Parameters.GetValue<ToDoDto>("Value");
                    // 开始调用待办事项服务接口修改方法
                    ApiResponse<ToDoDto> editToDoResponse = await this._toDoApiService.UpdateToDoAsync(ToDoDtoConverter.ConvertToDoDtoToUpdateToDoDto(editToDo));
                    // 判断后端返回修改待办事项后的状态
                    if (editToDoResponse.IsSuccess)
                    {
                        // 调用Summary的修改待办事项方法
                        this.Summary.EditToDo(editToDoResponse.Data);
                        /*
                        // 如果返回的是成功标识，在前端的结果集中找到对应的待办事项并修改
                        ToDoDto toDoByToDoCollection = this.ToDoCollection.FirstOrDefault((ToDoDto toDo) => toDo.Id == editToDo.Id);
                        // 如果找到了就更新他
                        if(toDoByToDoCollection != null)
                        {
                            toDoByToDoCollection.Status = editToDo.Status;
                            toDoByToDoCollection.Title = editToDo.Title;
                            toDoByToDoCollection.Content = editToDo.Content;
                        }
                        */
                    }
                    if(editToDoResponse.Data.Status == 1) { this._eventAggregator.MessageEventPublish($"待办事项 {editToDoResponse.Data.Title} 已完成!"); }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"修改待办事项异常：{ex.Message}");
            }
            finally
            {
                // 隐藏加载动画
                this.HideLoading();
            }
        }

        // 编辑备忘录方法
        private async void EditMemo(MemoDto memoDto)
        {
            try
            {
                // 准备传入对话框的参数
                DialogParameters dialogParameters = new DialogParameters();
                // 先判断传入的值是否为NULL
                if (memoDto == null)
                {
                    return;
                }
                else
                {
                    // toDoDto不为NULL贼赋值给参数
                    dialogParameters.Add("Value", memoDto);
                }

                // 显示对话框并接收返回结果
                IDialogResult dialogResult = await this._dialogHostService.ShowDialog("AddMemoDialog", dialogParameters);
                // 判断返回结果是否为ButtonResult.OK
                if (dialogResult.Result == ButtonResult.OK)
                {
                    // 显示加载动画
                    this.ShowLoading("正在修改备忘录...", "IndexViewModel");
                    // 接收对话框返回的修改后的待办事项
                    MemoDto editMemo = dialogResult.Parameters.GetValue<MemoDto>("Value");
                    // 开始调用待办事项服务接口修改方法
                    ApiResponse<MemoDto> editMemoResponse = await this._memoApiService.UpdateMemoAsync(MemoDtoConverter.ConvertMemoDtoToUpdateMemoDto(editMemo));
                    // 判断后端返回修改待办事项后的状态
                    if (editMemoResponse.IsSuccess)
                    {
                        // 调用Summary的修改待办事项方法
                        this.Summary.EditMemo(editMemoResponse.Data);
                        /*
                        // 如果返回的是成功标识，在前端的结果集中找到对应的待办事项并修改
                        MemoDto memoByMemoCollection = this.MemoCollection.FirstOrDefault((MemoDto memo) => memo.Id == editMemo.Id);
                        // 如果找到了就更新他
                        if (memoByMemoCollection != null)
                        {
                            memoByMemoCollection.Title = editMemo.Title;
                            memoByMemoCollection.Content = editMemo.Content;
                        }
                        */
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"修改待办事项异常：{ex.Message}");
            }
            finally
            {
                // 隐藏加载动画
                this.HideLoading();
            }
        }

        // 完成备忘录方法
        private async void ToDoCompleted(ToDoDto toDoDto)
        {
            try
            {
                // 显示加载的对话框
                this.ShowLoading($"待办事项\"{toDoDto.Title}\"已完成,刷新中...", "IndexViewModel");
                // 完成（状态更新）待办事项
                ApiResponse<ToDoDto> apiResponse = await this._toDoApiService.UpdateToDoAsync(ToDoDtoConverter.ConvertToDoDtoToUpdateToDoDto(toDoDto));
                // 判断是否修改成功
                if (apiResponse.IsSuccess)
                {
                    // 调用Summary的修改待办事项方法
                    this.Summary.EditToDo(apiResponse.Data);
                    /*
                    // 修改成功话就从当前待办事项集合中移除
                    ToDoDto toDoByToDoCollection = this.ToDoCollection.FirstOrDefault((ToDoDto toDo) => toDo.Id == toDoDto.Id);
                    // 如果找到了，就从集合中移除它
                    if(toDoByToDoCollection != null)
                    {
                        this.ToDoCollection.Remove(toDoByToDoCollection);
                    }
                    */
                }
                this._eventAggregator.MessageEventPublish($"待办事项 {apiResponse.Data.Title} 已完成!");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"完成待办事项异常：{ex.Message}");
            }
            finally
            {
                // 隐藏加载动画
                this.HideLoading();
            }
        }

        // 首页任务栏导航方法
        private void IndexTaskBarItemNavigate(TaskbarItemViewModel taskbarItemViewModel)
        {
            // 如果任务栏的Target是空，则直接return，无需导航
            if (string.IsNullOrWhiteSpace(taskbarItemViewModel.Target)) { return; }

            // 如果任务栏的Target不为空，则开始准备导航
            // 初始化导航参数
            NavigationParameters navigationParameters = new NavigationParameters();
            // 判断任务栏标题是什么,全部待办事项，已完成的待办事项，备忘录页面
            if(taskbarItemViewModel.Title == "已完成")
            {
                // 这里是点击的已完成的待办事项，需要传入一个参数
                navigationParameters.Add("ToDoStatus", 1);
            }
            // 导航
            this._regionManager.Regions[RegionNames.MainViewRegionName].RequestNavigate(taskbarItemViewModel.Target, navigationParameters);
        }

        // 刷新首页提示信息的方法
        private void UpdateIndexTitle()
        {
            // 初始化首页提示信息
            // this.IndexTitle = $"欢迎回来！今天是{DateTimeUtility.CurrentDateTime}";

            // 计时器开始运行，每秒更新一次当前的日期和时间
            DispatcherTimer timer = new DispatcherTimer();
            // 设置计时器间隔为1秒（1000毫秒）
            timer.Interval = TimeSpan.FromSeconds(1);
            // 注册Tick事件处理程序
            // 使用lambda表达式简化事件处理器的定义
            // 每次计时器触发时（每秒一次）：
            // 1. 获取当前系统时间
            // 2. 格式化时间字符串
            // 3. 更新CurrentDateTime属性值
            // 4. 自动触发TimeChanged事件通知所有订阅者
            timer.Tick += (sender, e) => this.IndexTitle = $"欢迎回来！今天是{DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss")}";
            // 启动计时器，开始每秒更新时间
            timer.Start();
        }


        // 重写父类OnNavigatedTo方法，用于导航到此ViewModel中时需要什么操作
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            // 调用父类方法（如果父类有操作）
            base.OnNavigatedTo(navigationContext);

            // 自己写的操作，需要做什么事儿
            // 初始化首页
            this.InitIndex();
            // 获取当前日期时间以便显示
            this.UpdateIndexTitle();
        }

        /*
        public void Dispose()
        {
            // 用于取消事件订阅，防止内存泄露
            DateTimeUtility.TimeChanged -= OnTimeChanged;
        }
        */
    }
}
