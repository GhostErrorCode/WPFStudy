using MemoApi.Data;
using MemoApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace MemoApi.Repositories.Implements
{
    public class ToDoRepository : Repository<ToDo>, IToDoRepository
    {
        // 构造函数
        // 调用基类构造函数：base(context)将context传递给基类
        public ToDoRepository(MemoApplicationDbContext context) : base(context)
        {
            // 可以在这里初始化产品仓储特有的依赖项
            // 例如：缓存服务、日志服务等
        }
    }
}
