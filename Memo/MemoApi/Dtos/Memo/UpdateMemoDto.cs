using System.ComponentModel.DataAnnotations;

namespace MemoApi.Dtos.Memo
{
    /// <summary>
    /// 更新备忘录数据传输对象
    /// 此类用于接收客户端更新现有备忘录时提交的数据
    /// </summary>
    public class UpdateMemoDto
    {
        /// <summary>
        /// 要更新的备忘录的唯一标识符（必填）
        /// </summary>
        [Required(ErrorMessage = "ID是必填字段，不能为空。")]
        public int Id { get; set; }

        /// <summary>
        /// 更新后的备忘录标题（必填）
        /// </summary>
        [Required(ErrorMessage = "标题是必填字段，不能为空。")]
        [MaxLength(100, ErrorMessage = "标题长度不能超过100个字符。")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 更新后的备忘录详细内容（可选）
        /// </summary>
        [MaxLength(1000, ErrorMessage = "内容长度不能超过1000个字符。")]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// 备忘录的最后修改时间
        /// 此字段由客户端提供，服务器应验证其合理性
        /// </summary>
        public DateTime UpdateDate { get; set; } = DateTime.Now;

        // 注意：不包含 CreateDate 字段
        // 因为创建时间在实体生命周期内不应被修改
    }
}
