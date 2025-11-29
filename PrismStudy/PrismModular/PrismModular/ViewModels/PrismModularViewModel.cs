using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace PrismModular.ViewModels
{
    // 继承Prism的Mvvm
    public class PrismModularViewModel : BindableBase
    {
        // 公共属性，实现 ICommand 接口，用于按钮命令绑定
        public DelegateCommand<string>? OpenCommand { get; private set; }

        // 定义区域管理器接口
        private readonly IRegionManager _regionManager;

        // 构造函数，初始化 ViewModel
        public PrismModularViewModel(IRegionManager regionManager)
        {
            // 创建 DelegateCommand 实例
            // 构造函数参数是一个 Action 委托，指向 Open 方法
            OpenCommand = new DelegateCommand<string>(Open);

            // 获取全局区域管理器到当前类的_regionManager字段
            // 将 Prism 依赖注入（DI）容器通过构造函数传入的 IRegionManager 实例，赋值给当前类的私有字段 _regionManager，让类的其他方法能复用这个实例完成区域管理和视图导航
            _regionManager = regionManager;
        }

        private void Open(string str)
        {
            // 在 PrismRegionContentControl 区域中导航到视图
            _regionManager.Regions["PrismModularContentControl"].RequestNavigate(str);
            // _regionManager.RequestNavigate("PrismRegionContentControl", str);
        }

    }
}
