using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;
using Wpf.Ui.Abstractions.Controls;

namespace WpfUiTest.Shared.Base
{
    // ViewModel基类
    public class BaseViewModel : ObservableObject, INavigationAware
    {
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
    }
}
