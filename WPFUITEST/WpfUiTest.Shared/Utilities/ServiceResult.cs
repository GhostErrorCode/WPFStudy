using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WpfUiTest.Shared.Utilities
{
    // 服务层返回结果统一格式
    public class ServiceResult<T>
    {
        // 服务结果是否成功(默认失败)
        public bool IsSuccess { get; set; } = false;
        // 服务结果返回的业务数据(默认空)
        public T? Data { get; set; } = default(T?);
        // 服务结果消息(默认空串)
        public string Message { get; set; } = string.Empty;
        // 服务结果操作时间(默认当前时间)
        public DateTime Timestamp { get; set; } = DateTimeUtility.NowNoMilliseconds();


        // =============== 工厂方法 =================
        // 成功：无消息提示、无返回数据
        public static ServiceResult<T> Success()
        {
            return new ServiceResult<T>
            {
                IsSuccess = true,
                Data = default(T),
                Message = "成功!",
                Timestamp = DateTimeUtility.NowNoMilliseconds()
            };
        }
        // 成功：包含消息提示，无返回数据
        public static ServiceResult<T> Success(string message)
        {
            return new ServiceResult<T>
            {
                IsSuccess = true,
                Data = default(T),
                Message = message,
                Timestamp = DateTimeUtility.NowNoMilliseconds()
            };
        }
        // 成功：包含消息提示，无返回数据
        public static ServiceResult<T> Success(string message, T data)
        {
            return new ServiceResult<T>
            {
                IsSuccess = true,
                Data = data,
                Message = message,
                Timestamp = DateTimeUtility.NowNoMilliseconds()
            };
        }

        // 失败：包含消息提示，无返回数据
        public static ServiceResult<T> Failure(string message)
        {
            return new ServiceResult<T>
            {
                IsSuccess = false,
                Data = default(T),
                Message = message,
                Timestamp = DateTimeUtility.NowNoMilliseconds()
            };
        }
        // 失败：包含消息提示，有返回数据
        public static ServiceResult<T> Failure(string message, T data)
        {
            return new ServiceResult<T>
            {
                IsSuccess = false,
                Data = data,
                Message = message,
                Timestamp = DateTimeUtility.NowNoMilliseconds()
            };
        }

        // 对外输出JSON格式
        public string ToJson()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions()
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, // 允许中文
            });
        }
    }
}
