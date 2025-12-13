using MemoApi.Entities;

namespace MemoApi.Repositories
{
    // 备忘录具体仓储接口
    // 作用：继承通用仓储接口，添加备忘录实体特有的业务数据访问方法
    // 继承关系：IMemoRepository : IRepository<Memo>
    // 设计原则：接口隔离原则 - 为特定客户端（MemoService）提供特定接口
    // 优势：1. 方法名具有业务语义 2. 编译时类型安全 3. 易于测试和Mock
    public interface IMemoRepository : IRepository<Memo>
    {
        // 注意：这里不需要重新声明GetByIdAsync、AddAsync等基础方法
        // 它们已从父接口IRepository<User>继承


        // ========== 产品特有的业务查询方法 ==========
        // 这些方法封装了产品特有的复杂查询逻辑，使业务层代码更简洁
    }
}
