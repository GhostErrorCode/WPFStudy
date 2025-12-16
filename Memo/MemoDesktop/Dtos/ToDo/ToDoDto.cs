namespace MemoDesktop.Dtos.ToDo
{
    /// <summary>
    /// 待办事项数据传输对象（用于查询返回）
    /// 此类定义了API接口返回给客户端的数据格式
    /// </summary>
    public class ToDoDto
    {
        /// <summary>
        /// 待办事项的唯一标识符
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 待办事项的标题
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 待办事项的详细内容
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// 待办事项的状态（转换为用户可读的文本）
        /// 0-待办，1-进行中，2-已完成
        /// </summary>
        public string StatusText { get; set; } = string.Empty;

        /// <summary>
        /// 待办事项的创建时间（格式化后的字符串）
        /// </summary>
        public string CreateDateFormatted { get; set; } = string.Empty;

        /// <summary>
        /// 待办事项的最后修改时间（格式化后的字符串）
        /// </summary>
        public string UpdateDateFormatted { get; set; } = string.Empty;

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
