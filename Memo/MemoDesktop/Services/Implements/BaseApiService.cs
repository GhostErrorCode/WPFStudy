using MemoDesktop.ApiResponses;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace MemoDesktop.Services.Implements
{
    /// <summary>
    /// API服务基类
    /// 提供统一的HTTP请求处理逻辑
    /// </summary>
    public class BaseApiService
    {
        /// <summary>
        /// HTTP客户端实例
        /// </summary>
        private readonly HttpClient _httpClient;

        /// <summary>
        /// JSON序列化选项
        /// </summary>
        private readonly JsonSerializerOptions _jsonOptions;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="httpClient">HTTP客户端</param>
        public BaseApiService(HttpClient httpClient)
        {
            // 保存HTTP客户端引用
            this._httpClient = httpClient;

            // 配置JSON序列化选项
            this._jsonOptions = new JsonSerializerOptions
            {
                // 属性名不区分大小写（与后端保持一致）
                PropertyNameCaseInsensitive = true,
                // 使用驼峰命名（与后端保持一致）
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }


        // ============= HTTP请求方法 =============

        /// <summary>
        /// 发送GET请求并获取响应
        /// </summary>
        /// <typeparam name="T">响应数据类型</typeparam>
        /// <param name="url">请求URL</param>
        /// <returns>API响应结果</returns>
        protected async Task<ApiResponse<T>> GetAsync<T>(string url)
        {
            try
            {
                // 创建HTTP请求消息
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);

                // 发送请求并获取响应
                HttpResponseMessage responseMessage = await this._httpClient.SendAsync(requestMessage);

                // 处理响应
                ApiResponse<T> apiResponse = await this.ProcessResponse<T>(responseMessage);

                return apiResponse;
            }
            catch (HttpRequestException httpException)
            {
                // 网络请求异常处理
                ApiResponse<T> apiResponse = this.CreateErrorResponse<T>(
                    $"网络请求失败: {httpException.Message}",
                    1000);

                return apiResponse;
            }
            catch (Exception exception)
            {
                // 其他异常处理
                ApiResponse<T> apiResponse = this.CreateErrorResponse<T>(
                    $"请求发生异常: {exception.Message}",
                    1000);

                return apiResponse;
            }
        }

        /// <summary>
        /// 发送POST请求并获取响应
        /// </summary>
        /// <typeparam name="T">响应数据类型</typeparam>
        /// <param name="url">请求URL</param>
        /// <param name="data">请求数据</param>
        /// <returns>API响应结果</returns>
        protected async Task<ApiResponse<T>> PostAsync<T>(string url, object data)
        {
            try
            {
                // 将数据序列化为JSON字符串
                string jsonContent = JsonSerializer.Serialize(data, this._jsonOptions);

                // 创建HTTP内容
                StringContent httpContent = new StringContent(
                    jsonContent,
                    Encoding.UTF8,
                    "application/json");

                // 发送请求并获取响应
                HttpResponseMessage responseMessage = await this._httpClient.PostAsync(url, httpContent);

                // 处理响应
                ApiResponse<T> apiResponse = await this.ProcessResponse<T>(responseMessage);

                return apiResponse;
            }
            catch (HttpRequestException httpException)
            {
                ApiResponse<T> apiResponse = this.CreateErrorResponse<T>(
                    $"网络请求失败: {httpException.Message}",
                    1000);

                return apiResponse;
            }
            catch (Exception exception)
            {
                ApiResponse<T> apiResponse = this.CreateErrorResponse<T>(
                    $"请求发生异常: {exception.Message}",
                    1000);

                return apiResponse;
            }
        }

        /// <summary>
        /// 发送PUT请求并获取响应
        /// </summary>
        /// <typeparam name="T">响应数据类型</typeparam>
        /// <param name="url">请求URL</param>
        /// <param name="data">请求数据</param>
        /// <returns>API响应结果</returns>
        protected async Task<ApiResponse<T>> PutAsync<T>(string url, object data)
        {
            try
            {
                // 将数据序列化为JSON字符串
                string jsonContent = JsonSerializer.Serialize(data, this._jsonOptions);

                // 创建HTTP内容
                StringContent httpContent = new StringContent(
                    jsonContent,
                    Encoding.UTF8,
                    "application/json");

                // 发送请求并获取响应
                HttpResponseMessage responseMessage = await this._httpClient.PutAsync(url, httpContent);

                // 处理响应
                ApiResponse<T> apiResponse = await this.ProcessResponse<T>(responseMessage);

                return apiResponse;
            }
            catch (HttpRequestException httpException)
            {
                ApiResponse<T> apiResponse = this.CreateErrorResponse<T>(
                    $"网络请求失败: {httpException.Message}",
                    1000);

                return apiResponse;
            }
            catch (Exception exception)
            {
                ApiResponse<T> apiResponse = this.CreateErrorResponse<T>(
                    $"请求发生异常: {exception.Message}",
                    1000);

                return apiResponse;
            }
        }

        /// <summary>
        /// 发送DELETE请求并获取响应
        /// </summary>
        /// <typeparam name="T">响应数据类型</typeparam>
        /// <param name="url">请求URL</param>
        /// <returns>API响应结果</returns>
        protected async Task<ApiResponse<T>> DeleteAsync<T>(string url)
        {
            try
            {
                // 发送请求并获取响应
                HttpResponseMessage responseMessage = await this._httpClient.DeleteAsync(url);

                // 处理响应
                ApiResponse<T> apiResponse = await this.ProcessResponse<T>(responseMessage);

                return apiResponse;
            }
            catch (HttpRequestException httpException)
            {
                ApiResponse<T> apiResponse = this.CreateErrorResponse<T>(
                    $"网络请求失败: {httpException.Message}",
                    1000);

                return apiResponse;
            }
            catch (Exception exception)
            {
                ApiResponse<T> apiResponse = this.CreateErrorResponse<T>(
                    $"请求发生异常: {exception.Message}",
                    1000);

                return apiResponse;
            }
        }



        // ============= 响应处理方法 =============

        /// <summary>
        /// 处理HTTP响应
        /// 将后端的ServiceResult<T>转换为前端的ApiResponse<T>
        /// </summary>
        /// <typeparam name="T">响应数据类型</typeparam>
        /// <param name="response">HTTP响应消息</param>
        /// <returns>API响应结果</returns>
        private async Task<ApiResponse<T>> ProcessResponse<T>(HttpResponseMessage response)
        {
            // 读取响应内容为字符串
            string responseContent = await response.Content.ReadAsStringAsync();

            // 尝试将响应内容反序列化为后端的ServiceResult<T>
            try
            {
                // 反序列化为后端ServiceResult<T>
                ApiResponse<T> serviceResult = JsonSerializer.Deserialize<ApiResponse<T>>(responseContent, this._jsonOptions);

                if (serviceResult != null)
                {
                    // 成功将ServiceResult<T>转换为ApiResponse<T>
                    ApiResponse<T> apiResponse = new ApiResponse<T>
                    {
                        IsSuccess = serviceResult.IsSuccess,
                        Data = serviceResult.Data,
                        ErrorCode = serviceResult.ErrorCode,
                        ErrorMessage = serviceResult.ErrorMessage,
                        Timestamp = serviceResult.Timestamp
                    };

                    return apiResponse;
                }
                else
                {
                    // 反序列化结果为null
                    ApiResponse<T> apiResponse = this.CreateErrorResponse<T>(
                        "服务器返回的数据格式不正确",
                        1001);

                    return apiResponse;
                }
            }
            catch (JsonException jsonException)
            {
                // JSON反序列化失败
                ApiResponse<T> apiResponse = this.CreateErrorResponse<T>(
                    $"JSON解析失败: {jsonException.Message}",
                    1001);

                return apiResponse;
            }
            catch (Exception exception)
            {
                // 其他异常
                ApiResponse<T> apiResponse = this.CreateErrorResponse<T>(
                    $"响应处理失败: {exception.Message}",
                    1000);

                return apiResponse;
            }
        }

        /// <summary>
        /// 创建错误响应
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="errorMessage">错误信息</param>
        /// <param name="errorCode">错误码</param>
        /// <returns>错误响应</returns>
        private ApiResponse<T> CreateErrorResponse<T>(string errorMessage, int errorCode)
        {
            ApiResponse<T> apiResponse = new ApiResponse<T>
            {
                IsSuccess = false,
                Data = default(T),
                ErrorCode = errorCode,
                ErrorMessage = errorMessage,
                Timestamp = DateTime.UtcNow
            };

            return apiResponse;
        }
    }
}
