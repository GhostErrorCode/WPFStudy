using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using WpfUiTest.Core.DTOs.ToDo;
using WpfUiTest.Modules.ToDo.ViewModels;

namespace WpfUiTest.Modules.ToDo.Mappers
{
    // ToDoItemViewModelMapper相关映射
    public static class ToDoItemViewModelMapper
    {
        // ToDoDto -> ToDoItemViewModel
        public static ToDoItemViewModel ToToDoItemViewModel(this ToDoDto toDoDto)
        {
            return new ToDoItemViewModel()
            {
                Id = toDoDto.Id,
                UserId = toDoDto.UserId,
                Title = toDoDto.Title,
                Content = toDoDto.Content,
                Status = toDoDto.Status,
                CreateDate = toDoDto.CreateDate,
                UpdateDate = toDoDto.UpdateDate
            };
        }

        // ToDoItemViewModel ->  AddToDoDto
        public static AddToDoDto ToAddToDoDto(this ToDoItemViewModel toDoItemViewModel)
        {
            return new AddToDoDto()
            {
                Title = toDoItemViewModel.Title,
                Content = toDoItemViewModel.Content,
                Status = toDoItemViewModel.Status
            };
        }
    }
}
