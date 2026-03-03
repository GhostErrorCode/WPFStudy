using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using WpfUiTest.Core.Data.Entities;

namespace WpfUiTest.Core.Data.Repositories.Interfaces
{
    // 产品具体仓储接口
    // 作用：继承通用仓储接口，添加产品实体特有的业务数据访问方法
    // 继承关系：IProductRepository : IRepository<Product>
    // 设计原则：接口隔离原则 - 为特定客户端（ProductService）提供特定接口
    // 优势：1. 方法名具有业务语义 2. 编译时类型安全 3. 易于测试和Mock
    public interface IUserRepository : IRepository<User>
    {
        // 注意：这里不需要重新声明GetByIdAsync、AddAsync等基础方法
        // 它们已从父接口IRepository<User>继承


        // ========== 用户特有的业务查询方法 ==========
        // 这些方法封装了产品特有的复杂查询逻辑，使业务层代码更简洁
        // <summary>
        /// 根据用户账户名查找唯一的用户实体
        /// </summary>
        /// <param name="account">要查找的用户账户名</param>
        /// <returns>找到的用户实体；如果未找到则返回null</returns>
        public Task<User?> UserGetByAccountAsync(string account, CancellationToken cancellationToken = default);

        // 根据用户ID查询用户实体
        public Task<User?> UserGetByIdAsync(int id, CancellationToken cancellationToken = default);


        /// <summary>
        /// 检查指定的用户账户名是否已存在于数据库中
        /// </summary>
        /// <param name="account">要检查的用户账户名</param>
        /// <returns>如果账户名已存在返回true，否则返回false</returns>
        public Task<bool> UserAccountExistsAsync(string account, CancellationToken cancellationToken = default);

        // 添加用户实体
        public Task<User> AddUserAsync(User user, CancellationToken cancellationToken = default);
    }

}
