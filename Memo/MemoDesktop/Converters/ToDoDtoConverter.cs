using MemoDesktop.Dtos.ToDo;
using System;
using System.Collections.Generic;
using System.Text;

namespace MemoDesktop.Converters
{
    // 用于各种Dto之间的转换
    public static class ToDoDtoConverter
    {
        // ToDoDto 转 UpdateToDoDto
        public static UpdateToDoDto ConvertToDoDtoToUpdateToDoDto(ToDoDto toDoDto)
        {
            return new UpdateToDoDto()
            {
                Id = toDoDto.Id,
                Title = toDoDto.Title,
                Content = toDoDto.Content,
                Status = toDoDto.Status,
                UpdateDate = DateTime.Now
            };
        }

        // ToDoDto 转 CreateToDoDto
        public static CreateToDoDto ConvertToDoDtoToAddToDoDto(ToDoDto toDoDto)
        {
            return new CreateToDoDto()
            {
                Title = toDoDto.Title,
                Content = toDoDto.Content,
                Status = toDoDto.Status
            };
        }
    }
}
