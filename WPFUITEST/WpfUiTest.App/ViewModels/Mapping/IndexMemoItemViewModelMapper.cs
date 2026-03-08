using System;
using System.Collections.Generic;
using System.Text;
using WpfUiTest.App.ViewModels.Main;
using WpfUiTest.Core.DTOs.Memo;
using WpfUiTest.Core.DTOs.ToDo;

namespace WpfUiTest.App.ViewModels.Mapping
{
    // Dto 转 VM 扩展方法（备忘录）
    public static class IndexMemoItemViewModelMapper
    {
        // MemoDto 转 IndexMemoItemViewModel
        public static IndexMemoItemViewModel ToIndexMemoItemViewModel(this MemoDto memoDto)
        {
            return new IndexMemoItemViewModel()
            {
                Id = memoDto.Id,
                UserId = memoDto.UserId,
                Title = memoDto.Title,
                Content = memoDto.Content,
                CreateDate = memoDto.CreateDate,
                UpdateDate = memoDto.UpdateDate
            };
        }

        // IndexMemoItemViewModel 转 AddMemoDto
        public static AddMemoDto ToAddMemoDto(this IndexMemoItemViewModel indexMemoItemViewModel)
        {
            return new AddMemoDto()
            {
                Title = indexMemoItemViewModel.Title,
                Content = indexMemoItemViewModel.Content
            };
        }

        // IndexMemoItemViewModel 转 UpdateMemoDto
        public static UpdateMemoDto ToUpdateMemoDto(this IndexMemoItemViewModel indexMemoItemViewModel)
        {
            return new UpdateMemoDto()
            {
                Id = indexMemoItemViewModel.Id,
                UserId = indexMemoItemViewModel.UserId,
                Title = indexMemoItemViewModel.Title,
                Content = indexMemoItemViewModel.Content
            };
        }

        // IndexMemoItemViewModel 转 DeleteMemoDto
        public static DeleteMemoDto ToDeleteMemoDto(this IndexMemoItemViewModel indexMemoItemViewModel)
        {
            return new DeleteMemoDto()
            {
                Id = indexMemoItemViewModel.Id,
                UserId = indexMemoItemViewModel.UserId,
                Title = indexMemoItemViewModel.Title
            };
        }
    }
}
