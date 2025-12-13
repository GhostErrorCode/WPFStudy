using MemoApi.Entities;
using MemoApi.Enums;
using MemoApi.Repositories;
using MemoApi.Results;
using MemoApi.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MemoApi.Controllers
{
    /// <summary>
    /// API控制器基类
    /// 提供ServiceResult到ActionResult的转换功能
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public abstract class ApiControllerBase : ControllerBase
    {
        // ============= 核心方法 =============

        /// <summary>
        /// 处理有数据返回的ServiceResult
        /// 使用场景：查询、创建返回ID等需要返回数据的操作
        /// </summary>
        protected ActionResult<T> HandleServiceResult<T>(ServiceResult<T> result)
        {
            if (result.IsSuccess)
            {
                // 成功：返回200 OK + 数据
                return Ok(result.Data);
            }

            // 失败：根据错误码返回对应的HTTP状态码
            return ConvertErrorToActionResult<T>(result.ErrorCode, result.ErrorMessage);
        }

        /// <summary>
        /// 处理创建操作，返回201 Created状态
        /// 使用场景：创建资源成功
        /// </summary>
        protected ActionResult<T> HandleCreatedResult<T>(string actionName, object routeValues, ServiceResult<T> result)
        {
            if (result.IsSuccess)
            {
                // 成功：返回201 Created + Location头
                return CreatedAtAction(actionName, routeValues, result.Data);
            }

            // 失败：返回错误
            return ConvertErrorToActionResult<T>(result.ErrorCode, result.ErrorMessage);
        }

        // ============= 错误处理方法 =============

        /// <summary>
        /// 将业务错误码转换为ActionResult<T>
        /// 内部方法，用于错误处理
        /// </summary>
        private ActionResult<T> ConvertErrorToActionResult<T>(int errorCode, string errorMessage)
        {
            // 根据错误码的第一位数字分类
            int errorCategory = errorCode / 1000;

            switch (errorCategory)
            {
                case 1: // 1xxx 系统级错误
                    return StatusCode(500, new { errorCode, errorMessage });

                case 2: // 2xxx 参数校验错误
                    return BadRequest(new { errorCode, errorMessage });

                case 3: // 3xxx 业务逻辑错误
                    return HandleBusinessError<T>(errorCode, errorMessage);

                case 4: // 4xxx 数据库操作错误
                    return StatusCode(500, new { errorCode, errorMessage });

                case 5: // 5xxx 权限相关错误
                    return HandlePermissionError<T>(errorCode, errorMessage);

                case 7: // 7xxx 文件操作错误
                    return BadRequest(new { errorCode, errorMessage });

                case 9: // 9xxx 自定义业务错误
                    return BadRequest(new { errorCode, errorMessage });

                default:
                    return StatusCode(500, new { errorCode, errorMessage });
            }
        }

        /// <summary>
        /// 处理业务逻辑错误（3xxx）
        /// </summary>
        private ActionResult<T> HandleBusinessError<T>(int errorCode, string errorMessage)
        {
            switch (errorCode)
            {
                case ErrorCodes.DataNotFound:
                    // 数据不存在：返回404
                    return NotFound(new { errorCode, errorMessage });

                case ErrorCodes.DataAlreadyExists:
                    // 数据已存在：返回409
                    return Conflict(new { errorCode, errorMessage });

                case ErrorCodes.OperationPermissionDenied:
                    // 操作权限不足：返回403
                    // Forbid()没有泛型版本，需要特殊处理
                    return Forbid() as ActionResult<T> ??
                           StatusCode(403, new { errorCode, errorMessage });

                case ErrorCodes.DataStatusAbnormal:
                    // 数据状态异常：返回400
                    return BadRequest(new { errorCode, errorMessage });

                default:
                    // 默认返回400
                    return BadRequest(new { errorCode, errorMessage });
            }
        }

        /// <summary>
        /// 处理权限相关错误（5xxx）
        /// </summary>
        private ActionResult<T> HandlePermissionError<T>(int errorCode, string errorMessage)
        {
            switch (errorCode)
            {
                case ErrorCodes.NotLoggedInOrTokenExpired:
                    // 未登录/Token过期：返回401
                    return Unauthorized(new { errorCode, errorMessage });

                case ErrorCodes.RolePermissionInsufficient:
                    // 角色权限不足：返回403
                    return Forbid() as ActionResult<T> ??
                           StatusCode(403, new { errorCode, errorMessage });

                default:
                    return Unauthorized(new { errorCode, errorMessage });
            }
        }
    }
}
