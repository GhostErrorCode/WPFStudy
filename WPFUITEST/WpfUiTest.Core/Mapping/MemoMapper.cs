using System;
using System.Collections.Generic;
using System.Text;
using WpfUiTest.Core.Data.Entities;
using WpfUiTest.Core.DTOs.Memo;

namespace WpfUiTest.Core.Mapping
{
    // 备忘录映射方法
    public static class MemoMapper
    {
        // Memo实体 映射 MemoDto数据传输对象
        public static MemoDto ToMemoDto(this Memo memo)
        {
            return new MemoDto()
            {
                Id = memo.Id,
                UserId = memo.UserId,
                Title = memo.Title,
                Content = memo.Content,
                CreateDate = memo.CreateDate,
                UpdateDate = memo.UpdateDate
            };
        }

        // List<Memo>实体集合 映射 List<MemoDto>数据传输对象集合
        public static List<MemoDto> ToMemoDtoCollection(this List<Memo> Memos)
        {
            List<MemoDto> MemoDtos = new List<MemoDto>();
            // 如果传入的是空集合，就直接返回空集合
            if (Memos == null) { return MemoDtos; }
            // 如果传入的不是空集合就遍历并转为Dto
            foreach (Memo Memo in Memos) { MemoDtos.Add(Memo.ToMemoDto()); }
            // 返回结果Dto集合
            return MemoDtos;
        }
    }
}
