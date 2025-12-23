using MemoDesktop.ApiResponses;
using MemoDesktop.Dtos.Memo;
using MemoDesktop.Extensions;
using MemoDesktop.Models;
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
        private ObservableCollection<MemoDto> _memoDataModels;
        public ObservableCollection<MemoDto> MemoDataModels
        {
            get { return _memoDataModels; }
            set { _memoDataModels = value; RaisePropertyChanged(); }
        }

        // 添加待办事项Command
        public DelegateCommand AddMemoDataCommand { get; private set; }
        // 右侧添加待办事项是否展开
        private bool _isRightDrawerOpen;
        public bool IsRightDrawerOpen
        {
            get { return _isRightDrawerOpen; }
            set { _isRightDrawerOpen = value; RaisePropertyChanged(); }
        }

        // 备忘录Api相关服务Service
        private readonly IMemoApiService _memoApiService;
        // 事件聚合器
        private readonly IEventAggregator _eventAggregator;

        // 构造函数
        public MemoViewModel(IMemoApiService memoApiService, IEventAggregator eventAggregator) : base(eventAggregator)
        {
            // 获取备忘录Api服务
            this._memoApiService = memoApiService;
            // 获取全局事件聚合器
            this._eventAggregator = eventAggregator;
            // 绑定添加待办事项Command方法
            this.AddMemoDataCommand = new DelegateCommand(AddMemoData);


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
                this.MemoDataModels = new ObservableCollection<MemoDto>();
                // 通过服务层请求获取全部的备忘录数据
                ApiResponse<List<MemoDto>> response = await this._memoApiService.GetAllMemoAsync();
                // 判断是否获取成功
                if (response.IsSuccess)
                {
                    // 先清除集合内所有数据
                    this.MemoDataModels.Clear();
                    // 将响应里的Data拿出来，存入MemoDataModels集合内
                    foreach (MemoDto memoDto in response.Data)
                    {
                        this.MemoDataModels.Add(memoDto);
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
            IsRightDrawerOpen = true;
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
