using System.ComponentModel.DataAnnotations;

namespace MemoDesktop.Dtos.ToDo
{
    /// <summary>
    /// 创建待办事项数据传输对象
    /// 此类用于接收客户端创建新待办事项时提交的数据
    /// </summary>
    public class CreateToDoDto
    {
        /// <summary>
        /// 待办事项标题（必填）
        /// </summary>
        [Required(ErrorMessage = "标题是必填字段，不能为空。")]
        [MaxLength(100, ErrorMessage = "标题长度不能超过100个字符。")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 待办事项详细内容（可选）
        /// </summary>
        [MaxLength(1000, ErrorMessage = "内容长度不能超过1000个字符。")]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// 待办事项初始状态
        /// 默认值为0，表示"待办"状态
        /// </summary>
        [Range(0, 2, ErrorMessage = "状态值必须为0（待办）、1（进行中）或2（已完成）。")]
        public int Status { get; set; } = 0;

        // 注意：不包含 Id、CreateDate、UpdateDate 字段
        // 因为这些字段应由服务器或数据库生成，不应由客户端提供
    }
}
