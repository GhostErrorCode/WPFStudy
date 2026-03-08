using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace WpfUiTest.Core.Data.Repositories.Interfaces
{
    // 通用仓储接口
    // 作用：定义所有仓储都必须实现的基础数据访问操作契约
    // 泛型参数：TEntity - 表示实体类型（如Product、Category等）
    // 泛型约束：where TEntity : class - 限制TEntity必须是引用类型（类、接口等）
    // 设计模式：仓储模式，抽象数据访问逻辑，使业务层不依赖具体数据源
    public interface IRepository<TEntity> where TEntity : class
    {
        // ========== 基础CRUD操作 ==========
        // CRUD：Create（创建）、Read（读取）、Update（更新）、Delete（删除）
        // 这是数据访问层最核心、最常用的操作


        // 方法：根据主键ID异步查询单个实体
        // 作用：通过唯一标识符获取单个实体对象
        // 参数：
        //   - id: int - 要查询的实体主键ID值
        //   - cancellationToken: CancellationToken - 取消令牌，用于取消异步操作
        //     默认值：default - 使用默认的取消令牌
        // 返回值：Task<TEntity?> - 异步任务，可能返回实体或null
        //   问号(?)表示：返回值可能为null（当实体不存在时）
        // 异常：可能抛出数据库异常、网络异常等
        // public Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        // 方法：查询所有实体
        // 作用：获取该类型的所有实体记录
        // 注意：由于没有BaseEntity，无法自动过滤软删除记录
        //       需要在具体实现中手动过滤（如.Where(e => !e.IsDeleted)）
        // 返回值：Task<List<TEntity>> - 异步任务，返回实体列表
        // 为什么返回List而不是IEnumerable：List更常用，支持索引访问，已完全加载
        // public Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

        // 方法：根据条件表达式查询实体
        // 作用：使用Lambda表达式筛选符合条件的实体
        // 参数：
        //   - predicate: Expression<Func<TEntity, bool>> - 查询条件表达式
        //     表达式树：LINQ可以将此表达式转换为SQL的WHERE子句
        //     示例：p => p.Price > 100 && p.StockQuantity > 0
        //   - cancellationToken: CancellationToken - 取消令牌
        // 返回值：Task<List<TEntity>> - 异步任务，返回符合条件的实体列表
        public Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        // 带排序的查询集合
        public Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, CancellationToken cancellationToken = default);

        // 查询单条数据的方法
        public Task<TEntity?> FindSingleAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        // 方法：异步添加新实体
        // 作用：将新实体添加到数据上下文，开始被EF Core跟踪
        // 重要：此方法只将实体添加到内存中的DbContext，未保存到数据库！
        //       必须调用工作单元的SaveChangesAsync()才能真正保存到数据库
        // 参数：
        //   - entity: TEntity - 要添加的实体对象
        //   - cancellationToken: CancellationToken - 取消令牌
        // 返回值：Task<TEntity> - 异步任务，返回添加后的实体
        //   返回的实体可能包含数据库生成的ID等属性
        public Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

        // 方法：更新实体状态
        // 作用：标记实体为已修改状态，实际更新在SaveChangesAsync时执行
        // 参数：
        //   - entity: TEntity - 要更新的实体对象
        // 返回值：void - 无返回值，更新操作本身不产生新数据
        // 重要：此方法只标记状态，必须调用SaveChangesAsync()才能真正更新数据库
        public void Update(TEntity entity);

        // 方法：异步删除实体（物理删除）
        // 作用：从数据库中永久删除实体记录
        // 参数：
        //   - id: int - 要删除的实体主键ID值
        //   - cancellationToken: CancellationToken - 取消令牌
        // 返回值：Task<bool> - 异步任务，返回操作是否成功
        //   true: 删除成功，false: 实体不存在（删除失败）
        // 注意：这是物理删除，数据将永久丢失，考虑使用软删除
        public Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);


        // ========== 统计与检查方法 ==========
        // 这些方法用于数据统计和存在性检查，常用于业务逻辑

        // 方法：异步统计实体数量
        // 作用：统计满足条件的实体数量
        // 参数：
        //   - predicate: Expression<Func<TEntity, bool>>? - 可空的查询条件表达式
        //     问号(?)表示：参数可以为null，表示统计所有实体
        //   - cancellationToken: CancellationToken - 取消令牌
        // 返回值：Task<int> - 异步任务，返回统计数量
        public Task<int> CountAsync(
            Expression<Func<TEntity, bool>>? predicate = null,
            CancellationToken cancellationToken = default);

        // 方法：异步检查是否存在满足条件的实体
        // 作用：快速检查是否存在符合条件的实体，比CountAsync > 0更高效
        // 参数：
        //   - predicate: Expression<Func<TEntity, bool>> - 查询条件表达式
        //   - cancellationToken: CancellationToken - 取消令牌
        // 返回值：Task<bool> - 异步任务，返回是否存在
        // 性能：数据库找到第一条匹配记录就返回，不需要统计全部数量
        public Task<bool> ExistsAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default);


        // ========== 软删除相关方法（可选） ==========
        // 这些方法不是仓储模式的标准部分，但常用于实际业务

        // 方法：异步软删除实体
        // 作用：将实体的IsDeleted字段设置为true，实现逻辑删除
        // 前提：实体必须有bool类型的IsDeleted属性
        // 参数：
        //   - id: int - 要软删除的实体主键ID值
        //   - cancellationToken: CancellationToken - 取消令牌
        // 返回值：Task<bool> - 异步任务，返回操作是否成功
        // 异常：如果实体没有IsDeleted属性，抛出InvalidOperationException
        // public Task<bool> SoftDeleteAsync(int id, CancellationToken cancellationToken = default);

        // 方法：异步恢复软删除的实体
        // 作用：将实体的IsDeleted字段设置为false，恢复被软删除的实体
        // 前提：实体必须有bool类型的IsDeleted属性
        // 参数：
        //   - id: int - 要恢复的实体主键ID值
        //   - cancellationToken: CancellationToken - 取消令牌
        // 返回值：Task<bool> - 异步任务，返回操作是否成功
        // public Task<bool> RestoreAsync(int id, CancellationToken cancellationToken = default);
    }
}
