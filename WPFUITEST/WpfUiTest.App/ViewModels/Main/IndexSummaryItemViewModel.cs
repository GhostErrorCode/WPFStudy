using System;
using System.Collections.Generic;
using System.Text;
using WpfUiTest.App.ViewModels.Enums;
using WpfUiTest.Shared.Base;

namespace WpfUiTest.App.ViewModels.Main
{
    // 首页汇总区域项
    public class IndexSummaryItemViewModel : BaseViewModel
    {
        // ==================== 字段、属性、命令 ====================
        // 属性：汇总项名称
        private string _title = string.Empty;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref  _title, value); }
        }
        // 属性：汇总项内容
        private string _content = string.Empty;
        public string Content
        {
            get { return _content; }
            set { SetProperty(ref _content, value); }
        }
        // 属性：汇总项图标
        private string _icon = string.Empty;
        public string Icon
        {
            get { return _icon; }
            set { SetProperty(ref _icon, value); }
        }
        // 属性：汇总项标识
        private IndexSummaryType _summaryType = IndexSummaryType.Custom;
        public IndexSummaryType SummaryType
        {
            get { return _summaryType; }
            set { SetProperty(ref _summaryType, value); }
        }
    }
}
