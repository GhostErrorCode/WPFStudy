namespace MemoApi.Entities
{
    // 备忘录实体类
    public class Memo
    {
        // 备忘录ID
        public int Id { get; set; }

        // 备忘录标题
        public string? Title { get; set; }

        // 备忘录内容
        public string? Content { get; set; }

        // 备忘录创建时间
        public DateTime CreateDate { get; set; }

        // 备忘录修改时间
        public DateTime UpdateDate { get; set; }
    }
}
