using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using WpfUiTest.Core.Data.Entities;
using WpfUiTest.Core.Data.Repositories.Interfaces;
using WpfUiTest.Core.Data.UnitOfWork.Interfaces;
using WpfUiTest.Core.DTOs.User;
using WpfUiTest.Core.Mapping;
using WpfUiTest.Core.Services.Implements;
using WpfUiTest.Core.Services.Interfaces;
using WpfUiTest.Shared.Utilities;

namespace WpfUiTest.Core.Tests.Services
{
    public class UserServiceTests
    {
        // 模拟依赖字段（日志、用户仓储、工作单元）
        private readonly ILogger<UserService> _mockLogger;
        private readonly IUserRepository _mockUserRepository;
        private readonly IUnitOfWork _mockUnitOfWork;

        // 被测试系统（用户服务层）
        private readonly UserService _userService;

        // 构造函数
        public UserServiceTests()
        {
            // 为模拟字段创建模拟实例
            this._mockLogger = Substitute.For<ILogger<UserService>>();
            this._mockUserRepository = Substitute.For<IUserRepository>();
            this._mockUnitOfWork = Substitute.For<IUnitOfWork>();

            // 关键：为工作单元的异步方法提供默认的、成功的任务完成行为
            // 这样，无论测试是否执行事务块，这些方法都能安全调用。
            // 配置 IUnitOfWork 的异步方法
            // 对于返回 Task 的方法，使用 Task.CompletedTask
            _mockUnitOfWork.BeginTransactionAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
            _mockUnitOfWork.CommitTransactionAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
            _mockUnitOfWork.RollbackTransactionAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

            // 关键：对于返回 Task<int> 的 SaveChangesAsync，返回一个 Task<int>，例如表示1行受影响
            _mockUnitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(1)); // 简化写法，等价于 Returns(Task.FromResult(1))

            // 创建真实IUserService实例
            this._userService = new UserService(this._mockLogger, this._mockUserRepository, this._mockUnitOfWork);
        }

