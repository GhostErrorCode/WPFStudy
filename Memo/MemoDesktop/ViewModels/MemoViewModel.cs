using MemoDesktop.ApiResponses;
using MemoDesktop.Dtos.Memo;
using MemoDesktop.Models;
using MemoDesktop.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MemoDesktop.ViewModels
{
    public class MemoViewModel : BindableBase
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


        // 构造函数
        public MemoViewModel(IMemoApiService memoApiService)
        {
            // 获取备忘录Api服务
            this._memoApiService = memoApiService;
            this.GetAllMemo();

            // 绑定添加待办事项Command方法
            this.AddMemoDataCommand = new DelegateCommand(AddMemoData);
        }

        private async void GetAllMemo()
        {
            this.MemoDataModels = new ObservableCollection<MemoDto>();
            ApiResponse<List<MemoDto>> response = await this._memoApiService.GetAllMemoAsync();
            if (response.IsSuccess)
            {
                this.MemoDataModels.Clear();
                foreach(MemoDto memoDto in response.Data)
                {
                    this.MemoDataModels.Add(memoDto);
                }
            }
        }

        // 添加待办事项方法
        private void AddMemoData()
        {
            IsRightDrawerOpen = true;
        }
    }
}
