using System;
using System.Collections.Generic;
using System.Text;
using WpfUiTest.Core.Data.Entities;
using WpfUiTest.Core.DTOs.User;
using WpfUiTest.Shared.Utilities;

namespace WpfUiTest.Core.Mapping
{
    // 用户映射方法
    public static class UserMapper
    {
        // User实体 映射 UserResultDto数据传输对象
        public static UserResultDto ToUserResultDto(this User user)
        {
            return new UserResultDto()
            {
                Id = user.Id,
                Account = user.Account,
                UserName = user.UserName,
                CreateDate = user.CreateDate,
                UpdateDate = user.UpdateDate
            };
        }

        // RegisterUserDto数据传输对象 映射 User实体
        public static User ToUser(this RegisterUserDto registerUserDto)
        {
            return new User()
            {
                Account = registerUserDto.Account,
                UserName = registerUserDto.UserName,
                Password = PasswordHasher.Hash(registerUserDto.Password),
                CreateDate = DateTimeUtility.NowNoMilliseconds(),
                UpdateDate = DateTimeUtility.NowNoMilliseconds()
            };
        }
    }
}
