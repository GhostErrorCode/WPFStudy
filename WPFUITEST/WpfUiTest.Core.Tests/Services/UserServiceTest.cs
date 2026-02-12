using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Text;
using WpfUiTest.Core.Data.Entities;
using WpfUiTest.Core.Data.Repositories.Interfaces;
using WpfUiTest.Core.Data.UnitOfWork.Interfaces;
using WpfUiTest.Core.DTOs.User;
using WpfUiTest.Core.Services.Implements;
using WpfUiTest.Shared.Utilities;

namespace WpfUiTest.Core.Tests.Services
{
    public class UserServiceTest
    {
        // region: 1. 模拟依赖字段 - 这些不是被测试对象，而是为了隔离`UserService`所创建的“假对象”
        private readonly ILogger<UserService> _mockLogger;
        private readonly IUserRepository _mockUserRepo;
        private readonly IUnitOfWork _mockUnitOfWork;

        // 2. 被测系统 (SUT - System Under Test) - 这是测试的焦点，我们将用上面的“假对象”来构造它
        private readonly UserService _userService;

        // 3. 构造函数 - 在每一个单独的测试方法执行前，xUnit都会创建一个新的`UserServiceTests`实例
        public UserServiceTest()
        {
            // 3.1 创建“假”的日志记录器。我们不会真的把日志写到文件里。
            _mockLogger = Substitute.For<ILogger<UserService>>();
            // 3.2 创建“假”的仓储。我们不会真的去查询数据库。
            _mockUserRepo = Substitute.For<IUserRepository>();
            // 3.3 创建“假”的工作单元。
            _mockUnitOfWork = Substitute.For<IUnitOfWork>();
            // 3.4 最关键的一步：用“假依赖”构造“真服务”。这样，测试就只关心UserService本身的逻辑。
            _userService = new UserService(_mockLogger, _mockUserRepo, _mockUnitOfWork);
        }

        // region: 具体的测试案例
        [Fact] // 这是一个“事实”测试，用于测试一个确定的、无需外部数据输入的场景。
        public async Task LoginAsync_WithValidAccountAndPassword_ShouldReturnSuccess()
        {
            // ========== ARRANGE 阶段：准备测试舞台 ==========
            // 目标：创造一个“用户提供了正确账号密码”的场景。

            // 1. 准备输入数据：模拟前端传递过来的登录请求。
            var validLoginDto = new LoginUserDto { Account = "testUser", Password = "123456" };

            // 2. 准备模拟数据：模拟数据库中存储的用户记录。
            //    注意：数据库里存的必须是密码的哈希值，不能是明文“123456”。
            var userInDb = new User
            {
                Id = 1,
                Account = "testUser",
                Password = PasswordHasher.Hash("123456") // 使用和真实业务一样的工具生成哈希
            };

            // 3. 配置“假”仓储的行为：当服务调用`GetByAccountAsync("testUser")`时，让它返回我们准备好的模拟用户。
            //    这相当于说：“假设数据库里存在这个用户”。
            _mockUserRepo.GetByAccountAsync("testUser").Returns(Task.FromResult<User?>(userInDb));

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
            Assert.Equal("testUser", _userService.UserAccount);
        }

        [Fact]
        public async Task LoginAsync_WithWrongPassword_ShouldReturnFailure()
        {
            // ========== ARRANGE ==========
            // 目标：创造一个“账号存在但密码错误”的场景。
            var wrongPasswordDto = new LoginUserDto { Account = "testUser", Password = "wrong" };
            var userInDb = new User
            {
                Id = 1,
                Account = "testUser",
                // 数据库里存的是“correctPassword”的哈希，而用户输入的是“wrong”
                Password = PasswordHasher.Hash("correctPassword")
            };
            _mockUserRepo.GetByAccountAsync("testUser").Returns(Task.FromResult<User?>(userInDb));

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

        [Fact]
        public async Task LoginAsync_WithNonExistentAccount_ShouldReturnFailure()
        {
            // ========== ARRANGE ==========
            // 目标：创造一个“账号根本不存在”的场景。
            var invalidLoginDto = new LoginUserDto { Account = "nonExistent", Password = "any" };
            // 配置“假”仓储返回null，模拟数据库中查无此人的情况。
            _mockUserRepo.GetByAccountAsync("nonExistent").Returns(Task.FromResult<User?>(null));

            // ========== ACT ==========
            var result = await _userService.LoginAsync(invalidLoginDto);

            // ========== ASSERT ==========
            // 验证结果应为失败，且未登录。
            Assert.False(result.IsSuccess);
            Assert.False(_userService.IsLoggedIn);
            // 注意：这里没有断言具体的错误信息，因为你的业务逻辑可能和密码错误返回同样的消息。
            // 如果想严格测试，可以在这里也加上对 `result.Message` 的断言。
        }

        [Fact]
        public async Task Logout_WhenLoggedIn_ShouldClearCurrentUser()
        {
            // Arrange: 先真实地登录（需要配置模拟仓储返回一个用户）
            var userInDb = new User { Id = 1, Account = "testUser", Password = PasswordHasher.Hash("123456") };
            _mockUserRepo.GetByAccountAsync("testUser").Returns(Task.FromResult<User?>(userInDb));
            await _userService.LoginAsync(new LoginUserDto { Account = "testUser", Password = "123456" });
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
