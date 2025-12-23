using System.ComponentModel.DataAnnotations;

namespace MemoApi.Dtos.ToDo
{
    /// <summary>
    /// 创建待办事项数据传输对象
    /// 此类用于接收客户端查询时的条件
    /// </summary>
    public class SearchToDoDto
    {
        /// <summary>
        /// 待办事项标题（选填）
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 待办事项初始状态
        /// 默认值为0，表示"待办"状态
        /// </summary>
        [Range(0, 2, ErrorMessage = "状态值必须为0（待办）、1（进行中）或2（已完成）。")]
        public int Status { get; set; } = 0;
    }
}
