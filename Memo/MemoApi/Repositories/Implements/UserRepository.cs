// 引入命名空间
using MemoApi.Data;
using MemoApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace MemoApi.Repositories.Implements
{
    // 产品仓储实现类
    // 继承：Repository<TEntity> - 复用通用仓储的基础CRUD实现
    // 实现：IUserRepository - 实现产品特有的业务方法
    // 设计模式：具体仓储模式，封装特定实体的复杂数据访问逻辑
    public class UserRepository : Repository<User>, IUserRepository
    {
        // 构造函数
        // 调用基类构造函数：base(context)将context传递给基类
        public UserRepository(MemoApplicationDbContext context) : base(context)
        {
            // 可以在这里初始化产品仓储特有的依赖项
            // 例如：缓存服务、日志服务等
        }
    }
}
