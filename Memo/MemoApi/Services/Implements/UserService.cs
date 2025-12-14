using MemoApi.Dtos.Memo;
using MemoApi.Dtos.User;
using MemoApi.Entities;
using MemoApi.Mappings;
using MemoApi.Repositories;
using MemoApi.Results;
using MemoApi.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using System;

namespace MemoApi.Services.Implements
{
    // 用户服务层，继承对应接口
    public class UserService : IUserService
    {
        // 私有字段：用户仓储接口
        // 作用：通过仓储访问用户数据，服务层不直接操作数据库
        private readonly IUserRepository _userRepository;

        // 私有字段：工作单元
        // 作用：协调多个仓储操作，管理事务
        private readonly IUnitOfWork _unitOfWork;

        // 私有字段：Hash加密
        // 作用：加密Password等敏感字段
        private readonly IPasswordHasher<User> _passwordHasher;

        // 构造函数 - 获取工作单元
        // 构造函数：依赖注入
        // 参数：
        //   - memoRepository: IMemoRepository - 产品具体仓储
        //   - unitOfWork: IUnitOfWork - 工作单元
        // 设计模式：构造函数注入，这是最推荐的依赖注入方式
        public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork, IPasswordHasher<User> passwordHasher)
        {
            // 赋值给字段
            // this关键字：明确引用当前实例的字段，避免与参数混淆
            this._userRepository = userRepository;
            this._unitOfWork = unitOfWork;
            this._passwordHasher = passwordHasher;
        }


        // 用户登录
        public async Task<ServiceResult<UserDto>> LoginUserAsync(LoginUserDto loginUserDto)
        {
            try
            {
                // 先查出账户再验证密码”是必须且唯一安全的做法
                // 判断loginUserDto是否为空
                if (loginUserDto == null) { return ServiceResult<UserDto>.Fail("请输入用户账户和密码!"); }
                // 验证数据库中是否有此用户以及密码是否正确
                User? loginUser = await this._userRepository.GetByAccountAsync(loginUserDto.Account);
                // 如果没有找到对应用户，则提示用户名或者密码错误
                if (loginUser == null) { return ServiceResult<UserDto>.Fail("用户账户或密码错误，请检查后重试!"); }
                // 如果找到了对应用户，则验证下密码是否正确
                if (this._passwordHasher.VerifyHashedPassword(loginUser, loginUser.Password, loginUserDto.Password) == PasswordVerificationResult.Failed) { return ServiceResult<UserDto>.Fail("用户账户或密码错误，请检查后重试!"); }
                return ServiceResult<UserDto>.Success(UserMappings.ConvertUserEntityToUserDto(loginUser));
            }
            catch(Exception ex)
            {
                // 执行发生内部异常处理
                return ServiceResult<UserDto>.Fail($"用户登录失败,异常信息: {ex.Message}");
            }
        }

        // 用户注册
        public async Task<ServiceResult<UserDto>> RegisterUserAsync(RegisterUserDto registerUserDto)
        {
            try
            {
                // 判断数据库是否已存在此用户账户
                bool accountExists = await this._userRepository.ExistsAsync(predicate: (User user) => user.Account.Equals(registerUserDto.Account));
                // 如果用户账户已存在则注册失败
                if (accountExists) { return ServiceResult<UserDto>.Fail($"用户账户 {registerUserDto.Account} 已存在，请重新输入新用户账户!"); }
                // 再次判断2次密码是否相同
                if (!registerUserDto.Password.Equals(registerUserDto.ConfirmPassword)) { return ServiceResult<UserDto>.Fail($"用户 {registerUserDto.Account} 输入的2次密码不一致，请检查!"); }

                // 两次验证都没问题的话，将数据写入数据库并注册成功
                User registerUser = UserMappings.ConvertRegisterUserDtoToUserEntity(registerUserDto);
                // Password密码加密
                registerUser.Password = this._passwordHasher.HashPassword(registerUser, registerUserDto.Password);
                // 暂存再事务，等待入库
                await this._userRepository.AddAsync(registerUser);
                // 将插入提交至数据库
                if(await this._unitOfWork.SaveChangesAsync() > 0)
                {
                    return ServiceResult<UserDto>.Success(UserMappings.ConvertUserEntityToUserDto(registerUser));
                }
                else
                {
                    // 提交至数据库时出错
                    return ServiceResult<UserDto>.Fail($"用户账户 {registerUserDto.Account} 注册时出错，请稍后再试!");
                }
            }
            catch (Exception ex)
            {
                // 执行发生内部异常处理
                return ServiceResult<UserDto>.Fail($"用户注册失败,异常信息: {ex.Message}");
            }
        }
    }
}
