// 引入命名空间
using MemoApi.Data;
using MemoApi.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

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

        // <summary>
        /// 实现：根据账户名查找用户
        /// </summary>
        public async Task<User?> GetByAccountAsync(string account)
        {
            // 关键：使用父类保护的 _dbSet（即 DbSet<User>）进行查询
            // 使用 SingleOrDefaultAsync 确保账户名唯一，并提高查询效率
            User? userEntity = await _dbSet
                .AsNoTracking()  // 如果只用于登录验证，不需要变更跟踪，可提升性能
                .SingleOrDefaultAsync((User user) => user.Account == account);
            return userEntity;
        }

        /// <summary>
        /// 实现：检查账户名是否存在
        /// </summary>
        public async Task<bool> AccountExistsAsync(string account)
        {
            // 使用 AnyAsync 比 CountAsync > 0 或 FirstOrDefaultAsync != null 性能更优
            return await _dbSet
                .AnyAsync((User user) => user.Account == account);
        }

        // 注意：通用的 AddAsync, GetByIdAsync 等方法已通过父类 Repository<User> 实现
        // 如果需要覆盖或增强某个通用方法，可以在这里重写（override）
    }
}
