using DryIoc.Messages;
using MemoDesktop.Events;
using System;
using System.Collections.Generic;
using System.Text;

// 此基类已弃用，转为NavigationViewModel作为基类
namespace MemoDesktop.ViewModels
{
    /// <summary>
    /// ViewModel基类
    /// 提供加载动画控制等公共功能，继承Prism的BindableBase以支持属性更改通知
    /// </summary>
    public class ViewModelBase : BindableBase
    {
        // 事件聚合器
        private readonly IEventAggregator _eventAggregator;

        // 构造函数(通过依赖注入获取全局的事件聚合器)
        public ViewModelBase(IEventAggregator eventAggregator)
        {
            this._eventAggregator = eventAggregator;
        }

        /// <summary>
        /// 显示加载动画
        /// </summary>
        /// <param name="message">加载提示消息，默认为"正在加载..."</param>
        /// <param name="moduleName">模块名称，默认为当前类名</param>
        protected void ShowLoading(string message = "正在加载...", string moduleName = "")
        {
            // 如果moduleName是空的话，就获取当前类型名称
            if (string.IsNullOrEmpty(moduleName))
            {
                moduleName = this.GetType().Name;
            }

            // 新建一个事件参数
            UpdateLoadingEventArgs args = new UpdateLoadingEventArgs()
            {
                IsOpen = true,
                Message = message,
                ModuleName = moduleName
            };

            // 发布加载动画事件 - 打开
            this._eventAggregator.GetEvent<UpdateLoadingEvent>().Publish(args);
        }

        /// <summary>
        /// 隐藏加载动画
        /// </summary>
        protected void HideLoading()
        {
            // 新建一个事件参数
            UpdateLoadingEventArgs args = new UpdateLoadingEventArgs()
            {
                IsOpen = false,
                Message = string.Empty
            };
            // 发布加载动画事件 - 关闭
            this._eventAggregator.GetEvent<UpdateLoadingEvent>().Publish(args);
        }
    }
}
