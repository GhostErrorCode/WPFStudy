namespace MemoApi.Enums
{
    /// <summary>
    /// 通用4位错误码常量类（WPF客户端+EF Core专属）
    /// 首位分类规则：
    /// 1xxx = 系统级错误 | 2xxx = 参数校验错误 | 3xxx = 业务逻辑错误
    /// 4xxx = 数据库操作错误 | 5xxx = 权限相关错误 | 6xxx = 第三方接口错误
    /// 7xxx = 文件操作错误 | 8xxx = 网络相关错误 | 9xxx = 自定义业务错误
    /// </summary>
    public static class ErrorCodes
    {
        #region 1xxx - 系统级错误
        public const int SystemInitFailed = 1001;        // 系统初始化失败（如DbContext/DI容器创建失败）
        public const int SystemInternalError = 1002;     // 系统内部异常（未捕获的通用异常）
        public const int ConfigLoadFailed = 1003;        // 配置加载失败（如数据库连接字符串读取失败）
        public const int ServiceNotRegistered = 1004;    // 服务未注册（DI容器中缺失仓储/Service依赖）
        #endregion

        #region 2xxx - 参数校验错误
        public const int ParameterNull = 2001;           // 参数为空（如仓储方法传空实体/空ID）
        public const int ParameterFormatError = 2002;    // 参数格式错误（如负数ID/手机号格式错误）
        public const int ParameterLengthExceeded = 2003; // 参数长度超限（如用户名过长/过短）
        public const int RequiredParameterMissing = 2004;// 必填参数缺失（如表单漏填用户名/密码）
        #endregion

        #region 3xxx - 业务逻辑错误
        public const int BusinessRuleValidationFailed = 3001; // 业务规则校验失败（如用户名重复/库存不足）
        public const int DataNotFound = 3002;                  // 数据不存在（根据ID查不到用户/备忘录/待办）
        public const int DataAlreadyExists = 3003;            // 数据已存在（新增时主键/唯一索引冲突）
        public const int OperationPermissionDenied = 3004;    // 操作权限不足（普通用户操作管理员数据）
        public const int DataStatusAbnormal = 3005;            // 数据状态异常（编辑已删除/锁定的记录）
        #endregion

        #region 4xxx - 数据库操作错误
        public const int DatabaseConnectionFailed = 4001;     // 数据库连接失败（SQLite文件无法访问/权限不足）
        public const int DatabaseOperationTimeout = 4002;     // 数据库操作超时（大批量数据操作超时）
        public const int DataInsertFailed = 4003;             // 数据插入失败（EF Core AddAsync失败）
        public const int DataUpdateFailed = 4004;             // 数据更新失败（EF Core Update失败/并发冲突）
        public const int DataDeleteFailed = 4005;             // 数据删除失败（外键关联/记录不存在）
        public const int MigrationScriptExecuteFailed = 4006; // 迁移脚本执行失败（EF Core数据库结构同步失败）
        #endregion

        #region 5xxx - 权限相关错误
        public const int NotLoggedInOrTokenExpired = 5001;    // 未登录/登录状态过期（客户端Token失效）
        public const int RolePermissionInsufficient = 5002;   // 角色权限不足（普通用户执行管理员操作）
        #endregion

        #region 6xxx - 第三方接口错误（客户端少见，预留）
        public const int ThirdPartyApiCallFailed = 6001;      // 第三方接口调用失败（如支付/推送接口调用失败）
        public const int ThirdPartyApiResponseAbnormal = 6002;// 第三方接口返回异常（数据格式非预期）
        #endregion

        #region 7xxx - 文件操作错误
        public const int FileNotFound = 7001;                 // 文件不存在（读取/写入本地文件失败）
        public const int FileReadWritePermissionDenied = 7002;// 文件读写权限不足（如保存到C盘根目录）
        public const int FileFormatNotSupported = 7003;       // 文件格式不支持（上传非指定格式文件）
        #endregion

        #region 8xxx - 网络相关错误（客户端少见，预留）
        public const int NetworkConnectionAbnormal = 8001;    // 网络连接异常（调用远程API时断网）
        public const int NetworkRequestTimeout = 8002;        // 网络请求超时（远程API响应超时）
        #endregion

        #region 9xxx - 自定义业务错误（项目专属，可按需扩展）
        public const int CustomOrderCanceled = 9001;          // 订单已取消（示例：电商类业务）
        public const int CustomGoodsOutOfStock = 9002;        // 商品已下架/库存不足（示例：电商类业务）
        public const int CustomMemoExpired = 9003;            // 备忘录已过期（示例：备忘录专属业务）
                                                              // 可根据项目实际需求继续添加...
        #endregion

        /// <summary>
        /// 成功状态码（无错误）
        /// </summary>
        public const int Success = 0;                        // 操作成功
    }
}
