using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Wpf.Ui.Controls;
using WpfUiTest.App.Views.Main;
using WpfUiTest.Modules.Memo.Views;
using WpfUiTest.Modules.ToDo.Views;
using WpfUiTest.Shared.Base;

namespace WpfUiTest.App.ViewModels.Main
{
    // MainView中主区域区域的菜单列表ViewModel
    public class MainMenuViewModel : BaseViewModel
    {
        // 属性：MainView的Footer区域菜单列表(这里用object类型是保证了灵活性，可以在集合中添加Separator分割线)
        private ObservableCollection<object> _menuItems;
        public ObservableCollection<object> MenuItems
        {
            get { return _menuItems; }
            set { SetProperty(ref _menuItems, value); }
        }


        // ========= 构造函数 ===========
        public MainMenuViewModel()
        {
            // 初始化
            this._menuItems = new ObservableCollection<object>()
            {
                new NavigationViewItem("首页", SymbolRegular.Home24, typeof(IndexView)),
                new NavigationViewItemSeparator(),
                new NavigationViewItem()
                {
                    Content = "待办&备忘",
                    Icon = new SymbolIcon { Symbol = SymbolRegular.BookNumber24 },
                    MenuItemsSource = new object[]
                    {
                        new NavigationViewItem("待办事项", SymbolRegular.TaskListLtr24, typeof(ToDoView)),
                        new NavigationViewItem("备忘录", SymbolRegular.Notebook24, typeof(MemoView))
                    }
                }
                /*
                // 这里也是可以写页面的，会同时打开页面并展开子项
                new NavigationViewItem("Basic Input", SymbolRegular.CheckboxChecked24, typeof(BasicInputPage))
                {
                    MenuItemsSource = new object[]
                    {
                        new NavigationViewItem(nameof(Anchor), typeof(AnchorPage)),
                        new NavigationViewItem(nameof(Wpf.Ui.Controls.Button), typeof(ButtonPage))
                    },
                }
                */
            };
        }
    }
}
