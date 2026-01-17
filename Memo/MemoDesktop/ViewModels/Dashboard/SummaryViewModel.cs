using MemoDesktop.Dtos.Dashboard;
using MemoDesktop.Dtos.Memo;
using MemoDesktop.Dtos.ToDo;
using MemoDesktop.Events;
using MemoDesktop.Extensions;
using MemoDesktop.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Navigation;

namespace MemoDesktop.ViewModels.Dashboard
{
    // 只处理汇总数据的ViewModel
    public class SummaryViewModel : BindableBase
    {
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

        // =================== Defined Fields and Properties Start ==========================================================================
        // 4个统计数据
        private int _toDoTotalCount;
        public int ToDoTotalCount
        {
            get { return _toDoTotalCount; }
            set { _toDoTotalCount = value; RaisePropertyChanged(); }
        }

        private int _toDoCompletedCount;
        public int ToDoCompletedCount
        {
            get { return _toDoCompletedCount; }
            set { _toDoCompletedCount = value; RaisePropertyChanged(); }
        }

        private string _toDoCompletedProportion;
        public string ToDoCompletedProportion
        {
            get { return _toDoCompletedProportion; }
            set { _toDoCompletedProportion = value; RaisePropertyChanged(); }
        }

        private int _memoTotalCount;
        public int MemoTotalCount
        {
            get { return _memoTotalCount; }
            set { _memoTotalCount = value; RaisePropertyChanged(); }
        }

        // 2个列表
        private ObservableCollection<ToDoDto> _toDoList;
        public ObservableCollection<ToDoDto> ToDoList
        {
            get { return _toDoList; }
            set { _toDoList = value; RaisePropertyChanged(); }
        }

        private ObservableCollection<MemoDto> _memoList;
        public ObservableCollection<MemoDto> MemoList
        {
            get { return _memoList; }
            set { _memoList = value; RaisePropertyChanged(); }
        }
        // =================== Defined Fields and Properties End ==========================================================================

        // 构造函数
        public SummaryViewModel(IEventAggregator eventAggregator)
        {
            this._eventAggregator = eventAggregator;
        }

        // 用来将Dto转为ViewModel
        public void LoadFromDto(SummaryDto dto)
        {
            ToDoTotalCount = dto.ToDoTotalCount;
            ToDoCompletedCount = dto.ToDoCompletedCount;
            ToDoCompletedProportion = dto.ToDoCompletedProportion;
            MemoTotalCount = dto.MemoTotalCount;

            ToDoList = new ObservableCollection<ToDoDto>(dto.ToDoList);
            MemoList = new ObservableCollection<MemoDto>(dto.MemoList);

            // 调用发布汇总数据更新的方法
            this.PublishSummaryChanged();
        }

        // 添加待办事项方法
        public void AddToDo(ToDoDto toDoDto)
        {
            // 先判断待办事项是否为待办，待办才允许添加到首页列表中
            if(toDoDto.Status == 0)
            {
                // 添加的待办事项为待办状态
                // 把新待办事项添加列表中
                this.ToDoList.Add(toDoDto);
                // 更新统计结果
                this.ToDoTotalCount = this.ToDoTotalCount + 1;
            }
            else
            {
                // 添加的待办事项为已完成状态
                this.ToDoTotalCount = this.ToDoTotalCount + 1;
                this.ToDoCompletedCount = this.ToDoCompletedCount + 1;
            }
            // 计算待办事项完成率
            this.CalculateToDoCompletedProportion();
            // 调用发布汇总数据更新的方法
            this.PublishSummaryChanged();
        }

        // 添加备忘录方法
        public void AddMemo(MemoDto memoDto)
        {
            // 把新备忘录添加列表中
            this.MemoList.Add(memoDto);
            this.MemoTotalCount = this.MemoTotalCount + 1;
            // 调用发布汇总数据更新的方法
            this.PublishSummaryChanged();
        }

        // 修改待办事项方法
        public void EditToDo(ToDoDto toDoDto)
        {
            // 先判断待办事项是否为待办
            if (toDoDto.Status == 0)
            {
                // 如果修改后状态仍为待办，就修改对应的待办事项
                ToDoDto? updateToDoDto = this.ToDoList.FirstOrDefault((ToDoDto t) => t.Id == toDoDto.Id);
                // 如果找到了的话就更新
                if(updateToDoDto != null)
                {
                    updateToDoDto.Title = toDoDto.Title;
                    updateToDoDto.Content = toDoDto.Content;
                    updateToDoDto.Status = toDoDto.Status;  // 严格来说这句可以不写，但还是写上吧
                }
                // 更新统计结果
                // ...
            }
            else
            {
                // 如果直接把待办事项修改成已完成了，那就直接从前端剔除它
                this.ToDoList.Remove(this.ToDoList.FirstOrDefault((ToDoDto t) => t.Id == toDoDto.Id));
                // 更新统计结果
                this.ToDoCompletedCount = this.ToDoCompletedCount + 1;
            }
            // 计算待办事项完成率
            this.CalculateToDoCompletedProportion();
            // 调用发布汇总数据更新的方法
            this.PublishSummaryChanged();
        }

        // 修改备忘录方法
        public void EditMemo(MemoDto memoDto)
        {
            // 从列表中找出来对应的备忘录
            MemoDto? updateMemoDto = this.MemoList.FirstOrDefault((MemoDto m) => m.Id == memoDto.Id);
            // 如果找到了的话就更新
            if (updateMemoDto != null)
            {
                updateMemoDto.Title = memoDto.Title;
                updateMemoDto.Content = memoDto.Content;
            }
            // 调用发布汇总数据更新的方法
            this.PublishSummaryChanged();
        }




        // 发布首页汇总数据更新事件(表达式体方法)
        private void PublishSummaryChanged() => this._eventAggregator.SummaryChangedEventPublish(new SummaryChangedEventArgs()
        {
            ToDoTotalCount = this._toDoTotalCount,
            ToDoCompletedCount = this._toDoCompletedCount,
            ToDoCompletedProportion = this._toDoCompletedProportion,
            MemoTotalCount = this._memoTotalCount
        });

        // 计算待办事项完成率(表达式体方法)
        private string CalculateToDoCompletedProportion() => this.ToDoCompletedProportion = this.ToDoTotalCount == 0 ? "0%" : $"{(double)this.ToDoCompletedCount / this.ToDoTotalCount * 100:F2}%";
    }
}
