using MaterialDesignThemes.Wpf;
using MemoDesktop.Services.Interfaces;
using MemoDesktop.ViewModels.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace MemoDesktop.Services.Implements
{
    /// <summary>
    /// 基于DialogHost的对话框服务实现类
    /// 大白话：这是实际干活儿的"对话框管家"，负责在指定位置打开对话框
    /// </summary>
    public class DialogHostService : DialogService, IDialogHostService
    {
        /// <summary>
        /// 依赖注入容器扩展接口
        /// 大白话：这是一个"对象工厂"，负责根据名称创建对话框实例
        /// </summary>
        private readonly IContainerExtension _containerExtension;

        /// <summary>
        /// 构造函数，初始化对话框服务
        /// </summary>
        /// <param name="containerExtension">
        /// 依赖注入容器
        /// 大白话：把"对象工厂"传进来，这样就能按需生产对话框了
        /// </param>
        public DialogHostService(IContainerExtension containerExtension) : base(containerExtension)
        {
            this._containerExtension = containerExtension;
        }

        /// <summary>
        /// 在指定对话框主机中显示对话框
        /// 大白话：在指定位置打开一个对话框窗口，并等待用户操作
        /// </summary>
        /// <param name="name">对话框名称</param>
        /// <param name="parameters">对话框参数</param>
        /// <param name="dialogHostName">对话框主机名称</param>
        /// <returns>对话框操作结果</returns>
        public async Task<IDialogResult> ShowDialog(string name, IDialogParameters parameters, string dialogHostName = "RootDialog")
        {
            // 1. 参数安全检查
            // 大白话：如果没传参数，就创建一个空的参数包
            if (parameters == null) {  parameters = new DialogParameters(); }

            // 2. 创建对话框内容
            // 大白话：找"对象工厂"要一个对话框实例，比如"用户编辑框"
            // 因Object太过于广泛，这里使用 FrameworkElement 

            // FrameworkElement 是 WPF（Windows Presentation Foundation）框架中的核心基类，专门用于构建具备完整 WPF 框架功能的 UI 元素 - 
            // 在你的 WPF+Prism+MVVM 开发环境中，几乎所有可视化且具备布局、数据绑定等高级功能的控件都直接或间接继承自该类。
            // Object → DispatcherObject → DependencyObject → Visual → UIElement → FrameworkElement
            // （后续常用控件如Button、TextBox、Grid、Window均继承自FrameworkElement）
            FrameworkElement content = this._containerExtension.Resolve<FrameworkElement>(name);

            // 3. 类型检查：必须是WPF控件
            // 大白话：检查拿到的到底是不是一个真正的对话框界面

            // 这段代码属于防御性编程，常用于 Prism 对话框服务、MaterialDesign 对话框加载等场景，确保后续操作的content是一个可正常使用的 WPF 可视化元素
            // 避免后续访问FrameworkElement特有属性（如DataContext、Width）时出现类型转换失败、空引用等运行时错误

            // is 是 .NET 中的模式匹配运算符
            // ① 检查左侧content是否是（或继承自）右侧的FrameworkElement类型；
            // ② 若类型检查通过（返回true），则自动将content安全转换为FrameworkElement类型，并赋值给右侧的局部变量dialogContent，无需手动执行强制类型转换（如(FrameworkElement)content）。
            if (!(content is FrameworkElement dialogContent))
            {
                // 错误信息大白话：这压根不是个对话框界面啊！赶紧检查注册的名字对不对！
                throw new NullReferenceException("这压根不是个对话框界面啊！赶紧检查注册的名字对不对！");
            }

            // 4. 手动从容器获取 ViewModel（不用 ViewModelLocator！）
            if (dialogContent.DataContext == null)
            {
                // 🔥 关键：通过映射表从容器获取 ViewModel
                object viewModel = GetViewModelFromContainer(name);
                dialogContent.DataContext = viewModel;
            }

            // 4. 确保对话框有ViewModel
            // 大白话：如果对话框背后没有"管家"（ViewModel），就给它自动配一个
            //if (dialogContent is FrameworkElement view && view.DataContext is null && ViewModelLocator.GetAutoWireViewModel(view) is null)
            //{
            //    // 大白话：给这个对话框界面绑定一个自动找来的ViewModel
            //    ViewModelLocator.SetAutoWireViewModel(view, true);
            //}

            // 5. 检查ViewModel类型
            // 大白话：对话框的"管家"必须知道怎么处理DialogHost
            if (!(dialogContent.DataContext is IDialogHostAware viewModelAware))
            {
                // 错误信息大白话：这个对话框的"管家"不合格！不会操作DialogHost！
                throw new NullReferenceException("这个对话框的\"管家\"不合格！不会操作DialogHost！");
            }

            // 6. 设置对话框要在哪个区域显示
            // 大白话：告诉对话框"管家"，一会儿要在哪个"舞台"上表演
            viewModelAware.DialogHostName = dialogHostName;

            // 7. 定义对话框打开后的处理逻辑
            // 大白话：等对话框一打开，就要做两件事：
            //   ① 把参数传给对话框的"管家"
            //   ② 更新对话框显示内容
            DialogOpenedEventHandler dialogOpenedEventHandler = (sender, eventArgs) =>
            {
                if(viewModelAware is IDialogHostAware dialogHostAware)
                {
                    dialogHostAware.OnDialogOpened(parameters);
                }
                eventArgs.Session.UpdateContent(content);
            };

            // 8. 真正显示对话框
            // 大白话：现在一切准备就绪，在指定"舞台"上正式打开对话框！
            return (IDialogResult)await DialogHost.Show(dialogContent, viewModelAware.DialogHostName, dialogOpenedEventHandler);
        }

        private object GetViewModelFromContainer(string viewName)
        {
            // 根据View的名称获取对应ViewModel
            switch (viewName)
            {
                case "AddToDoDialog": return this._containerExtension.Resolve<AddToDoDialogViewModel>();
                case "AddMemoDialog": return this._containerExtension.Resolve<AddToDoDialogViewModel>();
            }
            return null;
        }
    }
}
