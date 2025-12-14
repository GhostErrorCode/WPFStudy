using MemoApi.Dtos.Memo;
using MemoApi.Dtos.ToDo;
using MemoApi.Entities;
using MemoApi.Enums;
using MemoApi.Mappings;
using MemoApi.Repositories;
using MemoApi.Results;
using MemoApi.UnitOfWork;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MemoApi.Services.Implements
{
    // 待办事项服务层，继承对应接口
    public class ToDoService : IToDoService
    {
        // 私有字段：待办事项仓储接口
        // 作用：通过仓储访问待办事项数据，服务层不直接操作数据库
        private readonly IToDoRepository _toDoRepository;

        // 私有字段：工作单元
        // 作用：协调多个仓储操作，管理事务
        private readonly IUnitOfWork _unitOfWork;

        // 构造函数 - 获取工作单元
        // 构造函数：依赖注入
        // 参数：
        //   - productRepository: IProductRepository - 产品具体仓储
        //   - unitOfWork: IUnitOfWork - 工作单元
        // 设计模式：构造函数注入，这是最推荐的依赖注入方式
        public ToDoService(IToDoRepository toDoRepository, IUnitOfWork unitOfWork)
        {
            // 赋值给字段
            // this关键字：明确引用当前实例的字段，避免与参数混淆
            this._toDoRepository = toDoRepository;
            this._unitOfWork = unitOfWork;
        }


        // 根据ID查询待办事项
        public async Task<ServiceResult<ToDoDto>> GetToDoByIdAsync(int id)
        {
            try
            {
                // 查询对应ID的待办事项 - 可为NULL
                ToDo? toDo = await this._toDoRepository.GetByIdAsync(id);
                // 如果返回的实体不为NULL,则表示查询成功
                if (toDo != null)
                {
                    // 添加映射：将ToDo实体转换为ToDoDto
                    ToDoDto toDoDto = ToDoMappings.ConvertToDoEntityToToDoDto(toDo);
                    return ServiceResult<ToDoDto>.Success(toDoDto);
                }
                // 返回查询失败结果
                return ServiceResult<ToDoDto>.Fail(ErrorCodes.DataNotFound, $"查询待办事项失败,未找到ID为 {id} 的待办事项");
            }
            catch (Exception ex)
            {
                // 执行发生内部异常处理
                return ServiceResult<ToDoDto>.Fail($"查询ID为 {id} 的待办事项失败,异常信息: {ex.Message}");
            }
        }

        // 查询所有待办事项
        public async Task<ServiceResult<List<ToDoDto>>> GetToDoAllAsync()
        {
            try
            {
                // 查询所有待办事项
                List<ToDo> toDos = await this._toDoRepository.GetAllAsync();
                // 添加映射：将List<ToDo>转换为List<ToDoDto>
                List<ToDoDto> toDoDtos = ToDoMappings.ConvertToDoEntityCollectionToToDoDtoCollection(toDos);
                // 返回结果和数据
                return ServiceResult<List<ToDoDto>>.Success(toDoDtos);
            }
            catch (Exception ex)
            {
                // 执行发生内部异常处理
                return ServiceResult<List<ToDoDto>>.Fail($"查询所有待办事项失败,异常信息: {ex.Message}");
            }
        }

        // 更新待办事项
        public async Task<ServiceResult<ToDoDto>> UpdateToDoAsync(UpdateToDoDto updateToDoDto)
        {
            // Task.FromResult	方法中没有异步操作，但要保持接口一致性
            // 如果传入的待办事项实体为NULL,则返回失败
            if (updateToDoDto == null) return ServiceResult<ToDoDto>.Fail(ErrorCodes.ParameterNull, "修改的待办事项为空(NULL)");
            try
            {
                // 如果传入的待办事项实体不为NULL,则查询数据库中是否存在
                ToDo? updateToDo = await this._toDoRepository.GetByIdAsync(updateToDoDto.Id);
                // 如果数据库中不存在此记录，则返回未找到
                if (updateToDo == null) return ServiceResult<ToDoDto>.Fail(ErrorCodes.DataNotFound, $"修改待办事项失败,未找到ID为 {updateToDoDto.Id} 的待办事项");
                // 修改部分：使用映射方法更新实体，而不是直接赋值
                // 原有逻辑：updateToDo.Title = toDo.Title;
                // 改为使用映射：
                ToDoMappings.UpdateToDoEntityFromUpdateToDoDto(updateToDo, updateToDoDto);
                // 提交
                this._toDoRepository.Update(updateToDo);
                // 提交更改，判断是否成功
                if(await this._unitOfWork.SaveChangesAsync() > 0)
                {
                    // 成功时返回映射后的DTO
                    ToDoDto resultDto = ToDoMappings.ConvertToDoEntityToToDoDto(updateToDo);
                    return ServiceResult<ToDoDto>.Success(resultDto);
                }
                // 最后就是失败了
                return ServiceResult<ToDoDto>.Fail(ErrorCodes.DataUpdateFailed, $"修改待办事项 {updateToDoDto.Id} 失败");
            }
            catch (Exception ex)
            {
                // 执行发生内部异常处理
                return ServiceResult<ToDoDto>.Fail($"修改待办事项失败,异常信息: {ex.Message}");
            }
        }

        // 添加待办事项
        public async Task<ServiceResult<ToDoDto>> AddToDoAsync(CreateToDoDto createToDoDto)
        {
            // Task.FromResult	方法中没有异步操作，但要保持接口一致性
            // 如果传入的待办事项实体为NULL,则返回失败
            if (createToDoDto == null) return ServiceResult<ToDoDto>.Fail(ErrorCodes.ParameterNull, "添加的待办事项为空(NULL)");
            try
            {
                // 添加映射：将CreateToDoDto转换为ToDo实体
                ToDo createToDo = ToDoMappings.ConvertCreateToDoDtoToToDoEntity(createToDoDto);
                // 如果传入的待办事项实体不为NULL,则暂存到事务中
                await this._toDoRepository.AddAsync(createToDo);
                // 提交至数据库中，并判断是否成功 - 大于0
                if (await this._unitOfWork.SaveChangesAsync() > 0)
                {
                    // 成功时返回映射后的DTO
                    ToDoDto resultToDoDto = ToDoMappings.ConvertToDoEntityToToDoDto(createToDo);
                    return ServiceResult<ToDoDto>.Success(resultToDoDto);
                }
                // 如果添加失败
                return ServiceResult<ToDoDto>.Fail(ErrorCodes.DataInsertFailed, $"添加待办事项失败 标题: {createToDoDto.Title}");
            }
            catch(Exception ex)
            {
                // 执行发生内部异常处理
                return ServiceResult<ToDoDto>.Fail($"添加待办事项失败,异常信息: {ex.Message}");
            }
        }

        // 删除待办事项
        public async Task<ServiceResult<int>> DeleteToDoAsync(int id)
        {
            try
            {
                // 先判断数据库中是否存在此ID的记录
                bool existsToDo = await this._toDoRepository.ExistsAsync((ToDo toDo) => toDo.Id.Equals(id));
                // 如果此ID记录不存在
                if (!existsToDo) return ServiceResult<int>.Fail(ErrorCodes.DataDeleteFailed, $"删除待办事项失败,未找到ID为 {id} 的待办事项");
                // 进行删除
                await this._toDoRepository.DeleteAsync(id);
                // 如果删除的记录数量大于0则表示成功删除
                int deleteResult = await this._unitOfWork.SaveChangesAsync();
                if (deleteResult > 0) return ServiceResult<int>.Success(deleteResult);
                // 删除未成功
                return ServiceResult<int>.Fail(ErrorCodes.DataDeleteFailed, $"删除待办事项失败");
            }
            catch(Exception ex)
            {
                // 执行发生内部异常处理
                return ServiceResult<int>.Fail($"删除待办事项失败,异常信息: {ex.Message}");
            }
        }
    }
}
