using PrismModuleA.Views;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrismModuleA
{
    // 继承IModule接口
    public class ModuleAProfile : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            // 模块初始化完成后的操作
            // 可以在这里进行初始导航等操作
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // 注册模块内的视图
            containerRegistry.RegisterForNavigation<ViewA>("ViewA");

            // 注册模块内的服务
            // containerRegistry.Register<ICustomerService, CustomerService>();
        }
    }
}
