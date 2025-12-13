using MemoApi.Entities;

namespace MemoApi.Repositories
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


        // ========== 产品特有的业务查询方法 ==========
        // 这些方法封装了产品特有的复杂查询逻辑，使业务层代码更简洁

        // 方法：异步获取热销产品列表
        // 作用：获取热销产品的列表，用于首页展示等场景
        // 参数：
        //   - topCount: int - 返回的热销产品数量，默认10个
        //   - cancellationToken: CancellationToken - 取消令牌
        // 返回值：Task<List<Product>> - 异步任务，返回热销产品列表
        // 业务规则：热销产品 = IsHotSelling=true 且 库存>0 且 未删除
        // 排序规则：按销量降序，销量相同按名称升序
        //Task<List<Product>> GetHotSellingProductsAsync(
        //    int topCount = 10,
        //    CancellationToken cancellationToken = default);

        // 方法：异步根据分类ID获取产品
        // 作用：获取指定分类下的所有产品
        // 参数：
        //   - categoryId: int - 分类ID，用于筛选产品
        //   - includeCategory: bool - 是否包含关联的分类实体信息
        //     true: 使用Include加载关联数据，SQL包含JOIN操作
        //     false: 只加载产品数据，性能更好
        //   - cancellationToken: CancellationToken - 取消令牌
        // 返回值：Task<List<Product>> - 异步任务，返回产品列表
        //Task<List<Product>> GetProductsByCategoryAsync(
        //    int categoryId,
        //    bool includeCategory = true,
        //    CancellationToken cancellationToken = default);

        // 方法：异步搜索产品（多条件搜索）
        // 作用：根据多个条件搜索产品，支持关键字、价格范围、库存状态等
        // 参数：
        //   - keyword: string? - 搜索关键词，可空，匹配名称或描述
        //   - minPrice: decimal? - 最低价格，可空，价格范围筛选
        //   - maxPrice: decimal? - 最高价格，可空，价格范围筛选
        //   - inStockOnly: bool - 是否只显示有库存的产品
        //   - excludeDeleted: bool - 是否排除已删除的产品，默认true
        //   - cancellationToken: CancellationToken - 取消令牌
        // 返回值：Task<List<Product>> - 异步任务，返回搜索结果的產品列表
        // 排序规则：按价格升序排列
        //Task<List<Product>> SearchProductsAsync(
        //    string? keyword = null,
        //    decimal? minPrice = null,
        //    decimal? maxPrice = null,
        //    bool inStockOnly = false,
        //    bool excludeDeleted = true,
        //    CancellationToken cancellationToken = default);

        // 方法：异步更新产品库存
        // 作用：更新产品的库存数量，支持增加和减少库存
        // 参数：
        //   - productId: int - 产品ID，指定要更新哪个产品
        //   - quantityChange: int - 库存变化量，正数增加库存，负数减少库存
        //   - cancellationToken: CancellationToken - 取消令牌
        // 返回值：Task<bool> - 异步任务，返回操作是否成功
        // 业务规则：减少库存时不能导致库存为负数
        // 自动逻辑：当销量达到1000时自动标记为热销产品
        //Task<bool> UpdateStockAsync(
        //    int productId,
        //    int quantityChange,
        //    CancellationToken cancellationToken = default);

        //// 方法：异步获取价格区间统计
        //// 作用：统计各个价格区间的产品数量，用于数据分析和图表展示
        //// 参数：
        ////   - excludeDeleted: bool - 是否排除已删除的产品，默认true
        ////   - cancellationToken: CancellationToken - 取消令牌
        //// 返回值：Task<Dictionary<string, int>> - 异步任务，返回价格区间统计字典
        ////     键：价格区间标签（如"0-100元"）
        ////     值：该区间的产品数量
        //Task<Dictionary<string, int>> GetPriceRangeStatisticsAsync(
        //    bool excludeDeleted = true,
        //    CancellationToken cancellationToken = default);

        //// 方法：异步获取库存预警产品
        //// 作用：获取库存低于阈值的产品，用于库存预警和管理
        //// 参数：
        ////   - threshold: int - 库存预警阈值，默认10
        ////   - cancellationToken: CancellationToken - 取消令牌
        //// 返回值：Task<List<Product>> - 异步任务，返回库存预警产品列表
        //// 排序规则：按库存升序排列（库存最少的排前面）
        ////Task<List<Product>> GetLowStockProductsAsync(
        ////    int threshold = 10,
        ////    CancellationToken cancellationToken = default);

        //// 方法：异步批量更新产品价格（按百分比）
        //// 作用：批量调整产品价格，例如全场涨价10%或打折20%
        //// 参数：
        ////   - percentageChange: decimal - 价格变化百分比，正数涨价，负数打折
        ////     示例：10表示涨价10%，-20表示打8折（降价20%）
        ////   - cancellationToken: CancellationToken - 取消令牌
        //// 返回值：Task<int> - 异步任务，返回更新的产品数量
        //// 业务规则：价格保留两位小数，自动更新更新时间戳
        //Task<int> BulkUpdatePriceByPercentageAsync(
        //    decimal percentageChange,
        //    CancellationToken cancellationToken = default);
    }
}
