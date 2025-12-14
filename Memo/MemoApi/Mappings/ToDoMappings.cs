using MemoApi.Dtos.ToDo;
using MemoApi.Entities;

namespace MemoApi.Mappings
{
    /// <summary>
    /// 待办事项实体与DTO之间的手动映射工具类
    /// 此类包含所有与ToDo相关的对象转换逻辑
    /// 使用手动映射可以完全控制转换过程，避免使用自动化工具的黑盒操作
    /// </summary>
    public class ToDoMappings
    {
        /// <summary>
        /// 将 ToDo 实体对象转换为 ToDoDto 数据传输对象
        /// 此方法用于在查询操作后，将数据库实体转换为返回给客户端的数据格式
        /// </summary>
        /// <param name="todoEntity">从数据库获取的待办事项实体对象</param>
        /// <returns>转换为 ToDoDto 对象，如果输入为 null 则返回 null</returns>
        public static ToDoDto ConvertToDoEntityToToDoDto(ToDo todoEntity)
        {
            // 防御性编程：检查输入参数是否有效
            if (todoEntity == null)
            {
                return null;
            }

            // 创建新的DTO实例
            ToDoDto toDoDto = new ToDoDto();

            // 开始属性映射：将实体属性值复制到DTO对应属性
            toDoDto.Id = todoEntity.Id;
            toDoDto.Title = todoEntity.Title ?? String.Empty; // 处理可能为null的情况
            toDoDto.Content = todoEntity.Content ?? String.Empty; // 处理可能为null的情况

            // 状态值转换：将数字状态转换为可读的文本
            toDoDto.StatusText = ConvertStatusNumberToText(todoEntity.Status);

            // 日期格式化：将DateTime对象转换为特定格式的字符串
            toDoDto.CreateDateFormatted = todoEntity.CreateDate.ToString("yyyy年MM月dd日 HH:mm:ss");
            toDoDto.UpdateDateFormatted = todoEntity.UpdateDate.ToString("yyyy年MM月dd日 HH:mm:ss");

            // 返回完整填充的DTO对象
            return toDoDto;
        }


        /// <summary>
        /// 将 CreateToDoDto 对象转换为 ToDo 实体对象
        /// 此方法用于在创建操作前，将客户端提交的数据转换为数据库实体格式
        /// </summary>
        /// <param name="createToDoDto">客户端提交的创建待办事项数据</param>
        /// <returns>转换为 ToDo 实体对象</returns>
        public static ToDo ConvertCreateToDoDtoToToDoEntity(CreateToDoDto createToDoDto)
        {
            // 创建新的实体实例
            ToDo todoEntity = new ToDo();

            // 开始属性映射：将DTO属性值复制到实体对应属性
            todoEntity.Title = createToDoDto.Title;
            todoEntity.Content = createToDoDto.Content;
            todoEntity.Status = createToDoDto.Status;

            // 设置系统生成的字段
            todoEntity.CreateDate = DateTime.Now; // 创建时间设置为当前服务器时间
            todoEntity.UpdateDate = DateTime.Now; // 初始时修改时间与创建时间相同

            // Id 属性不在此处设置，将由数据库自动生成（自增主键）

            // 返回完整填充的实体对象
            return todoEntity;
        }


        /// <summary>
        /// 使用 UpdateToDoDto 对象更新已存在的 ToDo 实体对象
        /// 此方法用于在更新操作中，用客户端提交的新数据修改现有实体
        /// 注意：此方法直接修改传入的实体对象，而不是创建新实例
        /// </summary>
        /// <param name="existingToDoEntity">数据库中已存在的待办事项实体对象</param>
        /// <param name="updateToDoDto">客户端提交的更新数据</param>
        public static void UpdateToDoEntityFromUpdateToDoDto(ToDo existingToDoEntity, UpdateToDoDto updateToDoDto)
        {
            // 防御性编程：检查输入参数是否有效
            if (existingToDoEntity == null)
            {
                throw new ArgumentNullException("existingToDoEntity", "待更新的实体对象不能为null。");
            }

            if (updateToDoDto == null)
            {
                throw new ArgumentNullException("updateToDoDto", "更新数据对象不能为null。");
            }

            // 开始属性更新：用DTO中的新值替换实体中的旧值
            existingToDoEntity.Title = updateToDoDto.Title;
            existingToDoEntity.Content = updateToDoDto.Content;
            existingToDoEntity.Status = updateToDoDto.Status;

            // 更新修改时间为当前服务器时间
            // 注意：这里使用服务器时间而不是客户端提供的时间，确保时间一致性
            existingToDoEntity.UpdateDate = DateTime.Now;

            // 特别注意：不修改 CreateDate 字段，创建时间在实体生命周期内保持不变
        }


        /// <summary>
        /// 将 ToDo 实体对象集合转换为 ToDoDto 对象集合
        /// 此方法用于批量转换，如获取所有待办事项的场景
        /// </summary>
        /// <param name="toDoEntityCollection">待办事项实体对象集合</param>
        /// <returns>转换后的 ToDoDto 对象集合</returns>
        public static List<ToDoDto> ConvertToDoEntityCollectionToToDoDtoCollection(IEnumerable<ToDo> toDoEntityCollection)
        {
            // 创建空的DTO集合
            List<ToDoDto> toDoDtoCollection = new List<ToDoDto>();

            // 防御性编程：检查输入参数是否有效
            if (toDoEntityCollection == null)
            {
                return toDoDtoCollection; // 返回空集合而不是null
            }

            // 遍历实体集合，将每个实体转换为DTO
            foreach (ToDo todoEntity in toDoEntityCollection)
            {
                ToDoDto toDoDto = ConvertToDoEntityToToDoDto(todoEntity);

                // 确保转换结果有效后再添加到集合
                if (toDoDto != null)
                {
                    toDoDtoCollection.Add(toDoDto);
                }
            }

            // 返回完整填充的DTO集合
            return toDoDtoCollection;
        }


        /// <summary>
        /// 内部辅助方法：将状态数字代码转换为可读的文本描述
        /// 此方法封装了状态值的转换逻辑，确保整个应用中状态显示的一致性
        /// </summary>
        /// <param name="statusNumber">状态数字代码</param>
        /// <returns>对应的状态文本描述</returns>
        private static string ConvertStatusNumberToText(int statusNumber)
        {
            string statusText;

            // 使用switch语句处理不同的状态值
            switch (statusNumber)
            {
                case 0:
                    statusText = "待办";
                    break;
                case 1:
                    statusText = "进行中";
                    break;
                case 2:
                    statusText = "已完成";
                    break;
                default:
                    // 处理意外状态值，避免返回null或空字符串
                    statusText = "未知状态";
                    break;
            }

            return statusText;
        }
    }
}
