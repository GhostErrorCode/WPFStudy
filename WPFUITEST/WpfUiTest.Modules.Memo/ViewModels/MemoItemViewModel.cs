using System;
using System.Collections.Generic;
using System.Text;
using WpfUiTest.Shared.Base;
using WpfUiTest.Shared.Utilities;

namespace WpfUiTest.Modules.Memo.ViewModels
{
    // 备忘录列表项ViewModel
    public class MemoItemViewModel : BaseViewModel
    {
        // 备忘录ID
        private int _id = 0;
        public int Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        // 用户ID
        private int _userId = 0;
        public int UserId
        {
            get { return _userId; }
            set { SetProperty(ref _userId, value); }
        }

        // 备忘录标题
        private string _title = string.Empty;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        // 备忘录内容
        private string _content = string.Empty;
        public string Content
        {
            get { return _content; }
            set { SetProperty(ref _content, value); }
        }

        // 备忘录创建时间
        private DateTime _createDate = DateTimeUtility.NowNoMilliseconds();
        public DateTime CreateDate
        {
            get { return _createDate; }
            set { SetProperty(ref _createDate, value); }
        }

        // 备忘录修改时间
        private DateTime _updateDate = DateTimeUtility.NowNoMilliseconds();
        public DateTime UpdateDate
        {
            get { return _updateDate; }
            set { SetProperty(ref _updateDate, value); }
        }
    }
}
