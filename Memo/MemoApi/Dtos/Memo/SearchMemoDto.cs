using System.ComponentModel.DataAnnotations;

namespace MemoApi.Dtos.Memo
{
    /// <summary>
    /// 创建备忘录数据传输对象
    /// 此类用于接收客户端条件查询时提交的数据
    /// </summary>
    public class SearchMemoDto
    {
        /// <summary>
        /// 备忘录标题（必填）
        /// </summary>
        public string Title { get; set; } = string.Empty;
    }
}
