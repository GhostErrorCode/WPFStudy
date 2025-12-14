using MemoApi.Dtos.Memo;
using MemoApi.Dtos.ToDo;
using MemoApi.Results;

namespace MemoApi.Services
{
    public interface IMemoService
    {
        // ========== 备忘录服务层接口 ==========

        // 根据ID查询备忘录
        public Task<ServiceResult<MemoDto>> GetMemoByIdAsync(int id);

        // 查询所有备忘录
        public Task<ServiceResult<List<MemoDto>>> GetMemoAllAsync();

        // 更新备忘录
        public Task<ServiceResult<MemoDto>> UpdateMemoAsync(UpdateMemoDto updateMemoDto);

        // 添加备忘录
        public Task<ServiceResult<MemoDto>> AddMemoAsync(CreateMemoDto createMemoDto);

        // 删除备忘录
        public Task<ServiceResult<int>> DeleteMemoAsync(int id);
    }
}