        // 以下为具体测试方法
        // ===== 注册方法测试 ===================================================================
        // 1.注册失败 - 数据不完整或为空
        [Fact]
        public async Task RegisterAsync_WithNullOrIncompleteData()
        {
            // ========== ARRANGE 阶段：准备测试舞台 ==========
            // 准备注册数据Dto
            RegisterUserDto mockRegisterUserDto = new RegisterUserDto() { Account = "Ghost", UserName = "", Password="123456",ConfirmPassword="123456" };

            // ========== ACT 阶段：执行关键操作 ==========
            ServiceResult<bool> registerResult = await this._userService.RegisterAsync(mockRegisterUserDto);

            // ========== ASSERT 阶段：验证结果 ==========
            Assert.False(registerResult.IsSuccess);  // 判断结果是否为False
            Assert.Equal("注册信息不能为空", registerResult.Message);  // 判断返回的结果提示信息是否正确
        }
        // 2.注册失败 - 两次密码不一致
        [Fact]
        public async Task RegisterAsync_WithPasswordsDoNotMatch()
        {
            // ========== ARRANGE 阶段：准备测试舞台 ==========
            // 准备注册数据Dto
            RegisterUserDto mockRegisterUserDto = new RegisterUserDto() { Account = "Ghost", UserName = "Ghost", Password = "654321", ConfirmPassword = "123456" };

            // ========== ACT 阶段：执行关键操作 ==========
            ServiceResult<bool> registerResult = await this._userService.RegisterAsync(mockRegisterUserDto);

            // ========== ASSERT 阶段：验证结果 ==========
            Assert.False(registerResult.IsSuccess);  // 判断结果是否为False
            Assert.Equal("输入的两次密码不一致", registerResult.Message);  // 判断返回的结果提示信息是否正确
        }
        // 3.注册失败 - 密码不符合规则
        [Fact]
        public async Task RegisterAsync_WithPasswordsNotComplyRules()
        {
            // ========== ARRANGE 阶段：准备测试舞台 ==========
            // 准备注册数据Dto
            RegisterUserDto mockRegisterUserDto = new RegisterUserDto() { Account = "Ghost", UserName = "Ghost", Password = "12345", ConfirmPassword = "12345" };

            // ========== ACT 阶段：执行关键操作 ==========
            ServiceResult<bool> registerResult = await this._userService.RegisterAsync(mockRegisterUserDto);

            // ========== ASSERT 阶段：验证结果 ==========
            Assert.False(registerResult.IsSuccess);  // 判断结果是否为False
            Assert.Equal("密码长度至少为6位", registerResult.Message);  // 判断返回的结果提示信息是否正确
        }
        // 4.注册失败 - 用户已存在
        [Fact]
        public async Task RegisterAsync_WithUserAlreadyExists()
        {
            // ========== ARRANGE 阶段：准备测试舞台 ==========
            // 准备注册数据Dto
            RegisterUserDto mockRegisterUserDto = new RegisterUserDto() { Account = "Ghost", UserName = "Ghost", Password = "123456", ConfirmPassword = "123456" };
            // 准备已存在的用户数据
            User existUser = new User() { Account = "Ghost" };
            // 准备仓储模拟一条数据库已有的数据
            this._mockUserRepository.ExistsAsync(Arg.Any<Expression<Func<User, bool>>>(), // 正确匹配表达式树
                                                 Arg.Any<CancellationToken>()).Returns(Task.FromResult<bool>(true));
            // ========== ACT 阶段：执行关键操作 ==========
            ServiceResult<bool> registerResult = await this._userService.RegisterAsync(mockRegisterUserDto);

            // ========== ASSERT 阶段：验证结果 ==========
            Assert.False(registerResult.IsSuccess);  // 判断结果是否为False
            Assert.Equal("该账户已被注册", registerResult.Message);  // 判断返回的结果提示信息是否正确

            // 验证在用户已存在的情况下，AddAsync 方法从未被调用
            // await this._mockUserRepository.DidNotReceive().AddAsync(Arg.Any<User>());
        }
        // 5.注册成功 - 注册用户成功
        [Fact]
        public async Task RegisterAsync_WithRegisterSuccess()
        {
            // ========== ARRANGE 阶段：准备测试舞台 ==========
            // 1. 准备一个有效的注册请求
            RegisterUserDto mockRegisterUserDto = new RegisterUserDto()
            {
                Account = "NewUser", // 使用一个明确的新用户名
                UserName = "Ghost",
                Password = "123456",
                ConfirmPassword = "123456"
            };

            // 2. 准备一个“将要被保存并返回”的用户实体（模拟数据库操作后的结果）
            //    注意：这里的Password应该是哈希后的值，用于验证服务层是否做了哈希。
            User expectedSavedUser = new User()
            {
                Id = 100, // 模拟数据库生成的自增ID
                Account = "NewUser",
                UserName = "Ghost",
                Password = PasswordHasher.Hash("123456") // 关键：这是期望存入数据库的哈希值
            };
            // 3. 【核心配置】告诉模拟对象如何回应服务层的调用
            // a. 配置“用户不存在”检查
            this._mockUserRepository
                .ExistsAsync(
                    Arg.Any<Expression<Func<User, bool>>>(), // 匹配任何查询表达式
                    Arg.Any<CancellationToken>()
                )
                .Returns(Task.FromResult<bool>(false)); // 返回 false，表示账户可用

            // b. 配置“添加用户”操作
            // 我们用一个变量捕获服务层实际传给 AddAsync 的参数，以便后续断言
            User actuallyPassedUser = null;
            this._mockUserRepository
                .AddAsync(Arg.Do<User>(u => actuallyPassedUser = u)) // 捕获传入的参数
                .Returns(Task.FromResult(expectedSavedUser)); // 返回我们预设的“已保存用户”

            // ========== ACT 阶段：执行关键操作 ==========
            ServiceResult<bool> registerResult = await _userService.RegisterAsync(mockRegisterUserDto);

            // ========== ASSERT 阶段：验证结果 ==========
            // 1. 验证业务结果：必须成功
            Assert.True(registerResult.IsSuccess);
            Assert.Equal("注册成功", registerResult.Message);

            // 2. 验证交互：确保服务层正确调用了“检查存在”和“添加用户”的方法
            await _mockUserRepository.Received(1).ExistsAsync(
                Arg.Any<Expression<Func<User, bool>>>(),
                Arg.Any<CancellationToken>()
            );
            await _mockUserRepository.Received(1).AddAsync(Arg.Any<User>());

            // 3. 【安全性验证】验证传入 AddAsync 的用户密码是哈希后的，而不是明文！
            Assert.NotNull(actuallyPassedUser); // 确保确实捕获到了参数
            Assert.NotEqual("123456", actuallyPassedUser.Password); // 密码字段不应是明文
            Assert.True(PasswordHasher.Verify("123456", actuallyPassedUser.Password)); // 哈希值必须能通过验证

            // 4. （可选）验证事务被正确执行
            await _mockUnitOfWork.Received(1).BeginTransactionAsync(Arg.Any<CancellationToken>());
            await _mockUnitOfWork.Received(1).CommitTransactionAsync(Arg.Any<CancellationToken>());
            await _mockUnitOfWork.DidNotReceive().RollbackTransactionAsync(Arg.Any<CancellationToken>());
        }


