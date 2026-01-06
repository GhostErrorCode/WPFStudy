using MemoDesktop.ApiResponses;
using MemoDesktop.Converters;
using MemoDesktop.Dtos.Memo;
using MemoDesktop.Dtos.ToDo;
using MemoDesktop.Extensions;
using MemoDesktop.Models;
using MemoDesktop.Services.Implements;
using MemoDesktop.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

namespace MemoDesktop.ViewModels
{
    public class MemoViewModel : NavigationViewModel
    {
        // 字段和属性
        private ObservableCollection<MemoDto> _selectedMemoDataCollection;
        public ObservableCollection<MemoDto> SelectedMemoDataCollection
        {
            get { return _selectedMemoDataCollection; }
            set { _selectedMemoDataCollection = value; RaisePropertyChanged(); }
        }

        // 右侧添加待办事项是否展开
        private bool _isRightDrawerOpen;
        public bool IsRightDrawerOpen
        {
            get { return _isRightDrawerOpen; }
            set { _isRightDrawerOpen = value; RaisePropertyChanged(); }
        }

        // 搜索文本
        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set { _searchText = value; RaisePropertyChanged(); }
        }

        // 添加或编辑时存储的对象
        private MemoDto _addOrUpdateMemoData;
        public MemoDto AddOrUpdateMemoData
        {
            get { return _addOrUpdateMemoData; }
            set { _addOrUpdateMemoData = value; RaisePropertyChanged(); }
        }


        // 添加待办事项Command
        public DelegateCommand AddMemoDataCommand { get; private set; }
        // 带条件查询备忘录Command
        public DelegateCommand SelectMemoDataByConditionCommand { get; private set; }
        // 添加选中的备忘录Command
        public DelegateCommand<MemoDto> SelectedMemoDataCommand { get; private set; }
        // 添加/修改备忘录后保存Command（Save）
        public DelegateCommand SaveMemoDataCommand { get; private set; }
        // 删除待办事项Command
        public DelegateCommand<MemoDto> DeleteMemoDataCommand { get; private set; }



        // 备忘录Api相关服务Service
        private readonly IMemoApiService _memoApiService;
        // 事件聚合器
        private readonly IEventAggregator _eventAggregator;
        // 自定义对话服务
        private readonly IDialogHostService _dialogHostService;

        // 构造函数
        public MemoViewModel(IMemoApiService memoApiService, IEventAggregator eventAggregator, IDialogHostService dialogHostService) : base(eventAggregator)
        {
            // 获取备忘录Api服务
            this._memoApiService = memoApiService;
            // 获取全局事件聚合器
            this._eventAggregator = eventAggregator;
            // 获取自定义对话服务
            this._dialogHostService = dialogHostService;


            // 绑定添加备忘录Command方法
            this.AddMemoDataCommand = new DelegateCommand(AddMemoData);
            // 绑定带条件查询备忘录Command方法
            this.SelectMemoDataByConditionCommand = new DelegateCommand(GetMemoDataByCondition);
            // 绑定选中备忘录的Command方法
            this.SelectedMemoDataCommand = new DelegateCommand<MemoDto>(SelectedMemoData);
            // 绑定添加/修改备忘录后保存Command方法
            this.SaveMemoDataCommand = new DelegateCommand(SaveMemoData);
            // 绑定删除备忘录Command方法
            this.DeleteMemoDataCommand = new DelegateCommand<MemoDto>(DeleteMemoData);


            // 调用方法获取所有备忘录数据
            // this.GetAllMemo();
            // 不应该再这里调用获取全部数据，应该再OnNavigatedTo方法中
        }

        private async Task GetAllMemo()
        {
            // 显示加载动画
            // this 确实指代当前类的实例，但在面向对象继承中，this 可以访问的成员包括从父类继承下来的成员。
            this.ShowLoading("正在加载备忘录...", "MemoViewModel");

            // 确保资源清理或状态恢复的代码绝对执行，防止程序状态“卡死”。
            // 在加载动画这个场景下，finally 的作用就是：无论成功还是失败，都必须关闭加载动画。
            try
            {
                // ===== 执行业务逻辑 =====
                // 模拟等待时间
                // await Task.Delay(10000);
                // 初始化ObservableCollection对象
                this.SelectedMemoDataCollection = new ObservableCollection<MemoDto>();
                // 通过服务层请求获取全部的备忘录数据
                ApiResponse<List<MemoDto>> response = await this._memoApiService.GetAllMemoAsync();
                // 判断是否获取成功
                if (response.IsSuccess)
                {
                    // 先清除集合内所有数据
                    this.SelectedMemoDataCollection.Clear();
                    // 将响应里的Data拿出来，存入MemoDataModels集合内
                    foreach (MemoDto memoDto in response.Data)
                    {
                        this.SelectedMemoDataCollection.Add(memoDto);
                    }
                }
                else
                {
                    // 响应体为失败
                    Debug.WriteLine(response);
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"加载备忘录异常：{ex.Message}");
                // 这里抛出的异常可以让上级继续处理
                throw;
            }
            finally
            {
                // 隐藏加载动画
                this.HideLoading();
            }
        }

        // 添加待办事项方法
        private void AddMemoData()
        {
            // 先判断AddOrUpdateMemoData是否实例化了,如果是空的话就实例化一次
            if(this._addOrUpdateMemoData == null) { this.AddOrUpdateMemoData = new MemoDto(); }
            // 将AddOrUpdateMemoData 属性的值置空
            this.AddOrUpdateMemoData.Id = default(int);
            this.AddOrUpdateMemoData.Title = default(string);
            this.AddOrUpdateMemoData.Content = default(string);
            // 打开右侧添加/修改的栏位
            this.IsRightDrawerOpen = true;
        }

