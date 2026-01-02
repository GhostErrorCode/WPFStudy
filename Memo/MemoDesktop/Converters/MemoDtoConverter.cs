using MemoDesktop.Dtos.Memo;
using MemoDesktop.Dtos.ToDo;
using System;
using System.Collections.Generic;
using System.Text;

namespace MemoDesktop.Converters
{
    // 用于Memo中各个Dto的转换
    public static class MemoDtoConverter
    {
        // MemoDto 转 UpdateMemoDto
        public static UpdateMemoDto ConvertMemoDtoToUpdateMemoDto(MemoDto memoDto)
        {
            return new UpdateMemoDto()
            {
                Id = memoDto.Id,
                Title = memoDto.Title,
                Content = memoDto.Content,
                UpdateDate = DateTime.Now
            };
        }

        // MemoDto 转 CreateMemoDto
        public static CreateMemoDto ConvertMemoDtoToAddMemoDto(MemoDto memoDto)
        {
            return new CreateMemoDto()
            {
                Title = memoDto.Title,
                Content = memoDto.Content
            };
        }
    }
}
