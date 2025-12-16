using MemoApi.Enums;

namespace MemoApi.Results
{
    /// <summary>
    /// 业务操作结果包装器 - 泛型版本
    /// </summary>
    /// <typeparam name="T">返回的数据类型</typeparam>
    public class ServiceResult<T>
    {
        /// <summary>操作是否成功</summary>
        public bool IsSuccess { get; init; }

        /// <summary>业务数据（成功时有效）</summary>
        public T? Data { get; init; }

        /// <summary>错误描述信息</summary>
        public string? ErrorMessage { get; init; }

        /// <summary>4位业务错误码</summary>
        public int ErrorCode { get; init; }

        /// <summary>操作时间戳（UTC）</summary>
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;


        // ============= 工厂方法 =============

        /// <summary>创建成功结果</summary>
        public static ServiceResult<T> Success(T data) => new()
        {
            IsSuccess = true,
            Data = data,
            ErrorCode = ErrorCodes.Success,
            ErrorMessage = default(string)
        };

        /// <summary>创建成功结果（无返回结果）</summary>
        public static ServiceResult<T> Success() => new()
        {
            IsSuccess = true,
            Data = default(T),
            ErrorCode = ErrorCodes.Success,
            ErrorMessage = default(string)
        };

        /// <summary>创建失败结果（指定错误码）</summary>
        public static ServiceResult<T> Fail(int errorCode, string errorMessage) => new()
        {
            IsSuccess = false,
            Data = default(T),
            ErrorCode = errorCode,
            ErrorMessage = errorMessage
        };

        /// <summary>创建失败结果（默认系统错误）</summary>
        public static ServiceResult<T> Fail(string errorMessage) => new()
        {
            IsSuccess = false,
            Data = default(T),
            ErrorCode = ErrorCodes.SystemInternalError,
            ErrorMessage = errorMessage
        };
    }
}




// ====================== default 关键字全场景注释（适配.NET 10） ======================
// 核心规则：值类型返回“零值”，引用类型返回null，泛型自动适配对应类型默认值

// 一、基础值类型（新手最常用）
// 1. 整数类型
// default(int) → 0
// default(long) → 0
// default(short) → 0
// default(byte) → 0（取值范围0-255）
// default(uint) → 0（无符号整数）
// default(ulong) → 0（无符号长整数）

// 2. 布尔类型
// default(bool) → false

// 3. 小数类型
// default(decimal) → 0.0000000000000000000000000000
// default(double) → 0
// default(float) → 0

// 4. 字符类型
// default(char) → ASCII码0（空字符，(int)default(char) = 0）

// 5. 日期类型
// default(DateTime) → 0001/1/1 0:00:00（DateTime最小值）

// 二、自定义值类型（结构体struct）
// 规则：结构体的所有字段自动设为各自类型的默认值
// 示例结构体定义：
// public struct Point { public int X; public int Y; public string Name; }
// default(Point) → X=0, Y=0, Name=null（X/Y是int默认0，Name是string默认null）

// 三、引用类型（string/自定义类/数组/接口/集合等）
// 核心：所有引用类型的default值均为null

// 1. 字符串（特殊引用类型）
// default(string) → null（注意：不是空字符串""）
// 对比："" 是有内存指向但无字符，null是无内存指向

// 2. 自定义类
// 示例类定义：public class User { public int Id; public string Name; }
// default(User) → null

// 3. 数组
// default(int[]) → null（数组本身是引用类型，默认null；数组元素默认值需初始化）
// default(string[]) → null

// 4. 接口
// 示例接口定义：public interface ITest { void DoSomething(); }
// default(ITest) → null

// 5. 集合（List/Dictionary等）
// default(List<string>) → null
// default(Dictionary<int, string>) → null

// 四、特殊类型
// 1. 可空类型（Nullable<T>）
// default(int?) → null（可空值类型默认null，而非对应值类型的零值）
// default(DateTime?) → null

// 2. Task相关
// default(Task) → null
// default(Task<string>) → null（泛型Task仍为引用类型，默认null）

// 3. 泛型方法（default核心用途）
// 泛型方法内可简写default（等价于default(T)，C#7.1+支持）
// 示例：
// static T GetDefaultValue<T>() { return default; }
// GetDefaultValue<int>() → 0（int默认值）
// GetDefaultValue<string>() → null（string默认值）
// GetDefaultValue<User>() → null（User类默认值）
