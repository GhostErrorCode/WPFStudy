using MemoDesktop.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MemoDesktop.ViewModels
{
    public class MemoViewModel : BindableBase
    {
        // 字段和属性
        private ObservableCollection<MemoDataModel> _memoDataModels;
        public ObservableCollection<MemoDataModel> MemoDataModels
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



        // 构造函数
        public MemoViewModel()
        {
            // 创建待办事项的测试数据
            this.CreateMemoDataModels();

            // 绑定添加待办事项Command方法
            this.AddMemoDataCommand = new DelegateCommand(AddMemoData);
        }

        private void CreateMemoDataModels()
        {
            // 初始化待办事项属性
            this.MemoDataModels = new ObservableCollection<MemoDataModel>();
            // 循环添加数据
            for (int i = 0; i < 20; i++)
            {
                this.MemoDataModels.Add(new MemoDataModel() { Id = i, Title = "备忘录标题" + i, Content = "备忘录内容:" + i, Status = 0, CreateDate = DateTime.Now, UpdateDate = DateTime.Now });
            }
        }

        // 添加待办事项方法
        private void AddMemoData()
        {
            IsRightDrawerOpen = true;
        }
    }
}
