using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WpfUiTest.Shared.Base;
using WpfUiTest.Shared.Models;
using WpfUiTest.Shared.Utilities;

namespace WpfUiTest.App.ViewModels.Main
{
    public class SettingsAboutViewModel : BaseViewModel
    {
        // 属性: 应用名称
        public string AppName { get; set; } = string.Empty;
        // 属性: 版本号
        public string Version { get; set; } = string.Empty;
        // 属性: Github仓库地址
        public string GitHubRepositoryAddress { get; set; } = string.Empty;
        // 属性: 克隆Github仓库地址
        public string CloneGitHubRepositoryAddress { get; set; } = string.Empty;
        // 属性: 数据库目录
        public string DataBaseDirectory { get; set; } = string.Empty;
        // 属性: 日志目录
        public string LogDirectory { get; set; } = string.Empty;
        // 属性: 登录凭证目录
        public string LoginCredentialDirectory { get; set; } = string.Empty;
        // 属性: 设置目录
        public string SettingsDirectory { get; set; } = string.Empty;
        // 属性：声明和致谢
        public string DeclarationAndAcknowledgements { get; set; } = string.Empty;


        // ============ 构造函数 ==============
        public SettingsAboutViewModel()
        {
            // 读取配置
            AppConfiguration appConfiguration = AppConfigurationHelper.LoadMergeSettings();

            // 属性赋值
            this.AppName = appConfiguration.AppSettings.AppName;
            this.Version = appConfiguration.AppSettings.Version;
            this.GitHubRepositoryAddress = "https://github.com/GhostErrorCode/WPFStudy";
            this.CloneGitHubRepositoryAddress = "git https://github.com/GhostErrorCode/WPFStudy.git";
            this.DataBaseDirectory = Path.Combine(AppContext.BaseDirectory,appConfiguration.DataBaseSettings.Directory);
            this.LogDirectory = Path.Combine(AppContext.BaseDirectory, appConfiguration.LogSettings.Directory);
            this.LoginCredentialDirectory = Path.Combine(AppContext.BaseDirectory, appConfiguration.LoginCredentialSettings.Directory);
            this.SettingsDirectory = Path.Combine(AppContext.BaseDirectory, "Settings");

            this.DeclarationAndAcknowledgements = 
                $"This software uses the WPF UI library (https://github.com/lepoco/wpfui), {Environment.NewLine}" +
                $"licensed under the MIT License. {Environment.NewLine}" +
                $"Copyright (c) 2021-2025 Leszek Pomianowski and WPF UI Contributors." +
                $"";
        }
    }
}
