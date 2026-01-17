using MemoDesktop.Dtos.Memo;
using MemoDesktop.Dtos.ToDo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MemoDesktop.Dtos.Dashboard
{
    /// <summary>
    /// 创建面板汇总的数据传输对象
    /// 此类用于传输前端面板汇总数据，包含待办事项总数，待办事项完成数，待办事项完成比例，备忘录总数
    /// </summary>
    /// 

    // 用点奇妙的小工具Record(C#9+加入的)
    public record SummaryDto(
        int ToDoTotalCount,
        int ToDoCompletedCount,
        string ToDoCompletedProportion,
        int MemoTotalCount,
        List<ToDoDto> ToDoList,
        List<MemoDto> MemoList
        );


    /*
    public class SummaryDto : BaseDto
    {
        // 待办事项总数
        private int _toDoTotalCount = 0;
        public int ToDoTotalCount
        {
            get { return _toDoTotalCount; }
            set { _toDoTotalCount = value; OnPropertyChanged(); }
        }

        // 待办事项完成数量
        private int _toDoCompletedCount = 0;
        public int ToDoCompletedCount
        {
            get { return _toDoCompletedCount; }
            set { _toDoCompletedCount = value; OnPropertyChanged(); }
        }

        // 待办事项完成比例
        private string _toDoCompletedProportion = string.Empty;
        public string ToDoCompletedProportion
        {
            get { return _toDoCompletedProportion; }
            set { _toDoCompletedProportion = value; OnPropertyChanged(); }
        }

        // 备忘录总数
        private int _memoTotalCount = 0;
        public int MemoTotalCount
        {
            get { return _memoTotalCount; }
            set { _memoTotalCount = value; OnPropertyChanged(); }
        }

        // 待办事项列表（仅待办状态）
        private ObservableCollection<ToDoDto> _toDoList = new ObservableCollection<ToDoDto>();
        public ObservableCollection<ToDoDto> ToDoList
        {
            get { return _toDoList; }
            set { _toDoList = value; }
        }

        // 备忘录列表
        private ObservableCollection<MemoDto> _memoList = new ObservableCollection<MemoDto>();
        public ObservableCollection<MemoDto> MemoList
        {
            get { return _memoList; }
            set { _memoList = value; }
        }
    }
    */
}
