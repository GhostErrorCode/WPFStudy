using MemoApi.Dtos.Memo;
using MemoApi.Dtos.ToDo;
using MemoApi.Entities;
using MemoApi.Enums;
using MemoApi.Mappings;
using MemoApi.Repositories;
using MemoApi.Results;
using MemoApi.UnitOfWork;

namespace MemoApi.Services.Implements
{
    // 备忘录服务层，继承对应接口
    public class MemoService : IMemoService
    {
        // 私有字段：备忘录仓储接口
        // 作用：通过仓储访问备忘录数据，服务层不直接操作数据库
        private readonly IMemoRepository _memoRepository;

        // 私有字段：工作单元
        // 作用：协调多个仓储操作，管理事务
        private readonly IUnitOfWork _unitOfWork;

        // 构造函数 - 获取工作单元
        // 构造函数：依赖注入
        // 参数：
        //   - memoRepository: IMemoRepository - 产品具体仓储
        //   - unitOfWork: IUnitOfWork - 工作单元
        // 设计模式：构造函数注入，这是最推荐的依赖注入方式
        public MemoService(IMemoRepository memoRepository, IUnitOfWork unitOfWork)
        {
            // 赋值给字段
            // this关键字：明确引用当前实例的字段，避免与参数混淆
            this._memoRepository = memoRepository;
            this._unitOfWork = unitOfWork;
        }


        // 根据ID查询备忘录
        public async Task<ServiceResult<MemoDto>> GetMemoByIdAsync(int id)
        {
            try
            {
                // 查询对应ID的备忘录 - 可为NULL
                Memo? memo = await this._memoRepository.GetByIdAsync(id);
                // 如果返回的实体不为NULL,则表示查询成功
                if (memo != null)
                {
                    // 添加映射：将 Memo 实体转换为 MemoDto
                    MemoDto memoDto = MemoMappings.ConvertMemoEntityToMemoDto(memo);
                    return ServiceResult<MemoDto>.Success(memoDto);
                }
                // 返回查询失败结果
                return ServiceResult<MemoDto>.Fail(ErrorCodes.DataNotFound, $"查询备忘录失败,未找到ID为 {id} 的备忘录");
            }
            catch (Exception ex)
            {
                // 执行发生内部异常处理
                return ServiceResult<MemoDto>.Fail($"查询ID为 {id} 的备忘录失败,异常信息: {ex.Message}");
            }
        }


        // 查询所有备忘录
        public async Task<ServiceResult<List<MemoDto>>> GetMemoAllAsync()
        {
            try
            {
                // 查询所有备忘录
                List<Memo> memos = await this._memoRepository.GetAllAsync();
                // 添加映射：将List<Memo>转换为List<MemoDto>
                List<MemoDto> memoDtos = MemoMappings.ConvertMemoEntityCollectionToMemoDtoCollection(memos);
                // 返回结果和数据
                return ServiceResult<List<MemoDto>>.Success(memoDtos);
            }
            catch (Exception ex)
            {
                // 执行发生内部异常处理
                return ServiceResult<List<MemoDto>>.Fail($"查询所有备忘录失败,异常信息: {ex.Message}");
            }
        }


