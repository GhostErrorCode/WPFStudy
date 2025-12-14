using MemoApi.Dtos.Memo;
using MemoApi.Dtos.User;
using MemoApi.Entities;

namespace MemoApi.Mappings
{
    /// <summary>
    /// 用户实体与DTO之间的手动映射工具类
    /// 此类包含所有与ToDo相关的对象转换逻辑
    /// 使用手动映射可以完全控制转换过程，避免使用自动化工具的黑盒操作
    /// </summary>
    public class UserMappings
    {
        /// <summary>
        /// 将 RegisterUserDto 对象转换为 User 实体对象
        /// 注意：此方法仅转换基本属性，密码字段应在服务层进行哈希处理
        /// </summary>
        /// <param name="registerDto">客户端提交的用户注册数据</param>
        /// <returns>转换后的 User 实体对象，密码字段为原始字符串</returns>
        public static User ConvertRegisterUserDtoToUserEntity(RegisterUserDto registerUserDto)
        {
            // 防御性编程：检查输入参数是否有效
            if (registerUserDto == null)
            {
                throw new ArgumentNullException(nameof(registerUserDto), "用户注册数据对象不能为null。");
            }

            // 创建新的实体实例
            User userEntity = new User();

            // 开始属性映射：将DTO属性值复制到实体对应属性
            userEntity.Account = registerUserDto.Account.Trim(); // 去除首尾空格
            userEntity.UserName = registerUserDto.UserName?.Trim() ?? String.Empty;

            // 重要：密码字段在此处保持原始值，哈希化应在服务层完成
            // 这是为了明确责任划分：映射只负责数据转移，服务层负责业务逻辑（密码安全）
            userEntity.Password = registerUserDto.Password;

            // 设置系统生成的字段
            DateTime currentTime = DateTime.UtcNow;
            userEntity.CreateDate = currentTime;
            userEntity.UpdateDate = currentTime;

            // Id 属性不在此处设置，将由数据库自动生成（自增主键）

            // 返回完整填充的实体对象
            return userEntity;
        }


        /// <summary>
        /// 将 User 实体对象转换为 UserDto 数据传输对象
        /// 此方法用于在查询操作后，将数据库实体转换为返回给客户端的用户信息
        /// </summary>
        /// <param name="userEntity">从数据库获取的用户实体对象</param>
        /// <returns>转换为 UserDto 对象，如果输入为 null 则返回 null</returns>
        public static UserDto ConvertUserEntityToUserDto(User userEntity)
        {
            // 防御性编程：检查输入参数是否有效
            if (userEntity == null)
            {
                return null;
            }

            // 创建新的DTO实例
            UserDto userDto = new UserDto();

            // 开始属性映射：将实体属性值复制到DTO对应属性
            userDto.Id = userEntity.Id;
            userDto.Account = userEntity.Account ?? String.Empty; // 处理可能为null的情况
            userDto.UserName = userEntity.UserName ?? String.Empty; // 处理可能为null的情况
            userDto.CreateDate = userEntity.CreateDate;
            userDto.UpdateDate = userEntity.UpdateDate;

            // 特别注意：绝对不映射 Password 字段，确保敏感信息不泄露

            // 返回完整填充的DTO对象
            return userDto;
        }
    }
}
