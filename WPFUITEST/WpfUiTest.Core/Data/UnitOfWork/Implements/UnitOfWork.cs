using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using WpfUiTest.Core.Data.DbContexts;
using WpfUiTest.Core.Data.UnitOfWork.Interfaces;

namespace WpfUiTest.Core.Data.UnitOfWork.Implements
{
    // 工作单元实现类
    // 作用：实现IUnitOfWork接口，具体协调多个仓储和数据库事务
    // 核心设计：
    //   1. 所有仓储共享同一个DbContext实例
    //   2. 使用缓存避免重复创建仓储实例
    //   3. 实现显式事务控制
    //   4. 确保资源正确释放
    // 生命周期：Scoped（一次HTTP请求），确保一次业务操作使用同一个工作单元
    public class UnitOfWork : IUnitOfWork
    {
        // 私有只读字段：数据库上下文
        // 访问修饰符：private readonly
        //   - private：只能在当前类内部访问
        //   - readonly：只能在构造函数中赋值，之后不能修改
        // 作用：所有通过此工作单元创建的仓储都共享这个DbContext实例
        // 重要性：这是保证事务一致性的关键！所有仓储必须使用同一个DbContext
        private readonly ApplicationDbContext _applicationDbContext;

        // 私有字段：仓储实例缓存字典
        // 数据类型：ConcurrentDictionary<Type, object>
        //   - ConcurrentDictionary：线程安全的字典，支持并发访问
        //   - Type：键类型，存储实体类型（如typeof(Product)）
        //   - object：值类型，存储仓储实例（由于泛型擦除，用object存储）
        // 设计目的：
        //   1. 避免重复创建同类型仓储实例（节省内存）
        //   2. 确保同类型仓储只创建一次（单例行为）
        //   3. 线程安全，支持多线程并发访问
        // private readonly ConcurrentDictionary<Type, object> _repositories;

        // 私有字段：当前数据库事务对象
        // 数据类型：IDbContextTransaction? - 可空的事务接口
        // 问号(?)表示：该字段可以为null，表示没有活跃事务
        // 作用：用于显式事务控制，当为null时使用隐式事务
        // 数据库事务：就是数据库操作的 “打包办事”—— 比如你要做两件事：给用户扣 100 块 + 给用户加 1 个订单，这俩事必须 “要么都成，要么都败”（比如扣了钱但没加订单，用户就亏了），这时候就得用 “事务” 把这俩操作包起来。
        // 显式事务 vs 隐式事务：
        // 1.显式事务：你手动告诉数据库 “我要开个事务了”（比如点 “开始办事”），办完事再手动说 “确认提交” 或 “取消回滚”；
        // 2.隐式事务：你没手动开事务，数据库默认 “办一件事算一件”（比如单独插一条数据，插完就自动确认），不用你管。
        private IDbContextTransaction? _currentTransaction;

        // 私有字段：资源释放标志
        // 数据类型：bool
        // 作用：标记对象是否已被释放，防止重复释放资源
        // 重要：这是实现IDisposable模式的标准做法
        private bool _disposed = false;