        // 带条件查询备忘录方法
        private async void GetMemoDataByCondition()
        {
            // 显示加载动画
            this.ShowLoading("正在查询备忘录...", "MemoViewModel");

            // 确保资源清理或状态恢复的代码绝对执行，防止程序状态“卡死”。
            // 在加载动画这个场景下，finally 的作用就是：无论成功还是失败，都必须关闭加载动画。
            try
            {
                // ===== 执行业务逻辑 =====
                // 根据条件查询对应的备忘录数据
                ApiResponse<List<MemoDto>> response = await this._memoApiService.GetMemoByConditionAsync(this._searchText);
                // 判断是否获取成功
                if (response.IsSuccess)
                {
                    // 先清除集合内所有数据
                    this.SelectedMemoDataCollection.Clear();
                    // 将响应里的Data拿出来，存入 ToDoDataModels 集合内
                    foreach (MemoDto memoDto in response.Data)
                    {
                        this.SelectedMemoDataCollection.Add(memoDto);
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
                Debug.WriteLine($"查询备忘录异常：{ex.Message}");
            }
            finally
            {
                // 隐藏加载动画
                this.HideLoading();
            }
        }

        // 添加被选中的备忘录数据方法（修改方法）
        private void SelectedMemoData(MemoDto memoDto)
        {
            // 先判断 AddOrUpdateMemoData 是否实例化了,如果是空的话就实例化一次
            if (this._addOrUpdateMemoData == null) { this.AddOrUpdateMemoData = new MemoDto(); }
            // 将 AddOrUpdateMemoData 属性的值设置成选中的Memo数据
            this.AddOrUpdateMemoData.Id = memoDto.Id;
            this.AddOrUpdateMemoData.Title = memoDto.Title;
            this.AddOrUpdateMemoData.Content = memoDto.Content;
            // 打开右侧添加/修改的栏位
            this.IsRightDrawerOpen = true;
        }

        // 添加/修改备忘录后的保存方法
        private async void SaveMemoData()
        {
            try
            {
                // 先判断备忘录标题、内容、状态是否为空，为空直接结束方法
                if (string.IsNullOrWhiteSpace(this._addOrUpdateMemoData.Title) || string.IsNullOrWhiteSpace(this._addOrUpdateMemoData.Content))
                {
                    return;
                }
                // 不为空的话继续，先显示加载动画
                this.ShowLoading("刷新备忘录...", "MemoViewModel");

                // 用ID区分添加和修改
                if (this._addOrUpdateMemoData.Id > 0)
                {
                    // 修改业务逻辑
                    // 通过服务层请求修改指定备忘录数据
                    ApiResponse<MemoDto> response = await this._memoApiService.UpdateMemoAsync(MemoDtoConverter.ConvertMemoDtoToUpdateMemoDto(this._addOrUpdateMemoData));
                    // 修改成功
                    if (response.IsSuccess)
                    {
                        // 更新前端的状态
                        MemoDto? memo = this.SelectedMemoDataCollection.FirstOrDefault((MemoDto memoDto) => memoDto.Id == this._addOrUpdateMemoData.Id);
                        // 如果找到的话
                        if (memo != null) { memo.Title = this._addOrUpdateMemoData.Title; memo.Content = this._addOrUpdateMemoData.Content; }
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
                    // 通过服务层请求添加备忘录数据
                    ApiResponse<MemoDto> response = await this._memoApiService.AddMemoAsync(MemoDtoConverter.ConvertMemoDtoToAddMemoDto(this._addOrUpdateMemoData));
                    if (response.IsSuccess)
                    {
                        // 将添加成功的待办事项放到集合中
                        this.SelectedMemoDataCollection.Add(response.Data);
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
                Debug.WriteLine($"添加/修改备忘录异常：{ex.Message}");
            }
            finally
            {
                this.HideLoading();
            }
        }

        private async void DeleteMemoData(MemoDto memo)
        {
            try
            {
                // 先弹出个删除提示对话框
                IDialogResult dialogResult = await this._dialogHostService.ShowMsgDialog("温馨提示", $"确认删除备忘录\"{memo.Title}\"?");
                // 如果返回的是取消，就直接return，结束当前方法
                if (dialogResult.Result != ButtonResult.OK) { return; }

                // 先判断备忘录是否为空
                if (memo == null) { return; }
                // 不为空的话继续，先显示加载动画
                this.ShowLoading("删除备忘录...", "MemoViewModel");

                // 调用删除方法
                ApiResponse<int> response = await this._memoApiService.DeleteMemoAsync(memo.Id);

                // 判断响应体是否成功（删除是否成功）
                if (response.IsSuccess)
                {
                    // 从集合中获取删除到删除的备忘录MemoDto
                    MemoDto? deleteMemoDto = this.SelectedMemoDataCollection.FirstOrDefault((MemoDto memoDto) => memoDto.Id == memo.Id);
                    // 判断是否找到了被删除的备忘录MemoDto
                    if (deleteMemoDto != null) { this.SelectedMemoDataCollection.Remove(deleteMemoDto); }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"删除备忘录异常：{ex.Message}");
            }
            finally
            {
                this.HideLoading();
            }
        }

        // 重写父类OnNavigatedTo方法，用于导航到此ViewModel中时需要什么操作
        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            // 调用父类方法（如果父类有操作）
            base.OnNavigatedTo(navigationContext);

            // 自己写的操作，需要做什么事儿
            // 获取全部备忘录
            await this.GetAllMemo();
        }
    }
}
