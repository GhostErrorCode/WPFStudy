using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Text;
using Wpf.Ui.Controls;
using WpfUiTest.App.Views.Main;
using WpfUiTest.Shared.Base;

namespace WpfUiTest.App.ViewModels.Main
{
    // MainView中Footer区域的菜单列表ViewModel
    public class MainFooterMenuViewModel : BaseViewModel
    {
        // 属性：MainView的Footer区域菜单列表(这里用object类型是保证了灵活性，可以在集合中添加Separator分割线)
        private ObservableCollection<object> _footerMenuItems;
        public ObservableCollection<object> FooterMenuItems
        {
            get { return _footerMenuItems; }
            set { SetProperty(ref _footerMenuItems, value); }
        }


        // ========= 构造函数 ===========
        public MainFooterMenuViewModel()
        {
            // 初始化
            this._footerMenuItems = new ObservableCollection<object>()
            {
                new NavigationViewItem("设置", SymbolRegular.Settings24, typeof(SettingsView)),
            };
        }
    }
}
