using Prism.Ioc;  // 引入Prism依赖注入容器相关类
using Prism.Modularity;  // 引入Prism模块化相关类
using System;
using System.Collections.Generic;
using System.Text;
using UserControllerModule.ViewModels;
using UserControllerModule.Views;

namespace UserControllerModule
{
    /// <summary>
    /// UserControllerModule 用户控制器模块类
    /// 这个类实现IModule接口，用于注册模块中的类型和初始化模块
    /// </summary>
    public class UserControllerModule : IModule  // 实现IModule接口
    {
        /// <summary>
        /// OnInitialized 模块初始化方法
        /// 当模块加载完成后，Prism会调用此方法进行模块初始化
        /// </summary>
        /// <param name="containerProvider">容器提供者，用于解析依赖</param>
        public void OnInitialized(IContainerProvider containerProvider)
        {
            // 从容器提供者中解析区域管理器实例
            IRegionManager regionManager = containerProvider.Resolve<IRegionManager>();
            // 这里可以进行模块级别的区域注册，当前示例中在主程序注册
        }

        /// <summary>
        /// RegisterTypes 注册类型方法
        /// 在这个方法中注册模块中需要依赖注入的类型
        /// </summary>
        /// <param name="containerRegistry">容器注册器，用于注册类型</param>
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // 注册UserView和UserViewModel的导航映射
            // "UserView"是导航时使用的名称，UserView是视图类型，UserViewModel是视图模型类型
            containerRegistry.RegisterForNavigation<UserView, UserViewModel>("UserView");
        }
    }
}