        // 更新备忘录
        public async Task<ServiceResult<MemoDto>> UpdateMemoAsync(UpdateMemoDto updateMemoDto)
        {
            // Task.FromResult	方法中没有异步操作，但要保持接口一致性
            // 如果传入的备忘录实体为NULL,则返回失败
            if (updateMemoDto == null) return ServiceResult<MemoDto>.Fail(ErrorCodes.ParameterNull, "修改的备忘录为空(NULL)");
            try
            {
                // 如果传入的备忘录实体不为NULL,则查询数据库中是否存在
                Memo? updateMemo = await this._memoRepository.GetByIdAsync(updateMemoDto.Id);
                // 如果数据库中不存在此记录，则返回未找到
                if (updateMemo == null) return ServiceResult<MemoDto>.Fail(ErrorCodes.DataNotFound, $"修改备忘录失败,未找到ID为 {updateMemoDto.Id} 的备忘录");
                // 修改部分：使用映射方法更新实体，而不是直接赋值
                // 原有逻辑：updateMemo.Title = memo.Title;
                // 改为使用映射：
                MemoMappings.UpdateMemoEntityFromUpdateMemoDto(updateMemo, updateMemoDto);
                // 提交
                this._memoRepository.Update(updateMemo);
                // 提交更改，判断是否成功
                if (await this._unitOfWork.SaveChangesAsync() > 0)
                {
                    // 成功时返回映射后的DTO
                    MemoDto resultDto = MemoMappings.ConvertMemoEntityToMemoDto(updateMemo);
                    return ServiceResult<MemoDto>.Success(resultDto);
                }
                // 最后就是失败了
                return ServiceResult<MemoDto>.Fail(ErrorCodes.DataUpdateFailed, $"修改备忘录 {updateMemoDto.Id} 失败");
            }
            catch (Exception ex)
            {
                // 执行发生内部异常处理
                return ServiceResult<MemoDto>.Fail($"修改备忘录失败,异常信息: {ex.Message}");
            }
        }


        // 添加备忘录
        public async Task<ServiceResult<MemoDto>> AddMemoAsync(CreateMemoDto createMemoDto)
        {
            // Task.FromResult	方法中没有异步操作，但要保持接口一致性
            // 如果传入的待办事项实体为NULL,则返回失败
            if (createMemoDto == null) return ServiceResult<MemoDto>.Fail(ErrorCodes.ParameterNull, "添加的备忘录为空(NULL)");
            try
            {
                // 添加映射：将 createMemoDto 转换为 Memo 实体
                Memo createMemo = MemoMappings.ConvertCreateToDoDtoToToDoEntity(createMemoDto);
                // 如果传入的待办事项实体不为NULL,则暂存到事务中
                await this._memoRepository.AddAsync(createMemo);
                // 提交至数据库中，并判断是否成功 - 大于0
                if (await this._unitOfWork.SaveChangesAsync() > 0)
                {
                    // 成功时返回映射后的DTO
                    MemoDto resultMemoDto = MemoMappings.ConvertMemoEntityToMemoDto(createMemo);
                    return ServiceResult<MemoDto>.Success(resultMemoDto);
                }
                // 如果添加失败
                return ServiceResult<MemoDto>.Fail(ErrorCodes.DataInsertFailed, $"添加备忘录失败 标题: {createMemoDto.Title}");
            }
            catch (Exception ex)
            {
                // 执行发生内部异常处理
                return ServiceResult<MemoDto>.Fail($"添加备忘录失败,异常信息: {ex.Message}");
            }
        }


        // 删除备忘录
        public async Task<ServiceResult<int>> DeleteMemoAsync(int id)
        {
            try
            {
                // 先判断数据库中是否存在此ID的记录
                bool existsMemo = await this._memoRepository.ExistsAsync((Memo memo) => memo.Id.Equals(id));
                // 如果此ID记录不存在
                if (!existsMemo) return ServiceResult<int>.Fail(ErrorCodes.DataDeleteFailed, $"删除备忘录失败,未找到ID为 {id} 的待办事项");
                // 进行删除
                await this._memoRepository.DeleteAsync(id);
                // 如果删除的记录数量大于0则表示成功删除
                int deleteResult = await this._unitOfWork.SaveChangesAsync();
                if (deleteResult > 0) return ServiceResult<int>.Success(deleteResult);
                // 删除未成功
                return ServiceResult<int>.Fail(ErrorCodes.DataDeleteFailed, $"删除备忘录失败");
            }
            catch (Exception ex)
            {
                // 执行发生内部异常处理
                return ServiceResult<int>.Fail($"删除备忘录失败,异常信息: {ex.Message}");
            }
        }
    }
}
