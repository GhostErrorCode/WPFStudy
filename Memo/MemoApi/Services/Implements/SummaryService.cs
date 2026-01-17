using MemoApi.Dtos.Dashboard;
using MemoApi.Dtos.ToDo;
using MemoApi.Entities;
using MemoApi.Mappings;
using MemoApi.Repositories;
using MemoApi.Results;
using MemoApi.UnitOfWork;
using System.Linq.Expressions;

namespace MemoApi.Services.Implements
{
    // 数据汇总服务层
    public class SummaryService : ISummaryService
    {
        // 私有字段：待办事项仓储接口
        // 作用：通过仓储访问待办事项数据，服务层不直接操作数据库
        private readonly IToDoRepository _toDoRepository;

        // 私有字段：备忘录仓储接口
        // 作用：通过仓储访问备忘录数据，服务层不直接操作数据库
        private readonly IMemoRepository _memoRepository;

        // 私有字段：工作单元
        // 作用：协调多个仓储操作，管理事务
        private readonly IUnitOfWork _unitOfWork;

        // 构造函数 - 获取工作单元
        // 构造函数：依赖注入
        // 参数：
        //   - productRepository: IProductRepository - 产品具体仓储
        //   - unitOfWork: IUnitOfWork - 工作单元
        // 设计模式：构造函数注入，这是最推荐的依赖注入方式
        public SummaryService(IToDoRepository toDoRepository, IMemoRepository memoRepository, IUnitOfWork unitOfWork)
        {
            // 赋值给字段
            // this关键字：明确引用当前实例的字段，避免与参数混淆
            this._toDoRepository = toDoRepository;
            this._memoRepository = memoRepository;
            this._unitOfWork = unitOfWork;
        }


        // 数据汇总，统计待办事项总量、待办事项完成数量、待办事项完成比例、备忘录总数
        public async Task<ServiceResult<SummaryDto>> GetSummaryAsync()
        {
            try
            {
                /* 由于将Dto转为Record，所以代码重写
                // 初始化SummaryDto
                SummaryDto summaryDto = new SummaryDto();

                // 查询待办事项总量
                summaryDto.ToDoTotalCount = await this._toDoRepository.CountAsync();
                // 查询待办事项完成数量
                Expression<Func<ToDo, bool>> CountByToDoCompletedPredicate = (ToDo toDo) => toDo.Status == 1;
                summaryDto.ToDoCompletedCount = await this._toDoRepository.CountAsync(CountByToDoCompletedPredicate);
                // 计算待办事项完成比例
                //(double)completed / total - 强制转换为浮点数
                //* 100 - 转换为百分比值
                //:F1 - 保留1位小数
                //%" - 添加百分号
                //total > 0 ? ... : "0%" - 避免除零错误
                summaryDto.ToDoCompletedProportion = $"{(double)summaryDto.ToDoCompletedCount / summaryDto.ToDoTotalCount * 100:F2}%";
                // 查询备忘录总量
                summaryDto.MemoTotalCount = await this._memoRepository.CountAsync();
                // 查询待办事项待办状态的列表
                Expression<Func<ToDo, bool>> ToDoListByToDoIncompletePredicate = (ToDo toDo) => toDo.Status == 0;
                summaryDto.ToDoList = ToDoMappings.ConvertToDoEntityCollectionToToDoDtoCollection(await this._toDoRepository.FindAsync(ToDoListByToDoIncompletePredicate));
                // 查询备忘录列表
                summaryDto.MemoList = MemoMappings.ConvertMemoEntityCollectionToMemoDtoCollection(await this._memoRepository.GetAllAsync());
                */


                // 查询待办事项完成数量
                Expression<Func<ToDo, bool>> CountByToDoCompletedPredicate = (ToDo toDo) => toDo.Status == 1;
                int ToDoTotalCount = await this._toDoRepository.CountAsync();
                // 查询待办事项待办状态的列表
                Expression<Func<ToDo, bool>> ToDoListByToDoIncompletePredicate = (ToDo toDo) => toDo.Status == 0;
                int ToDoCompletedCount = await this._toDoRepository.CountAsync(CountByToDoCompletedPredicate);

                // 定义Dto
                SummaryDto summaryDto = new SummaryDto(
                    ToDoTotalCount: ToDoTotalCount,
                    ToDoCompletedCount: ToDoCompletedCount,
                    ToDoCompletedProportion: $"{(double)ToDoCompletedCount / ToDoTotalCount * 100:F2}%",
                    MemoTotalCount: await this._memoRepository.CountAsync(),
                    ToDoList: ToDoMappings.ConvertToDoEntityCollectionToToDoDtoCollection(await this._toDoRepository.FindAsync(ToDoListByToDoIncompletePredicate)),
                    MemoList: MemoMappings.ConvertMemoEntityCollectionToMemoDtoCollection(await this._memoRepository.GetAllAsync())
                    );

                // 返回成功的结果
                return ServiceResult<SummaryDto>.Success(summaryDto);
            }
            catch(Exception ex)
            {
                // 执行发生内部异常处理
                return ServiceResult<SummaryDto>.Fail($"首页数据汇总时异常,异常信息: {ex.Message}");
            }
        }
    }
}
