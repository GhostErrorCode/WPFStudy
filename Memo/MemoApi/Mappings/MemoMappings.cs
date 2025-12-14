using MemoApi.Dtos.Memo;
using MemoApi.Dtos.ToDo;
using MemoApi.Entities;
using System.Runtime.CompilerServices;

namespace MemoApi.Mappings
{
    /// <summary>
    /// 备忘录实体与DTO之间的手动映射工具类
    /// 此类包含所有与ToDo相关的对象转换逻辑
    /// 使用手动映射可以完全控制转换过程，避免使用自动化工具的黑盒操作
    /// </summary>
    public class MemoMappings
    {
        /// <summary>
        /// 将 Memo 实体对象转换为 MemoDto 数据传输对象
        /// 此方法用于在查询操作后，将数据库实体转换为返回给客户端的数据格式
        /// </summary>
        /// <param name="memoEntity">从数据库获取的待办事项实体对象</param>
        /// <returns>转换为 ToDoDto 对象，如果输入为 null 则返回 null</returns>
        public static MemoDto ConvertMemoEntityToMemoDto(Memo memoEntity)
        {
            // 防御性编程：检查输入参数是否有效
            if (memoEntity == null)
            {
                return null;
            }

            // 创建新的DTO实例
            MemoDto memoDto = new MemoDto();

            // 开始属性映射：将实体属性值复制到DTO对应属性
            memoDto.Id = memoEntity.Id;
            memoDto.Title = memoEntity.Title ?? String.Empty; // 处理可能为null的情况
            memoDto.Content = memoEntity.Content ?? String.Empty; // 处理可能为null的情况

            // 日期格式化：将DateTime对象转换为特定格式的字符串
            memoDto.CreateDateFormatted = memoEntity.CreateDate.ToString("yyyy年MM月dd日 HH:mm:ss");
            memoDto.UpdateDateFormatted = memoEntity.UpdateDate.ToString("yyyy年MM月dd日 HH:mm:ss");

            // 返回完整填充的DTO对象
            return memoDto;
        }


        /// <summary>
        /// 将 CreateMemoDto 对象转换为 Memo 实体对象
        /// 此方法用于在创建操作前，将客户端提交的数据转换为数据库实体格式
        /// </summary>
        /// <param name="createMemoDto">客户端提交的创建待办事项数据</param>
        /// <returns>转换为 ToDo 实体对象</returns>
        public static Memo ConvertCreateToDoDtoToToDoEntity(CreateMemoDto createMemoDto)
        {
            // 创建新的实体实例
            Memo memoEntity = new Memo();

            // 开始属性映射：将DTO属性值复制到实体对应属性
            memoEntity.Title = createMemoDto.Title;
            memoEntity.Content = createMemoDto.Content;

            // 设置系统生成的字段
            memoEntity.CreateDate = DateTime.Now; // 创建时间设置为当前服务器时间
            memoEntity.UpdateDate = DateTime.Now; // 初始时修改时间与创建时间相同

            // Id 属性不在此处设置，将由数据库自动生成（自增主键）

            // 返回完整填充的实体对象
            return memoEntity;
        }


        /// <summary>
        /// 使用 UpdateMemoDto 对象更新已存在的 Memo 实体对象
        /// 此方法用于在更新操作中，用客户端提交的新数据修改现有实体
        /// 注意：此方法直接修改传入的实体对象，而不是创建新实例
        /// </summary>
        /// <param name="existingMemoEntity">数据库中已存在的待办事项实体对象</param>
        /// <param name="updateMemoDto">客户端提交的更新数据</param>
        public static void UpdateMemoEntityFromUpdateMemoDto(Memo existingMemoEntity, UpdateMemoDto updateMemoDto)
        {
            // 防御性编程：检查输入参数是否有效
            if (existingMemoEntity == null)
            {
                throw new ArgumentNullException("existingMemoEntity", "待更新的实体对象不能为null。");
            }

            if (updateMemoDto == null)
            {
                throw new ArgumentNullException("updateMemoDto", "更新数据对象不能为null。");
            }

            // 开始属性更新：用DTO中的新值替换实体中的旧值
            existingMemoEntity.Title = updateMemoDto.Title;
            existingMemoEntity.Content = updateMemoDto.Content;

            // 更新修改时间为当前服务器时间
            // 注意：这里使用服务器时间而不是客户端提供的时间，确保时间一致性
            existingMemoEntity.UpdateDate = DateTime.Now;

            // 特别注意：不修改 CreateDate 字段，创建时间在实体生命周期内保持不变
        }


        /// <summary>
        /// 将 ToDo 实体对象集合转换为 ToDoDto 对象集合
        /// 此方法用于批量转换，如获取所有待办事项的场景
        /// </summary>
        /// <param name="memoEntityCollection">待办事项实体对象集合</param>
        /// <returns>转换后的 ToDoDto 对象集合</returns>
        public static List<MemoDto> ConvertMemoEntityCollectionToMemoDtoCollection(IEnumerable<Memo> memoEntityCollection)
        {
            // 创建空的DTO集合
            List<MemoDto> memoDtoCollection = new List<MemoDto>();

            // 防御性编程：检查输入参数是否有效
            if (memoEntityCollection == null)
            {
                return memoDtoCollection; // 返回空集合而不是null
            }

            // 遍历实体集合，将每个实体转换为DTO
            foreach (Memo memoEntity in memoEntityCollection)
            {
                MemoDto memoDto = ConvertMemoEntityToMemoDto(memoEntity);

                // 确保转换结果有效后再添加到集合
                if (memoDto != null)
                {
                    memoDtoCollection.Add(memoDto);
                }
            }

            // 返回完整填充的DTO集合
            return memoDtoCollection;
        }
    }
}
