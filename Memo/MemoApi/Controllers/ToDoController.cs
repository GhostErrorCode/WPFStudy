using MemoApi.Dtos.ToDo;
using MemoApi.Entities;
using MemoApi.Results;
using MemoApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MemoApi.Controllers
{
    // 待办事项控制器
    // 路径: Api/ToDo/...

    [ApiController]
    [Route("Api/ToDo")]
    public class ToDoController : ApiControllerBase
    {
        // 依赖注入服务层对象
        private readonly IToDoService _toDoService;

        // 构造函数注入
        public ToDoController(IToDoService toDoService)
        {
            this._toDoService = toDoService;
        }

        // 根据ID查询对应的待办事项
        [HttpGet("GetToDoById/{id}")]
        public async Task<ActionResult<ToDoDto>> GetToDoById(int id)
        {
            return HandleServiceResult<ToDoDto>(await this._toDoService.GetToDoByIdAsync(id));
        }

        // 查询所有的待办事项
        [HttpGet("GetToDoAll")]
        public async Task<ActionResult<List<ToDoDto>>> GetToDoAll()
        {
            ServiceResult<List<ToDoDto>> toDoDtos = await this._toDoService.GetToDoAllAsync();
            return HandleServiceResult<List<ToDoDto>>(toDoDtos);
        }

        // 修改相应待办事项
        [HttpPut("UpdateToDo")]
        public async Task<ActionResult<ToDoDto>> UpdateToDo([FromBody] UpdateToDoDto updateToDoDto)
        {
            return HandleServiceResult<ToDoDto>(await this._toDoService.UpdateToDoAsync(updateToDoDto));
        }

        // 添加待办事项
        [HttpPost("AddToDo")]
        public async Task<ActionResult<ToDoDto>> AddToDo([FromBody] CreateToDoDto createToDoDto)
        {
            return HandleServiceResult<ToDoDto>(await this._toDoService.AddToDoAsync(createToDoDto));
        }

        // 删除相应待办事项
        [HttpDelete("DeleteToDo/{id}")]
        public async Task<ActionResult<int>> DeleteToDo(int id)
        {
            return HandleServiceResult<int>(await this._toDoService.DeleteToDoAsync(id));
        }
    }
}



/*
总结表格：参数从哪里获取
操作	      HTTP方法	ID来源	  数据来源	        示例URL
查询单个	  GET	    URL路径	  -	                GET /api/todo/5
查询所有	  GET	    -	      查询参数（可选）	GET /api/todo
查询带条件	  GET	    -	      查询参数	        GET /api/todo?status=active
创建	      POST	    -	      Body（JSON）	    POST /api/todo
更新（全量）  PUT	    URL路径	  Body（JSON）	    PUT /api/todo/5
更新（部分）  PATCH	    URL路径	  Body（JSON）	    PATCH /api/todo/5
删除	      DELETE	URL路径	  -	                DELETE /api/todo/5
*/