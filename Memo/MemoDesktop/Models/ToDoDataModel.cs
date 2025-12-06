using System;
using System.Collections.Generic;
using System.Text;

namespace MemoDesktop.Models
{
    /// <summary>
    /// 待办事项实体Model
    /// </summary>
    public class ToDoDataModel
    {
        // id
        private int _id;
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        // 标题
        private string _title;
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        // 内容
        private string _content;
        public string Content
        {
            get { return _content; }
            set { _content = value; }
        }

        // 状态
        private int _status;
        public int Status
        {
            get { return _status; }
            set { _status = value; }
        }

        // 创建时间
        private DateTime _createDate;
        public DateTime CreateDate
        {
            get { return _createDate; }
            set { _createDate = value; }
        }

        // 修改时间
        private DateTime _updateDate;
        public DateTime UpdateDate
        {
            get { return _updateDate; }
            set { _updateDate = value; }
        }
    }
}
