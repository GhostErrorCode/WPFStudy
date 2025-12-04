using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Text;

namespace MemoDesktop.Models
{
    // 菜单栏的Model，包含图标，名称，命名空间
    public class MenuBarModel
    {
        /// <summary>
        /// 菜单图标
        /// PackIconKind是UI组件的
        /// </summary>
        private PackIconKind _icon;
        public PackIconKind Icon
        {
            get { return _icon; } 
            set { _icon = value; }
        }

        /// <summary>
        /// 菜单名称
        /// </summary>
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Prism 导航的 ViewName（对应已注册的 View），例如 "DashboardView"
        /// </summary>
        private string _navigationpath;
        public string NavigationPath
        {
            get { return _navigationpath; }
            set { _navigationpath = value; }
        }
    }
}
