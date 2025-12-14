using System.ComponentModel.DataAnnotations;

namespace MemoApi.Dtos.Memo
{
    /// <summary>
    /// 创建备忘录数据传输对象
    /// 此类用于接收客户端创建新待办事项时提交的数据
    /// </summary>
    public class CreateMemoDto
    {
        /// <summary>
        /// 备忘录标题（必填）
        /// </summary>
        [Required(ErrorMessage = "标题是必填字段，不能为空。")]
        [MaxLength(100, ErrorMessage = "标题长度不能超过100个字符。")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 备忘录详细内容（可选）
        /// </summary>
        [MaxLength(1000, ErrorMessage = "内容长度不能超过1000个字符。")]
        public string Content { get; set; } = string.Empty;

        // 注意：不包含 Id、CreateDate、UpdateDate 字段
        // 因为这些字段应由服务器或数据库生成，不应由客户端提供
    }
}
