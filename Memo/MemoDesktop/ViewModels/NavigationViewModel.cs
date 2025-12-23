using MemoDesktop.Events;
using MemoDesktop.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MemoDesktop.ViewModels
{
    // 导航、加载动画、ViewModel的基类
    // 建立一套标准的、可扩展的、带有默认实现的导航生命周期框架
    public class NavigationViewModel : BindableBase, INavigationAware
    {
        // 事件聚合器
        private readonly IEventAggregator _eventAggregator;

        // 构造函数(通过依赖注入获取全局的事件聚合器)
        public NavigationViewModel(IEventAggregator eventAggregator)
        {
            this._eventAggregator = eventAggregator;
        }


        /// <summary>
        /// 确定当前实例是否可以重用为导航目标
        /// </summary>
        public virtual bool IsNavigationTarget(NavigationContext navigationContext)
        {
            // 决定是否重用现有的ViewModel实例
            // throw new NotImplementedException();
            // 永远重用实例
            return true;
        }

        // 设置成虚方法，以便子类重写或调用父类方法
        // 当从当前ViewModel对应的视图导航离开时调用
        public virtual void OnNavigatedFrom(NavigationContext navigationContext)
        {
            // throw new NotImplementedException();
        }

        // 设置成虚方法，以便子类重写或调用父类方法
        // 当导航到这个ViewModel对应的视图时调用
        // 注意：只有通过Region导航到页面才会触发OnNavigatedTo方法
        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {
            // this.GetType().Name是获取实例的名称（子类）
            Debug.WriteLine(this.GetType().Name);
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
            this._eventAggregator.UpdateLoadingEventPublish(args);
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
            this._eventAggregator.UpdateLoadingEventPublish(args);
        }
    }
}
