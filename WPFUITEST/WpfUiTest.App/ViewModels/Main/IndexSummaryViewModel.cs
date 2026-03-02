using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using WpfUiTest.App.ViewModels.Enums;
using WpfUiTest.Shared.Base;

namespace WpfUiTest.App.ViewModels.Main
{
    // 首页汇总ViewModel
    public class IndexSummaryViewModel : BaseViewModel
    {
        // ==================== 字段、属性、命令 ====================
        // 属性：首页汇总
        private ObservableCollection<IndexSummaryItemViewModel> _indexSummaryItems;
        public ObservableCollection<IndexSummaryItemViewModel> IndexSummaryItems
        {
            get { return _indexSummaryItems; }
            set { SetProperty(ref _indexSummaryItems, value); }
        }


        // ==================== 构造函数 ====================
        public IndexSummaryViewModel()
        {
            // 初始化字段
            // 初始化属性
            this._indexSummaryItems = new ObservableCollection<IndexSummaryItemViewModel>()
            {
                new IndexSummaryItemViewModel(){ Title="待办汇总", Content="999999", Icon="ClipboardTaskListRtl24", SummaryType=IndexSummaryType.ToDoTotal },
                new IndexSummaryItemViewModel(){ Title="已完成", Content="999999", Icon="ClipboardTask24", SummaryType=IndexSummaryType.Completed },
                new IndexSummaryItemViewModel(){ Title="完成率", Content="100%", Icon="DocumentPercent24", SummaryType=IndexSummaryType.CompletionRate },
                new IndexSummaryItemViewModel(){ Title="备忘录", Content="999999", Icon="ClipboardTextLtr24", SummaryType=IndexSummaryType.MemoTotal },
                new IndexSummaryItemViewModel(){ Title="NULL", Content="NULL", Icon="Add24", SummaryType=IndexSummaryType.Custom }
            };

            // 初始化命令
        }

        // ==================== 方法 ====================
    }
}
