using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using WpfUiTest.Core.Data.DbContexts;
using WpfUiTest.Core.Data.Entities;
using WpfUiTest.Core.Data.Repositories.Interfaces;
using WpfUiTest.Core.Services.Interfaces;
using WpfUiTest.Shared.Utilities;

namespace WpfUiTest.Core.Data.Repositories.Implements
{
    public class ToDoRepository : Repository<ToDo>, IToDoRepository
    {
        // 构造函数
        // 调用基类构造函数：base(context)将context传递给基类
        public ToDoRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            // 可以在这里初始化产品仓储特有的依赖项
            // 例如：缓存服务、日志服务等
        }

        // 分页多条件查询
        public async Task<PagedResult<ToDo>> FindPagedToDosAsync(Expression<Func<ToDo, bool>> predicate, Func<IQueryable<ToDo>, IOrderedQueryable<ToDo>> orderBy, int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            // 构建基础查询器
            IQueryable<ToDo> query = this._dbSet.Where(predicate);

            // 统计所有符合条件的总数量
            int total = await query.CountAsync();

            // 添加排序条件
            IOrderedQueryable<ToDo> orderedQuery = orderBy?.Invoke(query) ?? throw new InvalidOperationException("分页查询必须提供排序规则");

            // 添加分页并进行查询
            List<ToDo> items = await orderedQuery
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            // 返回查到的结果
            return new PagedResult<ToDo>()
            {
                TotalCount = total,
                Items = items
            };
        }
    }

}
