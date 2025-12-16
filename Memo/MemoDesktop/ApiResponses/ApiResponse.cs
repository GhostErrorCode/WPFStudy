using System;
using System.Collections.Generic;
using System.Text;

namespace MemoDesktop.ApiResponses
{
    /// <summary>
    /// 业务操作结果包装器（对应后端ServiceResult<T>）
    /// 用于统一前后端响应结构
    /// </summary>
    /// <typeparam name="T">返回的数据类型</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// 操作是否成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 业务数据（成功时有效）
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// 错误描述信息
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// 4位业务错误码
        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        /// 操作时间戳（UTC）
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}
