using System;
using System.Collections.Generic;
using System.Text;
using WpfUiTest.Shared.Base;
using WpfUiTest.Shared.Enums;
using WpfUiTest.Shared.Utilities;

namespace WpfUiTest.Modules.ToDo.ViewModels
{
    // 首页待办事项列表项ViewModel
    public class ToDoItemViewModel : BaseViewModel
    {
        // 待办事项ID
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

        // 待办事项标题
        private string _title = string.Empty;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        // 待办事项内容
        private string _content = string.Empty;
        public string Content
        {
            get { return _content; }
            set { SetProperty(ref _content, value); }
        }

        // 待办事项状态
        private TodoStatusEnum _status = TodoStatusEnum.Pending;
        public TodoStatusEnum Status
        {
            get { return _status; }
            set { SetProperty(ref _status, value); }
        }

        // 待办事项创建时间
        private DateTime _createDate = DateTimeUtility.NowNoMilliseconds();
        public DateTime CreateDate
        {
            get { return _createDate; }
            set { SetProperty(ref _createDate, value); }
        }

        // 待办事项修改时间
        private DateTime _updateDate = DateTimeUtility.NowNoMilliseconds();
        public DateTime UpdateDate
        {
            get { return _updateDate; }
            set { SetProperty(ref _updateDate, value); }
        }
    }
}
