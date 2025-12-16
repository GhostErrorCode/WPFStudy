using MemoApi.Enums;
using MemoApi.Results;
using Microsoft.AspNetCore.Mvc;

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
                return Ok(result);
            }

            // 失败：根据错误码返回对应的HTTP状态码
            return ConvertErrorToActionResult<T>(result);
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
                return CreatedAtAction(actionName, routeValues, result);
            }

            // 失败：返回错误
            return ConvertErrorToActionResult<T>(result);
        }

        // ============= 错误处理方法 =============

        /// <summary>
        /// 将业务错误码转换为ActionResult<T>
        /// 内部方法，用于错误处理
        /// </summary>
        private ActionResult<T> ConvertErrorToActionResult<T>(ServiceResult<T> result)
        {
            // 根据错误码的第一位数字分类
            int errorCategory = result.ErrorCode / 1000;

            switch (errorCategory)
            {
                case 1: // 1xxx 系统级错误
                    return StatusCode(500, result);

                case 2: // 2xxx 参数校验错误
                    return BadRequest(result);

                case 3: // 3xxx 业务逻辑错误
                    return HandleBusinessError<T>(result);

                case 4: // 4xxx 数据库操作错误
                    return StatusCode(500, result);

                case 5: // 5xxx 权限相关错误
                    return HandlePermissionError<T>(result);

                case 7: // 7xxx 文件操作错误
                    return BadRequest(result);

                case 9: // 9xxx 自定义业务错误
                    return BadRequest(result);

                default:
                    return StatusCode(500, result);
            }
        }

        /// <summary>
        /// 处理业务逻辑错误（3xxx）
        /// </summary>
        private ActionResult<T> HandleBusinessError<T>(ServiceResult<T> result)
        {
            switch (result.ErrorCode)
            {
                case ErrorCodes.DataNotFound:
                    // 数据不存在：返回404
                    return NotFound(result);

                case ErrorCodes.DataAlreadyExists:
                    // 数据已存在：返回409
                    return Conflict(result);

                case ErrorCodes.OperationPermissionDenied:
                    // 操作权限不足：返回403
                    return StatusCode(403, result);

                case ErrorCodes.DataStatusAbnormal:
                    // 数据状态异常：返回400
                    return BadRequest(result);

                default:
                    // 默认返回400
                    return BadRequest(result);
            }
        }

        /// <summary>
        /// 处理权限相关错误（5xxx）
        /// </summary>
        private ActionResult<T> HandlePermissionError<T>(ServiceResult<T> result)
        {
            switch (result.ErrorCode)
            {
                case ErrorCodes.NotLoggedInOrTokenExpired:
                    // 未登录/Token过期：返回401
                    return Unauthorized(result);

                case ErrorCodes.RolePermissionInsufficient:
                    // 操作权限不足：返回403
                    return StatusCode(403, result);

                default:
                    return Unauthorized(result);
            }
        }
    }
}
