using MemoApi.Dtos.Dashboard;
using MemoApi.Dtos.ToDo;
using MemoApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MemoApi.Controllers
{
    // 数据汇总控制器
    // 路径: Api/Summary/...

    [ApiController]
    [Route("Api/Summary")]
    public class SummaryController : ApiControllerBase
    {
        // 依赖注入服务层对象
        private readonly ISummaryService _summaryService;

        // 构造函数注入
        public SummaryController(ISummaryService summaryService)
        {
            this._summaryService = summaryService;
        }

        // 首页的数据汇总
        [HttpGet("GetSummary")]
        public async Task<ActionResult<SummaryDto>> GetSummary()
        {
            return HandleServiceResult<SummaryDto>(await this._summaryService.GetSummaryAsync());
        }
    }
}
