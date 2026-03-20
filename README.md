# WPFStudy

#### 仓库介绍

此库内的所有项目均作为学习使用，使用的SDK：.NET10

- v1.0.0版本采用WPF + ASP.NET + MaterialDesign
- v2.0.0版本采用WPF + WPFUI（无后端）

#### 主要功能实现（v2.0.0）

1. 用户隔离的CRUD操作
2. 自定义主题切换
3. 自定义控件及依赖属性的使用
4. 多窗口切换功能、明暗主题切换功能
5. 实现多类库架构、登录缓存、JSON设置等功能

#### 使用的组件包（v2.0.0）

1. CommunityToolkit.Mvvm：官方轻量级MVVM框架
2. Microsoft.EntityFrameworkCore.Sqlite：SQLite数据库驱动程序，用于EF Core操作SQLite。
3. Microsoft.EntityFrameworkCore.Tools：EF Core命令行工具支持（如迁移、脚手架）。
4. Microsoft.Extensions.Configuration：通用的配置抽象，支持多种配置源。
5. Microsoft.Extensions.Configuration.Json：JSON文件配置源。
6. Microsoft.Extensions.Hosting：后台服务托管和应用程序生命周期管理。
7. Microsoft.Extensions.Logging：日志记录抽象和基础框架。
8. Microsoft.Xaml.Behaviors.Wpf：WPF交互行为库，用于在XAML中处理事件和命令。
9. Serilog.Extensions.Logging：Serilog与Microsoft.Extensions.Logging的桥接。
10. Serilog.Settings.Configuration：从配置文件读取Serilog设置。
11. Serilog.Sinks.Console：Serilog控制台输出目标。
12. Serilog.Sinks.File：Serilog文件输出目标。
13. WPF-UI：WPF现代化UI控件库。
14. WPF-UI.DependencyInjection：WPF-UI与依赖注入的集成。
15. WPF-UI.Tray：WPF-UI系统托盘支持。
16. WPF-UI.Violeta：WPF-UI扩展包（附加主题或控件）。
17. Microsoft.Extensions.DependencyInjection.Abstractions：依赖注入抽象接口。
18. BCrypt.Net-Next：BCrypt密码哈希库（Next版本）。
19. System.Security.Cryptography.ProtectedData：Windows数据保护API（DPAPI），用于加密数据。

#### 贡献指南

欢迎提交 Issue 或 Pull Request，以修复未知的漏洞或添加新功能

#### 许可证

###### 本项目代码

Copyright (c)  2026 GhostErrorCode
本项目采用 **GNU General Public License v3.0 (GPL-3.0)** 开源协议。  
完整的许可证文本请查看根目录下的 [LICENSE](https://LICENSE) 文件。
