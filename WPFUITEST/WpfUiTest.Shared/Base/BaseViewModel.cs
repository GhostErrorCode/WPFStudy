using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using Wpf.Ui;
using Wpf.Ui.Abstractions.Controls;
using Wpf.Ui.Controls;

namespace WpfUiTest.Shared.Base
{
    // ViewModel基类
    public class BaseViewModel : ObservableObject, INavigationAware
    {
        // ==================== 字段属性 ====================
        // 字段: IServiceProvider DI容器服务
        // protected readonly IServiceProvider _serviceProvider;

        // ==================== 构造函数 ====================
        public BaseViewModel()
        {
            // 初始化字段
            // this._serviceProvider = serviceProvider;

            // 初始化属性

            // 初始化命令
        }

        // ==================== 方法 ====================
        // 导航离开当前页面时触发
        public virtual Task OnNavigatedFromAsync()
        {
            return Task.CompletedTask;
        }

        // 导航到当前页面时触发（页面已显示）

        public virtual Task OnNavigatedToAsync()
        {
            return Task.CompletedTask;
        }

        // 自定义对话服务方法: 显示对话、
        /*
        protected async Task<ContentDialogResult> ShowDialogAsync(object title, object content, string primaryButtonText)
        {
            return await this._contentDialogService.ShowAsync(new ContentDialog()
            {
                Title = title,
                Content = content,
                PrimaryButtonText = primaryButtonText
            },CancellationToken.None);
        }
        */
    }
}
