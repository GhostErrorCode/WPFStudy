using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using WpfUiTest.Modules.Memo.Views;

namespace WpfUiTest.Modules.Memo
{
    // 依赖注入容器，用于将类库的组件注册到全局的Host中
    public static class DependencyInjection
    {
        // 只安装抽象包即可 Microsoft.Extensions.DependencyInjection.Abstractions
        public static IServiceCollection AddModuleMemo(this IServiceCollection services)
        {
            // 注册View
            services.AddSingleton<MemoView>();

            // 注册ViewModel
            // services.AddTransient<SettingsViewModel>();

            // 如果模块有其他服务，也在这里注册


            return services;
        }
    }
}
