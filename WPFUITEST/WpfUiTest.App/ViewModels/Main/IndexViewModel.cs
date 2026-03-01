using System;
using System.Collections.Generic;
using System.Text;
using WpfUiTest.Shared.Base;

namespace WpfUiTest.App.ViewModels.Main
{
    // 首页的ViewModel
    public class IndexViewModel : BaseViewModel
    {
        // ==================== 字段属性命令 ====================
        // 属性: 首页标题ViewModel
        private IndexTitleViewModel _indexTitleViewModel;
        public IndexTitleViewModel IndexTitleViewModel
        {
            get { return _indexTitleViewModel; }
            set { SetProperty(ref _indexTitleViewModel, value); }
        }


        // ==================== 构造函数 ====================
        public IndexViewModel(IndexTitleViewModel indexTitleViewModel)
        {
            // 初始化字段
            // 初始化属性
            this._indexTitleViewModel = indexTitleViewModel;
            // 初始化命令
        }


        // ==================== 方法 ====================
        // 方法: 导航完成
        public override async Task OnNavigatedToAsync()
        {
            // 调用子VM此方法
            await this._indexTitleViewModel.OnNavigatedToAsync();
            await base.OnNavigatedToAsync();
        }
    }
}
