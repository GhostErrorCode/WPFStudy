using System;
using System.Collections.Generic;
using System.Text;
using WpfUiTest.Core.DTOs.Memo;
using WpfUiTest.Modules.Memo.ViewModels;

namespace WpfUiTest.Modules.Memo.Mappers
{
    // MemoItemViewModelMapper相关映射
    public static class MemoItemViewModelMapper
    {
        // MemoDto -> MemoItemViewModel
        public static MemoItemViewModel ToMemoItemViewModel(this MemoDto memoDto)
        {
            return new MemoItemViewModel()
            {
                Id = memoDto.Id,
                UserId = memoDto.UserId,
                Title = memoDto.Title,
                Content = memoDto.Content,
                CreateDate = memoDto.CreateDate,
                UpdateDate = memoDto.UpdateDate
            };
        }

        // MemoItemViewModel ->  AddMemoDto
        public static AddMemoDto ToAddMemoDto(this MemoItemViewModel memoItemViewModel)
        {
            return new AddMemoDto()
            {
                Title = memoItemViewModel.Title,
                Content = memoItemViewModel.Content
            };
        }

        // MemoItemViewModel ->  UpdateMemoDto
        public static UpdateMemoDto ToUpdateMemoDto(this MemoItemViewModel memoItemViewModel)
        {
            return new UpdateMemoDto()
            {
                Id = memoItemViewModel.Id,
                UserId = memoItemViewModel.UserId,
                Title = memoItemViewModel.Title,
                Content = memoItemViewModel.Content
            };
        }

        // MemoItemViewModel 转 DeleteMemoDto
        public static DeleteMemoDto ToDeleteMemoDto(this MemoItemViewModel memoItemViewModel)
        {
            return new DeleteMemoDto()
            {
                Id = memoItemViewModel.Id,
                UserId = memoItemViewModel.UserId,
                Title = memoItemViewModel.Title
            };
        }
    }
}
