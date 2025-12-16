namespace MemoDesktop.Dtos.Memo
{
    /// <summary>
    /// 备忘录数据传输对象（用于查询返回）
    /// 此类定义了API接口返回给客户端的数据格式
    /// </summary>
    public class MemoDto
    {
        /// <summary>
        /// 备忘录的唯一标识符
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 备忘录的标题
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 备忘录的详细内容
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// 备忘录的创建时间（格式化后的字符串）
        /// </summary>
        public string CreateDateFormatted { get; set; } = string.Empty;

        /// <summary>
        /// 备忘录的最后修改时间（格式化后的字符串）
        /// </summary>
        public string UpdateDateFormatted { get; set; } = string.Empty;

    }
}


/// <summary>
/// 数据约束特性速查（适配WPF+EF Core场景）
/// 分类：1.DTO/ViewModel层验证约束（System.ComponentModel.DataAnnotations）；2.EF Core数据库层约束
/// 使用说明：直接复制特性到字段上，配合Validator.TryValidateObject触发校验（DTO层）/EF Core迁移生成表结构（数据库层）
/// </summary>
#region 1. DTO/ViewModel层 - 前端/服务层数据验证约束
/// <summary>
/// Required：必填项校验（默认不允许空字符串）
/// 示例：[Required(ErrorMessage = "标题不能为空")] public string Title { get; set; }
/// 扩展：AllowEmptyStrings = true 允许空字符串（[Required(AllowEmptyStrings = true)]）
/// </summary>
/// <summary>
/// MaxLength(n)：字符串/集合最大长度
/// 示例：[MaxLength(50, ErrorMessage = "标题最长50字")] public string Title { get; set; }
/// </summary>
/// <summary>
/// MinLength(n)：字符串/集合最小长度
/// 示例：[MinLength(6, ErrorMessage = "密码至少6位")] public string Password { get; set; }
/// </summary>
/// <summary>
/// StringLength(n, MinimumLength=m)：同时限制字符串最大/最小长度（更简洁）
/// 示例：[StringLength(50, MinimumLength = 2, ErrorMessage = "标题长度2-50字")] public string Title { get; set; }
/// </summary>
/// <summary>
/// Range(min, max)：数值类型范围校验（int/decimal/DateTime等）
/// 示例：[Range(1, 999, ErrorMessage = "数量需1-999")] public int Count { get; set; }
/// </summary>
/// <summary>
/// RegularExpression(正则)：自定义格式校验（手机号/身份证等）
/// 示例：[RegularExpression(@"^1[3-9]\d{9}$", ErrorMessage = "手机号格式错误")] public string Phone { get; set; }
/// </summary>
/// <summary>
/// EmailAddress：邮箱格式校验（比手写正则可靠）
/// 示例：[EmailAddress(ErrorMessage = "邮箱格式错误")] public string Email { get; set; }
/// </summary>
/// <summary>
/// Phone：手机号/座机格式校验（适配国际号码，国内手机号建议用正则）
/// 示例：[Phone(ErrorMessage = "联系电话格式错误")] public string Phone { get; set; }
/// </summary>
/// <summary>
/// Url：URL格式校验（待办/用户链接字段）
/// 示例：[Url(ErrorMessage = "链接格式错误")] public string Link { get; set; }
/// </summary>
/// <summary>
/// Compare("字段名")：校验两个字段值一致（密码+确认密码）
/// 示例：[Compare("Password", ErrorMessage = "两次密码不一致")] public string ConfirmPassword { get; set; }
/// </summary>
/// <summary>
/// DataType(类型)：标记字段类型（辅助校验+界面展示）
/// 示例1：[DataType(DataType.Password)] public string Password { get; set; }
/// 示例2：[DataType(DataType.Date)] public DateTime Deadline { get; set; }
/// </summary>
/// <summary>
/// CustomValidation：自定义复杂校验逻辑（如截止时间不能早于当前时间）
/// 示例：[CustomValidation(typeof(ToDoValidator), "ValidateDeadline")] public DateTime? Deadline { get; set; }
/// 配套验证类：
/// public static class ToDoValidator {
///   public static ValidationResult ValidateDeadline(DateTime? deadline, ValidationContext ctx) {
///     var todo = (AddToDoDto)ctx.ObjectInstance;
///     if (deadline < DateTime.Now) return new ValidationResult("截止时间不能早于当前时间");
///     return ValidationResult.Success;
///   }
/// }
/// </summary>
/// <summary>
/// CreditCard：信用卡格式校验（极少用，仅作补充）
/// 示例：[CreditCard(ErrorMessage = "信用卡号格式错误")] public string CardNo { get; set; }
/// </summary>
#endregion

#region 2. EF Core数据库层 - 表结构约束（映射到数据库）
/// <summary>
/// Key：标记主键（复合主键可多个字段加[Key]，或OnModelCreating配置）
/// 示例：[Key] public long Id { get; set; }
/// </summary>
/// <summary>
/// Index(IsUnique = true)：标记唯一索引（如用户邮箱不重复）
/// 示例：[Index(nameof(Email), IsUnique = true, ErrorMessage = "邮箱已存在")] public string Email { get; set; }
/// </summary>
/// <summary>
/// Column(TypeName = "类型")：指定数据库字段类型（避免EF Core自动推断）
/// 示例：[Column(TypeName = "VARCHAR(50)")] public string Title { get; set; }
/// </summary>
/// <summary>
/// DatabaseGenerated：指定字段值生成方式（自增/计算列/无）
/// 示例1（自增主键）：[DatabaseGenerated(DatabaseGeneratedOption.Identity)] public long Id { get; set; }
/// 示例2（不生成）：[DatabaseGenerated(DatabaseGeneratedOption.None)] public string Code { get; set; }
/// </summary>
/// <summary>
/// ConcurrencyCheck：乐观锁（并发检查，防止多用户同时修改）
/// 示例：[ConcurrencyCheck] public string Content { get; set; }
/// </summary>
/// <summary>
/// ForeignKey("关联实体/外键字段")：标记外键（如待办关联用户）
/// 示例：[ForeignKey("UserId")] public User Creator { get; set; }
/// 配套外键字段：public long UserId { get; set; }
/// </summary>
/// <summary>
/// Timestamp：时间戳（替代ConcurrencyCheck，自动生成，用于并发控制）
/// 示例：[Timestamp] public byte[] RowVersion { get; set; }
/// </summary>
#endregion

/// <summary>
/// 约束使用核心原则：
/// 1. DTO层用「验证约束」（前端/服务层校验），实体层用「数据库约束」（表结构限制）；
/// 2. 优先服务层校验（减少数据库交互），数据库约束作为最后防线；
/// 3. 高频约束：StringLength/RegularExpression/Index(IsUnique=true)（ToDo/User场景）；
/// 4. 触发DTO校验：Validator.TryValidateObject(dto, new ValidationContext(dto), results, validateAllProperties: true)
/// </summary>