        // 构造函数
        // 作用：初始化工作单元，接收数据库上下文
        // 参数：
        //   - context: DbContext - 数据库上下文实例
        // 设计原则：依赖注入，从外部接收依赖项
        // 参数验证：确保传入的context不为null，避免空引用异常
        /*
        public UnitOfWork(ApplicationDbContext applicationDbContext)
        {
            // 参数验证：使用if语句检查context是否为null
            // 作用：尽早发现配置错误，提供清晰的错误信息
            if (applicationDbContext == null)
            {
                // 抛出ArgumentNullException异常
                // 参数1：nameof(context) - 获取参数名"context"（编译时安全）
                // 参数2：错误消息，帮助调试
                throw new ArgumentNullException(nameof(applicationDbContext), "数据库上下文不能为空");
            }

            // 初始化字段
            this._applicationDbContext = applicationDbContext;  // 存储传入的DbContext
            this._currentTransaction = null;  // 初始没有活跃事务
        }


        // 实现接口方法：获取仓储实例
        // 作用：获取或创建指定实体类型的仓储实例
        // 泛型约束：where TEntity : class - TEntity必须是引用类型
        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            // 获取实体类型
            // typeof(TEntity)：获取泛型类型参数的Type对象
            // 示例：如果TEntity是Product，则entityType是typeof(Product)
            Type entityType = typeof(TEntity);

            // 使用线程安全字典的GetOrAdd方法
            // 作用：如果字典中已有该类型的仓储，则返回现有实例；否则创建新实例
            // 参数1：entityType - 查找键（实体类型）
            // 参数2：值工厂函数 - 当键不存在时，调用此函数创建新值
            object repository = this._repositories.GetOrAdd(
                entityType,  // 键：实体类型

                // 值工厂：当键不存在时执行的创建逻辑
                // 参数：type（这里用_表示不使用这个参数）
                // 返回：新创建的仓储实例
                // 下划线是 C# 里的 “占位符”，专门用来表示 “这个参数我声明了，但完全不用”。
                (Type _) =>
                {
                    // 创建通用仓储实例
                    // 注意：这里创建的是通用仓储Repository<TEntity>，不是具体仓储
                    // 具体仓储应该通过依赖注入获取，而不是通过工作单元
                    // 这是因为具体仓储可能有额外的依赖项或特殊逻辑
                    return new Repository<TEntity>(this._context);
                }
            );

            // 类型转换：将object转换为具体的仓储接口类型
            // 这是安全的，因为repository确实是Repository<TEntity>类型
            // 转换失败会抛出InvalidCastException
            IRepository<TEntity> typedRepository = (IRepository<TEntity>)repository;

            // 返回转换后的仓储实例
            return typedRepository;
        }
        */

        // 实现接口方法：异步保存所有更改
        // 核心方法：这是工作单元模式的"总开关"
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // 局部变量：受影响的行数
            // 作用：记录数据库操作影响的数据行数
            int affectedRows = 0;

            // try-catch块：处理可能发生的异常
            // 作用：确保发生异常时能正确处理事务回滚
            try
            {
                // 调用DbContext的SaveChangesAsync方法
                // 作用：将所有挂起的更改（Added/Modified/Deleted状态）保存到数据库
                // 执行过程：
                //   1. 检查所有被跟踪的实体状态
                //   2. 生成对应的SQL语句（INSERT/UPDATE/DELETE）
                //   3. 如果开启了事务，在事务内执行SQL
                //   4. 执行SQL并返回受影响的行数
                // 注意：如果_context未开启显式事务，SaveChangesAsync会自动开启隐式事务
                affectedRows = await this._applicationDbContext.SaveChangesAsync(cancellationToken);

                // 如果有活跃的显式事务，提交它
                // 检查：this._currentTransaction != null 表示有显式事务
                if (this._currentTransaction != null)
                {
                    // 提交事务：将事务中的所有操作永久保存到数据库
                    await this._currentTransaction.CommitAsync(cancellationToken);

                    // 释放事务资源：事务提交后需要显式释放
                    await this._currentTransaction.DisposeAsync();

                    // 将事务引用设为null，表示没有活跃事务
                    this._currentTransaction = null;
                }
            }
            catch (Exception ex)
            {
                // 发生异常，需要回滚事务（如果存在）
                // 检查：this._currentTransaction != null 表示有需要回滚的事务
                if (this._currentTransaction != null)
                {
                    // 回滚事务：撤销事务中的所有未提交操作
                    await this._currentTransaction.RollbackAsync(cancellationToken);

                    // 释放事务资源
                    await this._currentTransaction.DisposeAsync();

                    // 将事务引用设为null
                    this._currentTransaction = null;
                }

                // 重新抛出异常，让上层调用者处理
                // 包装异常：提供更清晰的错误信息
                // 参数1：错误消息，描述发生了什么
                // 参数2：原始异常，保留原始堆栈跟踪
                throw new Exception("保存数据时发生错误", ex);
            }

            // 返回受影响的行数
            // 用途：调用者可以判断操作是否成功（>0表示成功）
            return affectedRows;
        }


        // 实现接口方法：异步开始事务
        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            // 检查是否已有活跃事务
            // 作用：防止嵌套事务（虽然EF Core支持，但显式管理时最好避免）
            if (this._currentTransaction != null)
            {
                // 抛出异常：已经有活跃事务，不能开始新事务
                throw new InvalidOperationException("已存在活跃的事务，不能开始新事务");
            }

