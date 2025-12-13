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
            ErrorCode = ErrorCodes.Success
        };

        /// <summary>创建成功结果（无返回结果）</summary>
        public static ServiceResult<T> Success() => new()
        {
            IsSuccess = true,
            Data = default(T),
            ErrorCode = ErrorCodes.Success
        };

        /// <summary>创建失败结果（指定错误码）</summary>
        public static ServiceResult<T> Fail(int errorCode, string errorMessage) => new()
        {
            IsSuccess = false,
            ErrorCode = errorCode,
            ErrorMessage = errorMessage
        };

        /// <summary>创建失败结果（默认系统错误）</summary>
        public static ServiceResult<T> Fail(string errorMessage) => new()
        {
            IsSuccess = false,
            ErrorCode = ErrorCodes.SystemInternalError,
            ErrorMessage = errorMessage
        };
    }
}
