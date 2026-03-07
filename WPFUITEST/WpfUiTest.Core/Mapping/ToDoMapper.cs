using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using WpfUiTest.Core.Data.Entities;
using WpfUiTest.Core.DTOs.Memo;
using WpfUiTest.Core.DTOs.ToDo;
using WpfUiTest.Shared.Utilities;

namespace WpfUiTest.Core.Mapping
{
    // 待办事项映射方法
    public static class ToDoMapper
    {
        // ToDo实体 映射 ToDoDto数据传输对象
        public static ToDoDto ToToDoDto(this ToDo toDo)
        {
            return new ToDoDto()
            {
                Id = toDo.Id,
                UserId = toDo.UserId,
                Title = toDo.Title,
                Content = toDo.Content,
                Status  = toDo.Status,
                CreateDate = toDo.CreateDate,
                UpdateDate = toDo.UpdateDate
            };
        }

        // List<ToDo>实体集合 映射 List<ToDoDto>数据传输对象集合
        public static List<ToDoDto> ToToDoDtoCollection(this List<ToDo> toDos)
        {
            List<ToDoDto> toDoDtos = new List<ToDoDto>();
            // 如果传入的是空集合，就直接返回空集合
            if(toDos == null) { return toDoDtos; }
            // 如果传入的不是空集合就遍历并转为Dto
            foreach(ToDo toDo in toDos) { toDoDtos.Add(toDo.ToToDoDto()); }
            // 返回结果Dto集合
            return toDoDtos;
        }

        // AddToDoDto数据传输对象 映射 ToDo实体
        public static ToDo ToToDo(this AddToDoDto addToDoDto, int userId)
        {
            return new ToDo()
            {
                Title = addToDoDto.Title,
                Content = addToDoDto.Content,
                Status = addToDoDto.Status,
                UserId = userId,
                CreateDate = DateTimeUtility.NowNoMilliseconds(),
                UpdateDate = DateTimeUtility.NowNoMilliseconds()
            };
        }
    }
}
