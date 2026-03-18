using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using WpfUiTest.Core.Data.Entities;
using WpfUiTest.Shared.Utilities;

namespace WpfUiTest.Core.Data.Repositories.Interfaces
{
    // 待办事项具体仓储接口
    // 作用：继承通用仓储接口，添加待办事项实体特有的业务数据访问方法
    // 继承关系：IToDoRepository : IRepository<ToDo>
    // 设计原则：接口隔离原则 - 为特定客户端（ToDoService）提供特定接口
    // 优势：1. 方法名具有业务语义 2. 编译时类型安全 3. 易于测试和Mock
    public interface IToDoRepository : IRepository<ToDo>
    {
        // 注意：这里不需要重新声明GetByIdAsync、AddAsync等基础方法
        // 它们已从父接口IRepository<User>继承


        // ========== 产品特有的业务查询方法 ==========
        // 这些方法封装了产品特有的复杂查询逻辑，使业务层代码更简洁

        // 分页多条件查询
        public Task<PagedResult<ToDo>> FindPagedToDosAsync(
            Expression<Func<ToDo, bool>> predicate,
            Func<IQueryable<ToDo>,IOrderedQueryable<ToDo>> orderBy,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default
            );

    }

}
