using System;
using System.Collections.Generic;
using System.Text;
using WpfUiTest.App.ViewModels.Main;
using WpfUiTest.Core.DTOs.Memo;
using WpfUiTest.Core.DTOs.ToDo;

namespace WpfUiTest.App.ViewModels.Mapping
{
    // Dto 转 VM 扩展方法
    public static class IndexToDoItemViewModelMapper
    {
        // ToDoDto 转 IndexToDoItemViewModel
        public static IndexToDoItemViewModel ToIndexToDoItemViewModel(this ToDoDto toDoDto)
        {
            return new IndexToDoItemViewModel()
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

        // IndexToDoItemViewModel 转 AddToDoDto
        public static AddToDoDto ToAddToDoDto(this IndexToDoItemViewModel indexToDoItemViewModel)
        {
            return new AddToDoDto()
            {
                Title = indexToDoItemViewModel.Title,
                Content = indexToDoItemViewModel.Content,
                Status = indexToDoItemViewModel.Status
            };
        }
    }
}
