// 引入命名空间
// Microsoft.EntityFrameworkCore：Entity Framework Core核心功能
// System.Linq.Expressions：表达式树，用于构建LINQ查询
// System.Reflection：反射功能，用于动态检查实体属性
using MemoApi.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace MemoApi.Repositories.Implements
{

    // 通用仓储基类
    // 作用：提供IRepository<TEntity>接口的默认实现，减少具体仓储类的重复代码
    // 设计模式：模板方法模式，提供骨架实现，允许子类重写特定步骤
    // 泛型约束：where TEntity : class，确保TEntity是引用类型
    // 抽象类：不能被直接实例化，只能作为其他类的基类
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        // 保护字段：数据库上下文
        // 访问修饰符：protected - 允许子类访问此字段
        // 只读：确保只能在构造函数中赋值，之后不能修改
        // 命名约定：下划线前缀表示私有或保护字段
        protected readonly MemoApplicationDbContext _context;

        // 保护字段：对应实体的DbSet
        // 作用：对特定实体类型执行数据库操作
        // DbSet<TEntity>：EF Core中表示数据库表的集合
        protected readonly DbSet<TEntity> _dbSet;


        // 构造函数
        // 访问修饰符：protected - 只能被派生类调用，不能从外部实例化
        // 参数：context - 数据库上下文，由依赖注入提供
        public Repository(MemoApplicationDbContext context)
        {
            // 参数验证：防御性编程，尽早发现错误
            if (context == null)
            {
                // 抛出ArgumentNullException异常，提供清晰的错误信息
                // nameof(context)：编译时获取参数名，避免硬编码字符串
                throw new ArgumentNullException(nameof(context), "数据库上下文不能为空");
            }

            // 赋值给字段
            this._context = context;
            // 通过DbContext的Set方法获取对应实体的DbSet
            // 这是EF Core的标准做法：每个实体类型对应一个DbSet
            this._dbSet = context.Set<TEntity>();
        }


        // 方法：根据主键ID异步查询单个实体（虚方法）
        // 作用：通过唯一标识符获取单个实体对象
        // 虚方法：virtual关键字允许子类重写此方法
        public virtual async Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            // FindAsync：DbSet的方法，根据主键查找实体
            // 参数：new object[] { id } - 主键值数组（支持复合主键）
            // 返回：Task<TEntity?> - 异步任务，可能返回null（如果实体不存在）
            TEntity? entity = await this._dbSet.FindAsync(new object[] { id }, cancellationToken);
            return entity;
        }


        // 方法：查询所有实体（虚方法）
        // 注意：由于没有BaseEntity，无法自动过滤软删除记录
        public virtual async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            // AsNoTracking()：告诉EF Core不要跟踪返回的实体
            // 优点：提高查询性能，减少内存占用
            // 缺点：不能直接更新这些实体（需要重新附加）
            // 适用场景：只读查询，数据展示
            List<TEntity> entities = await this._dbSet.AsNoTracking().ToListAsync(cancellationToken);
            return entities;
        }


        // 方法：根据条件表达式查询实体（虚方法）
        public virtual async Task<List<TEntity>> FindAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            // Where：应用查询条件
            // predicate：Lambda表达式树，如 p => p.Price > 100
            // 表达式树的优势：EF Core可以将其转换为SQL的WHERE子句
            List<TEntity> entities = await this._dbSet
                .Where(predicate)          // 应用查询条件
                .AsNoTracking()            // 不跟踪返回的实体
                .ToListAsync(cancellationToken);  // 异步执行查询并转换为列表

            return entities;
        }


        // 方法：异步添加新实体（虚方法）
        public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            // 参数验证：确保传入的实体不为null
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "要添加的实体不能为空");
            }

            // 注意：由于没有BaseEntity，不能自动设置CreatedAt等字段
            // 需要在具体仓储中重写此方法来设置这些字段

            // AddAsync：将实体添加到DbSet，EF Core开始跟踪此实体
            // 返回：EntityEntry<TEntity>，包含实体的跟踪信息
            Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<TEntity> entry =
                await this._dbSet.AddAsync(entity, cancellationToken);

            // 返回添加的实体（可能包含数据库生成的ID等属性）
            return entry.Entity;
        }


        // 方法：更新实体状态（虚方法）
        public virtual void Update(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "要更新的实体不能为空");
            }

            // 由于没有BaseEntity，不能自动设置UpdatedAt字段
            // 需要在具体仓储中重写此方法

            // 检查实体是否已被上下文跟踪
            // EntityState.Detached：实体未被DbContext跟踪
            if (this._context.Entry(entity).State == EntityState.Detached)
            {
                // 附加实体到DbContext，开始跟踪
                this._dbSet.Attach(entity);
            }

            // 标记实体状态为Modified（已修改）
            // 注意：这会导致所有属性都被标记为修改，可能生成更新所有字段的SQL
            // 如果只想更新部分字段，可以使用更精细的控制：
            // _context.Entry(entity).Property(e => e.Name).IsModified = true;
            this._context.Entry(entity).State = EntityState.Modified;
        }


        // 方法：物理删除实体（虚方法）
        public virtual async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            // 先查找要删除的实体
            TEntity? entity = await GetByIdAsync(id, cancellationToken);

            // 如果实体不存在，返回false表示删除失败
            if (entity == null)
            {
                return false;
            }

            // 从DbSet中移除实体，标记为删除状态
            // 注意：此时实体还未从数据库删除，需要调用SaveChangesAsync
            this._dbSet.Remove(entity);

            return true;
        }


        // 方法：统计实体数量（虚方法）
        public virtual async Task<int> CountAsync(
            Expression<Func<TEntity, bool>>? predicate = null,
            CancellationToken cancellationToken = default)
        {
            // 如果没有提供条件，统计所有实体
            if (predicate == null)
            {
                return await this._dbSet.CountAsync(cancellationToken);
            }
            else
            {
                // 如果有条件，统计满足条件的实体
                return await this._dbSet.CountAsync(predicate, cancellationToken);
            }
        }


        // 方法：检查是否存在满足条件的实体（虚方法）
        public virtual async Task<bool> ExistsAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            // AnyAsync：检查是否存在满足条件的记录
            // 比CountAsync > 0更高效，因为找到第一条匹配记录就返回
            return await this._dbSet.AnyAsync(predicate, cancellationToken);
        }


        // 方法：软删除实体（虚方法）
        public virtual async Task<bool> SoftDeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            // 使用反射检查实体是否有IsDeleted属性
            // GetProperty：通过属性名获取PropertyInfo对象
            PropertyInfo? isDeletedProperty = typeof(TEntity).GetProperty("IsDeleted");

            // 检查：属性存在且类型为bool
            if (isDeletedProperty == null || isDeletedProperty.PropertyType != typeof(bool))
            {
                // 抛出异常：实体没有IsDeleted属性，无法执行软删除
                throw new InvalidOperationException($"实体{typeof(TEntity).Name}没有IsDeleted属性，无法执行软删除");
            }

            // 获取要软删除的实体
            TEntity? entity = await GetByIdAsync(id, cancellationToken);
            if (entity == null)
            {
                return false; // 实体不存在
            }

            // 使用反射设置IsDeleted属性值为true
            // SetValue：设置对象的属性值
            isDeletedProperty.SetValue(entity, true);

            // 尝试设置UpdatedAt属性（如果有）
            PropertyInfo? updatedAtProperty = typeof(TEntity).GetProperty("UpdatedAt");
            // 检查：属性存在且类型为DateTime?（可空的DateTime）
            if (updatedAtProperty != null && updatedAtProperty.PropertyType == typeof(DateTime?))
            {
                // 设置UpdatedAt为当前UTC时间
                updatedAtProperty.SetValue(entity, DateTime.UtcNow);
            }

            // 调用Update方法标记实体为修改状态
            // 注意：这会导致实体被更新，需要调用SaveChangesAsync才能真正保存
            this.Update(entity);

            return true;
        }


        // 方法：恢复软删除的实体（虚方法）
        public virtual async Task<bool> RestoreAsync(int id, CancellationToken cancellationToken = default)
        {
            // 同样检查IsDeleted属性
            PropertyInfo? isDeletedProperty = typeof(TEntity).GetProperty("IsDeleted");
            if (isDeletedProperty == null || isDeletedProperty.PropertyType != typeof(bool))
            {
                throw new InvalidOperationException($"实体{typeof(TEntity).Name}没有IsDeleted属性，无法恢复");
            }

            TEntity? entity = await GetByIdAsync(id, cancellationToken);
            if (entity == null)
            {
                return false;
            }

            // 设置IsDeleted为false，恢复实体
            isDeletedProperty.SetValue(entity, false);

            // 更新实体
            Update(entity);

            return true;
        }
    }
}