        // ===== 登录方法测试 ===================================================================
        // 默认的数据库里的模拟对象
        User userInDb = new User() { Id=1001,Account="Ghost",UserName="Ghost",Password=PasswordHasher.Hash("123456") };
        // 1.登录失败 - 用户或密码为空
        [Fact]
        public async Task LoginAsync_WithNullOrIncompleteData()
        {
            // ========== ARRANGE 阶段：准备测试舞台 ==========
            // 目标：创造一个“用户提供了正确账号密码”的场景。

            // 1. 准备输入数据：模拟前端传递过来的登录请求。
            LoginUserDto validLoginDto1 = new LoginUserDto { Account = "Ghost", Password = "" };
            LoginUserDto validLoginDto2 = new LoginUserDto { Account = "", Password = "123456" };

            // 2. 准备模拟数据：模拟数据库中存储的用户记录。
            //    注意：数据库里存的必须是密码的哈希值，不能是明文“123456”。

            // 3. 配置“假”仓储的行为：当服务调用`GetByAccountAsync("testUser")`时，让它返回我们准备好的模拟用户。
            //    这相当于说：“假设数据库里存在这个用户”。
            // this._mockUserRepository.GetByAccountAsync("Ghost").Returns(Task.FromResult<User?>(userInDb));

            // ========== ACT 阶段：执行关键操作 ==========
            // 目标：触发我们要验证的核心业务逻辑。
            // 这一行就是测试的“开关”。我们调用真实的`LoginAsync`方法，但因为它内部依赖的仓储是“假的”，
            // 所以它实际上是在一个完全受我们控制的模拟环境中运行。
            ServiceResult<bool> result1 = await _userService.LoginAsync(validLoginDto1);
            ServiceResult<bool> result2 = await _userService.LoginAsync(validLoginDto2);
            ServiceResult<bool> result3 = await _userService.LoginAsync(null);

            // ========== ASSERT 阶段：验证结果 ==========
            // 目标：检查`LoginAsync`方法的行为是否完全符合我们的预期。

            // 4.1 验证业务结果：返回的`ServiceResult`对象应该指示成功。
            //    如果这里失败，说明登录逻辑在凭证正确的情况下返回了失败，这是严重的Bug。
            Assert.False(result1.IsSuccess);
            Assert.False(result2.IsSuccess);
            Assert.False(result3.IsSuccess);

            // 4.2 验证对象状态：登录成功后，`UserService`内部的当前用户状态应该被更新。
            //    我们通过公开的`IsLoggedIn`属性来间接验证内部私有字段`_currentUser`已被赋值。
            Assert.False(_userService.IsLoggedIn);

            // 4.3 验证状态细节：当前用户的账号名应该和我们登录的账号一致。
            //    这确保了状态被正确地设置为了刚刚登录的用户，而不是其他用户。
            Assert.NotEqual("Ghost", _userService.UserAccount);
        }
        // 2.登录失败 - 用户账户不存在
        [Fact]
        public async Task LoginAsync_WithNonExistentAccount_ShouldReturnFailure()
        {
            // ========== ARRANGE ==========
            // 目标：创造一个“账号根本不存在”的场景。
            var invalidLoginDto = new LoginUserDto { Account = "nonExistent", Password = "any" };
            // 配置“假”仓储返回null，模拟数据库中查无此人的情况。
            this._mockUserRepository.GetByAccountAsync("nonExistent").Returns(Task.FromResult<User?>(null));

            // ========== ACT ==========
            var result = await _userService.LoginAsync(invalidLoginDto);

            // ========== ASSERT ==========
            // 验证结果应为失败，且未登录。
            Assert.False(result.IsSuccess);
            Assert.False(_userService.IsLoggedIn);
            // 注意：这里没有断言具体的错误信息，因为你的业务逻辑可能和密码错误返回同样的消息。
            // 如果想严格测试，可以在这里也加上对 `result.Message` 的断言。
        }
        // 3.登录失败 - 密码错误
        [Fact]
        public async Task LoginAsync_WithWrongPassword_ShouldReturnFailure()
        {
            // ========== ARRANGE ==========
            // 目标：创造一个“账号存在但密码错误”的场景。
            var wrongPasswordDto = new LoginUserDto { Account = "testUser", Password = "wrong" };
            this._mockUserRepository.GetByAccountAsync("testUser").Returns(Task.FromResult<User?>(userInDb));

            // ========== ACT ==========
            var result = await _userService.LoginAsync(wrongPasswordDto);

            // ========== ASSERT ==========
            // 5.1 验证业务结果：应该失败。
            Assert.False(result.IsSuccess);
            // 5.2 验证失败信息：返回的错误消息应该包含我们预设的安全提示文本。
            //     `Assert.Contains` 检查一个字符串是否包含另一个字符串。
            Assert.Contains("用户账户或密码错误", result.Message);
            // 5.3 验证对象状态：登录失败后，服务应保持未登录状态。
            Assert.False(_userService.IsLoggedIn);
        }
        // 4.登录成功 - 登录成功
        [Fact] // 这是一个“事实”测试，用于测试一个确定的、无需外部数据输入的场景。
        public async Task LoginAsync_WithValidAccountAndPassword_ShouldReturnSuccess()
        {
            // ========== ARRANGE 阶段：准备测试舞台 ==========
            // 目标：创造一个“用户提供了正确账号密码”的场景。

            // 1. 准备输入数据：模拟前端传递过来的登录请求。
            var validLoginDto = new LoginUserDto { Account = "Ghost", Password = "123456" };

            // 2. 准备模拟数据：模拟数据库中存储的用户记录。
            //    注意：数据库里存的必须是密码的哈希值，不能是明文“123456”。

            // 3. 配置“假”仓储的行为：当服务调用`GetByAccountAsync("testUser")`时，让它返回我们准备好的模拟用户。
            //    这相当于说：“假设数据库里存在这个用户”。
            this._mockUserRepository.GetByAccountAsync("Ghost").Returns(Task.FromResult<User?>(userInDb));

            // ========== ACT 阶段：执行关键操作 ==========
            // 目标：触发我们要验证的核心业务逻辑。
            // 这一行就是测试的“开关”。我们调用真实的`LoginAsync`方法，但因为它内部依赖的仓储是“假的”，
            // 所以它实际上是在一个完全受我们控制的模拟环境中运行。
            var result = await _userService.LoginAsync(validLoginDto);

            // ========== ASSERT 阶段：验证结果 ==========
            // 目标：检查`LoginAsync`方法的行为是否完全符合我们的预期。

            // 4.1 验证业务结果：返回的`ServiceResult`对象应该指示成功。
            //    如果这里失败，说明登录逻辑在凭证正确的情况下返回了失败，这是严重的Bug。
            Assert.True(result.IsSuccess);

            // 4.2 验证对象状态：登录成功后，`UserService`内部的当前用户状态应该被更新。
            //    我们通过公开的`IsLoggedIn`属性来间接验证内部私有字段`_currentUser`已被赋值。
            Assert.True(_userService.IsLoggedIn);

            // 4.3 验证状态细节：当前用户的账号名应该和我们登录的账号一致。
            //    这确保了状态被正确地设置为了刚刚登录的用户，而不是其他用户。
            Assert.Equal("Ghost", _userService.UserAccount);
        }

        // ===== 登出方法测试 ===================================================================
        // 登出测试
        [Fact]
        public async Task Logout_WhenLoggedIn_ShouldClearCurrentUser()
        {
            // Arrange: 先真实地登录（需要配置模拟仓储返回一个用户）
            var userInDb = new User { Id = 1, Account = "Ghost", Password = PasswordHasher.Hash("123456") };
            this._mockUserRepository.GetByAccountAsync("Ghost").Returns(Task.FromResult<User?>(userInDb));
            await _userService.LoginAsync(new LoginUserDto { Account = "Ghost", Password = "123456" });
            // 先断言登录成功，确保前置条件满足
            Assert.True(_userService.IsLoggedIn);

            // Act: 执行登出
            _userService.Logout(); // 注意：根据之前讨论，Logout应改为同步方法

            // Assert: 验证状态已清除
            Assert.False(_userService.IsLoggedIn);
            Assert.Equal(string.Empty, _userService.UserAccount);
        }
    }
}
