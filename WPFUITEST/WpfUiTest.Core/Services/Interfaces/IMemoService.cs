using System;
using System.Collections.Generic;
using System.Text;
using WpfUiTest.Core.DTOs.Memo;
using WpfUiTest.Core.DTOs.ToDo;
using WpfUiTest.Shared.Utilities;

namespace WpfUiTest.Core.Services.Interfaces
{
    // 备忘录服务接口
    public interface IMemoService
    {
        // 以下方法均为用户隔离

        // 查询所有备忘录
        public Task<ServiceResult<List<MemoDto>>> GetAllMemosAsync();

        // 根据条件分页查询备忘录
        public Task<ServiceResult<PagedResult<MemoDto>>> GetPagedMemosAsync(PagedQueryMemoDto pagedQueryMemoDto);

        // 添加备忘录
        public Task<ServiceResult<MemoDto>> AddMemoAsync(AddMemoDto addMemoDto);

        // 修改备忘录
        public Task<ServiceResult<MemoDto>> UpdateMemoAsync(UpdateMemoDto updateMemoDto);

        // 删除备忘录
        public Task<ServiceResult<bool>> DeleteMemoAsync(DeleteMemoDto deleteMemoDto);
    }
}
