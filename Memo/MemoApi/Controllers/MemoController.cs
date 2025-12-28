using MemoApi.Dtos.Memo;
using MemoApi.Dtos.ToDo;
using MemoApi.Results;
using MemoApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace MemoApi.Controllers
{
    // 待办事项控制器
    // 路径: Api/Memo/...

    [ApiController]
    [Route("Api/Memo")]
    public class MemoController : ApiControllerBase
    {
        // 依赖注入服务层对象
        private readonly IMemoService _memoService;

        // 构造函数注入
        public MemoController(IMemoService memoService)
        {
            this._memoService = memoService;
        }


        // 根据ID查询对应的备忘录
        [HttpGet("GetMemoById/{id}")]
        public async Task<ActionResult<MemoDto>> GetMemoId(int id)
        {
            // 调用服务层根据ID查询备忘录并接收返回值
            ServiceResult<MemoDto> getMemoByIdResult = await this._memoService.GetMemoByIdAsync(id);
            return HandleServiceResult<MemoDto>(getMemoByIdResult);
        }

        // 查询所有的备忘录
        [HttpGet("GetMemoAll")]
        public async Task<ActionResult<List<MemoDto>>> GetMemoAll()
        {
            // 调用服务层获取所有备忘录并接收返回值
            ServiceResult<List<MemoDto>> getMemoAllResult = await this._memoService.GetMemoAllAsync();
            return HandleServiceResult<List<MemoDto>>(getMemoAllResult);
        }

        // 根据条件查询备忘录
        [HttpGet("GetMemoByCondition")]
        public async Task<ActionResult<List<MemoDto>>> GetMemoByCondition([FromQuery] string? memoTitle = null)
        {
            ServiceResult<List<MemoDto>> getMemoResultByCondition = await this._memoService.GetMemoByConditionAsync(memoTitle);
            return HandleServiceResult<List<MemoDto>>(getMemoResultByCondition);
        }

        // 修改相应备忘录
        [HttpPut("UpdateMemo")]
        public async Task<ActionResult<MemoDto>> UpdateMemo([FromBody] UpdateMemoDto updateMemoDto)
        {
            // 调用服务层修改相应的备忘录
            ServiceResult<MemoDto> updateMemoResult = await this._memoService.UpdateMemoAsync(updateMemoDto);
            return HandleServiceResult<MemoDto>(updateMemoResult);
        }

        // 添加备忘录
        [HttpPost("AddMemo")]
        public async Task<ActionResult<MemoDto>> AddMemo([FromBody] CreateMemoDto createMemoDto)
        {
            // 调用服务层添加备忘录
            ServiceResult<MemoDto> addMemoResult = await this._memoService.AddMemoAsync(createMemoDto);
            return HandleServiceResult<MemoDto>(addMemoResult);
        }

        // 删除相应备忘录
        [HttpDelete("DeleteMemo/{id}")]
        public async Task<ActionResult<int>> DeleteMemo(int id)
        {
            // 调用服务层删除相应备忘录
            ServiceResult<int> deleteMemoResult = await this._memoService.DeleteMemoAsync(id);
            return HandleServiceResult<int>(deleteMemoResult);
        }
    }
}
/*
特性          参数来源	    HTTP方法	使用场景
[FromQuery]   URL查询字符串 GET	搜索、  过滤、分页
[FromBody]    请求体        POST/PUT	创建、更新数据
[FromRoute]   URL路径       任意	    获取特定资源
[FromForm]    表单数据      POST	    表单提交
*/
