using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace WpfUiTest.Shared.Utilities
{
    // ContentPresenter 是 WPF 专门用来 “渲染 DataTemplate + 数据（VM）” 的「模板渲染器」
    // 此方法用于静态创建ContentPresenter
    public static class ContentPresenterHelper
    {
        // 创建ContentPresenter
        public static ContentPresenter Build(object content, object contentTemplate)
        {
            return new ContentPresenter()
            {
                Content = content,
                ContentTemplate = (DataTemplate)contentTemplate
            };
        }
    }
}
