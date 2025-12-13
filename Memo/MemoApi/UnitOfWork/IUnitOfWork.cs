// 引入命名空间：System
// 作用：提供IDisposable接口，用于资源释放管理
using MemoApi.Repositories;
using System;

namespace MemoApi.UnitOfWork
{
    // 工作单元接口
    // 作用：定义工作单元模式的核心契约，协调多个仓储操作和数据库事务
    // 设计模式：工作单元模式，确保数据操作的一致性和事务完整性
    // 接口继承：IDisposable，要求实现资源释放逻辑
    // 核心职责：
    //   1. 提供统一的仓储访问入口
    //   2. 管理数据库事务（开始、提交、回滚）
    //   3. 统一提交所有更改到数据库
    //   4. 释放数据库连接等资源
    public interface IUnitOfWork : IDisposable
    {
        // 方法：获取通用仓储实例
        // 作用：为指定实体类型获取一个实现了IRepository<TEntity>接口的仓储实例
        // 泛型参数：
        //   - TEntity：实体类型，必须是引用类型（class）
        // 返回值：IRepository<TEntity> - 对应实体类型的仓储接口
        // 设计思想：通过泛型方法动态获取任意实体类型的仓储，无需为每个实体单独定义属性
        // 使用示例：var productRepo = unitOfWork.GetRepository<Product>();
        //           var categoryRepo = unitOfWork.GetRepository<Category>();
        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;

        // 方法：异步保存所有更改到数据库
        // 作用：提交所有通过工作单元进行的实体更改（增、删、改）到数据库
        // 参数：
        //   - cancellationToken: CancellationToken - 取消令牌，用于取消异步操作
        // 返回值：Task<int> - 异步任务，返回受影响的数据行数
        // 核心重要性：这是工作单元模式最关键的"总开关"
        //   如果不调用此方法，所有仓储的AddAsync、Update、DeleteAsync操作都不会真正保存到数据库！
        // 异常处理：如果保存失败，会抛出异常，已开启的事务会自动回滚
        // 返回值用途：判断操作是否成功（受影响行数>0表示成功）
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);



        // ========== 事务控制方法（可选但推荐） ==========
        // 这些方法用于显式控制数据库事务，适用于复杂的业务操作

        // 方法：异步开始数据库事务
        // 作用：显式开始一个数据库事务，后续所有操作都在这个事务中执行
        // 参数：
        //   - cancellationToken: CancellationToken - 取消令牌
        // 返回值：Task - 异步任务，无返回值
        // 使用场景：需要手动控制事务边界的复杂业务操作
        // 注意事项：
        //   1. 必须先调用此方法开始事务，才能使用CommitTransactionAsync或RollbackTransactionAsync
        //   2. 如果已存在活跃事务，调用此方法会抛出InvalidOperationException
        //   3. 事务开始后，所有仓储操作都使用同一个数据库连接
        // 事务隔离级别：使用数据库默认隔离级别，通常是READ COMMITTED
        public Task BeginTransactionAsync(CancellationToken cancellationToken = default);

        // 方法：异步提交当前事务
        // 作用：提交当前活跃的事务，将所有未提交的更改永久保存到数据库
        // 参数：
        //   - cancellationToken: CancellationToken - 取消令牌
        // 返回值：Task - 异步任务，无返回值
        // 执行流程：
        //   1. 先调用SaveChangesAsync保存所有实体更改
        //   2. 然后提交数据库事务
        //   3. 最后释放事务资源
        // 异常处理：如果提交失败，事务会自动回滚
        // 重要原则：通常在try块中调用，catch块中调用RollbackTransactionAsync
        public Task CommitTransactionAsync(CancellationToken cancellationToken = default);

        // 方法：异步回滚当前事务
        // 作用：回滚当前活跃的事务，撤销所有未提交的更改
        // 参数：
        //   - cancellationToken: CancellationToken - 取消令牌
        // 返回值：Task - 异步任务，无返回值
        // 使用场景：
        //   1. 业务操作失败时（如库存不足、金额错误等）
        //   2. 发生未处理异常时
        //   3. 用户取消操作时
        // 设计优势：确保数据一致性，要么全部成功，要么全部回滚
        public Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}