            // 开始新的数据库事务
            // Database.BeginTransactionAsync：DbContext的方法，开始一个数据库事务
            // 返回：IDbContextTransaction对象，用于后续的提交或回滚
            this._currentTransaction = await this._applicationDbContext.Database.BeginTransactionAsync(cancellationToken);
        }


        // 实现接口方法：异步提交事务
        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            // 检查是否有活跃事务可提交
            if (this._currentTransaction == null)
            {
                throw new InvalidOperationException("没有活跃的事务可提交");
            }

            // try-finally块：确保无论如何都释放事务资源
            try
            {
                // 先保存所有更改到数据库
                // 注意：必须在提交事务前保存更改
                await this._applicationDbContext.SaveChangesAsync(cancellationToken);

                // 提交事务
                await this._currentTransaction.CommitAsync(cancellationToken);
            }
            finally
            {
                // finally块：无论try块是否抛出异常都会执行
                // 作用：确保事务资源被正确释放

                // 检查事务是否仍然存在（可能在CommitAsync时已被释放）
                if (this._currentTransaction != null)
                {
                    // 释放事务资源
                    await this._currentTransaction.DisposeAsync();

                    // 将引用设为null，表示没有活跃事务
                    this._currentTransaction = null;
                }
            }
        }


        // 实现接口方法：异步回滚事务
        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            // 检查是否有活跃事务可回滚
            if (this._currentTransaction == null)
            {
                throw new InvalidOperationException("没有活跃的事务可回滚");
            }

            // try-finally块：确保释放事务资源
            try
            {
                // 回滚事务：撤销所有未提交的更改
                await this._currentTransaction.RollbackAsync(cancellationToken);
            }
            finally
            {
                // 释放事务资源
                if (this._currentTransaction != null)
                {
                    await this._currentTransaction.DisposeAsync();
                    this._currentTransaction = null;
                }
            }
        }


        // 受保护方法：释放资源（IDisposable模式的标准实现）
        // 作用：实际执行资源释放逻辑
        // 参数：
        //   - disposing: bool - 表示是主动调用Dispose()还是垃圾回收
        //     true：主动调用Dispose()，需要释放托管资源
        //     false：垃圾回收器调用，只释放非托管资源
        // 访问修饰符：protected virtual
        //   - protected：允许子类访问
        //   - virtual：允许子类重写此方法
        protected virtual void Dispose(bool disposing)
        {
            // 检查是否已释放
            // 作用：防止重复释放资源（多次调用Dispose()）
            if (!this._disposed)
            {
                // disposing为true表示是主动调用Dispose()
                if (disposing)
                {
                    // 释放托管资源（由CLR管理的内存资源）

                    // 释放DbContext（会关闭数据库连接）
                    if (this._applicationDbContext != null)
                    {
                        this._applicationDbContext.Dispose();
                    }

                    // 释放事务资源（如果还有活跃事务）
                    if (this._currentTransaction != null)
                    {
                        this._currentTransaction.Dispose();
                    }

                    // 清空仓储缓存（帮助垃圾回收）
                    // this._repositories?.Clear();
                }

                // 标记为已释放
                // 重要：必须在释放资源后设置，防止重复释放
                this._disposed = true;
            }
        }


        // 公开方法：释放资源（IDisposable接口实现）
        // 作用：供外部调用者释放资源
        // 调用场景：
        //   1. using语句结束时自动调用
        //   2. 手动调用Dispose()方法
        //   3. 依赖注入容器在作用域结束时调用
        public void Dispose()
        {
            // 调用受保护的Dispose方法，传递true表示主动释放
            this.Dispose(true);

            // 通知垃圾回收器不再需要调用终结器
            // 作用：提高性能，避免额外的垃圾回收开销
            // 原理：如果已经调用了Dispose()，就不需要垃圾回收器再调用终结器
            GC.SuppressFinalize(this);
        }


        // 终结器（析构函数）
        // 作用：最后的资源释放保障，防止忘记调用Dispose()
        // 语法：~类名()
        // 调用时机：垃圾回收器回收对象时自动调用
        // 注意：终结器的调用时间不确定，不应该依赖它释放重要资源
        ~UnitOfWork()
        {
            // 调用Dispose方法，传递false表示是垃圾回收器调用
            // 只释放非托管资源，不释放托管资源
            this.Dispose(false);
        }
    }
}
