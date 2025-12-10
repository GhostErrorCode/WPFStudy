namespace MemoApi.Entities
{
    // 待办事项实体类
    public class ToDo
    {
        // 待办事项ID
        public int Id { get; set; }

        // 待办事项标题
        public string? Title { get; set; }

        // 待办事项内容
        public string? Content { get; set; }

        // 待办事项状态
        public int Status { get; set; }

        // 待办事项创建时间
        public DateTime CreateDate {  get; set; }

        // 待办事项修改时间
        public DateTime UpdateDate { get; set; }

    }
}
