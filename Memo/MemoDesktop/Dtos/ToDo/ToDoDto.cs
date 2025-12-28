using System.ComponentModel;

namespace MemoDesktop.Dtos.ToDo
{
    /// <summary>
    /// 待办事项数据传输对象（用于查询返回）
    /// 此类定义了API接口返回给客户端的数据格式
    /// </summary>
    public class ToDoDto : BaseDto
    {
        /// <summary>
        /// 待办事项的唯一标识符
        /// </summary>
        private int _id;
        public int Id
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 待办事项的标题
        /// </summary>
        private string _title;
        public string Title
        {
            get { return _title; }
            set { _title = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 待办事项的详细内容
        /// </summary>
        private string _content;
        public string Content
        {
            get { return _content; }
            set { _content = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 待办事项的状态（转换为用户可读的文本）
        /// 0-待办，1-已完成
        /// </summary>
        private int _status;
        public int Status
        {
            get { return _status; }
            set { _status = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 待办事项的创建时间（格式化后的字符串）
        /// </summary>
        private string _createDataFormatted;
        public string CreateDataFormatted
        {
            get { return _createDataFormatted; }
            set { _createDataFormatted = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 待办事项的最后修改时间（格式化后的字符串）
        /// </summary>
        private string _updateDataFormatted;
        public string UpdateDateFormatted
        {
            get { return _updateDataFormatted;  }
            set { _updateDataFormatted = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 计算属性：判断该事项是否为24小时内创建的
        /// 此属性在实体类中不存在，是DTO根据业务需求添加的
        /// </summary>
        //public Boolean IsRecentlyCreated
        //{
        //    get
        //    {
        //        DateTime createDate = DateTime.Parse(CreateDateFormatted);
        //        TimeSpan timeSinceCreation = DateTime.Now.Subtract(createDate);
        //        return timeSinceCreation.TotalHours < 24.0;
        //    }
        //}
    }
}
