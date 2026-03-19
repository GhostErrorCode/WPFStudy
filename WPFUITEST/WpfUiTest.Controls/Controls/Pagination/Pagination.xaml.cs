using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfUiTest.Controls.Models;

namespace WpfUiTest.Controls.Controls.Pagination
{
    /// <summary>
    /// Pagination.xaml 的交互逻辑
    /// </summary>
    public partial class Pagination : UserControl
    {
        // ==================== 依赖属性 ====================
        // 总页数
        public static readonly DependencyProperty TotalPagesProperty =
            DependencyProperty.Register(
                nameof(TotalPages),
                typeof(int),
                typeof(Pagination),
                new PropertyMetadata(0, OnPagingPropertyChanged)
                );
        // 总条数
        public static readonly DependencyProperty TotalCountProperty =
            DependencyProperty.Register(
                nameof(TotalCount),
                typeof(int),
                typeof(Pagination),
                new PropertyMetadata(0, OnPagingPropertyChanged)
                );
        // 页大小（每页显示多少条）
        public static readonly DependencyProperty PageSizeProperty =
            DependencyProperty.Register(
                nameof(PageSize),
                typeof(int),
                typeof(Pagination),
                new PropertyMetadata(10, OnPagingPropertyChanged)
                );
        // 当前页码
        public static readonly DependencyProperty CurrentPageProperty =
            DependencyProperty.Register(
                nameof(CurrentPage),
                typeof(int),
                typeof(Pagination),
                new PropertyMetadata(1, OnPagingPropertyChanged)
                );
        // 当页码改变时触发的命令，参数为 int 类型的新页码
        public static readonly DependencyProperty PaginationChangedCommandProperty =
            DependencyProperty.Register(
                nameof(PaginationChangedCommand),
                typeof(ICommand),
                typeof(Pagination),
                new PropertyMetadata(null)
                );
        // 页大小ComboBox选择源
        public static readonly DependencyProperty PageSizeOptionsProperty =
            DependencyProperty.Register(
                nameof(PageSizeOptions),
                typeof(IEnumerable<int>),
                typeof(Pagination),
                new PropertyMetadata(new List<int> { 10, 20, 50, 100 })
                );

        // ==================== CLR包装属性 ====================
        // 总页数
        public int TotalPages
        {
            get { return (int)GetValue(TotalPagesProperty); }
            set { SetValue(TotalPagesProperty, value); }
        }
        // 总条数
        public int TotalCount
        {
            get => (int)GetValue(TotalCountProperty);
            set => SetValue(TotalCountProperty, value);
        }
        // 页大小（每页显示多少条）

        public int PageSize
        {
            get => (int)GetValue(PageSizeProperty);
            set => SetValue(PageSizeProperty, value);
        }
        // 当前页码
        public int CurrentPage
        {
            get { return (int)GetValue(CurrentPageProperty); }
            set { SetValue(CurrentPageProperty, value); }
        }
        // 当页码改变时触发的命令，参数为 int 类型的新页码
        public ICommand PaginationChangedCommand
        {
            get { return (ICommand)GetValue(PaginationChangedCommandProperty); }
            set { SetValue(PaginationChangedCommandProperty, value); }
        }
        // 页大小ComboBox选择源
        public IEnumerable<int> PageSizeOptions
        {
            get => (IEnumerable<int>)GetValue(PageSizeOptionsProperty);
            set => SetValue(PageSizeOptionsProperty, value);
        }
        // 内部绑定的页码项集合，用于生成带省略号及页码或前进后退的按钮列表
        public ObservableCollection<PageItem> PageItems { get; } = new ObservableCollection<PageItem>();
        //  有效总页数计算（核心）（TotalPages优先，如果TotalPages为0就以总条数和当前页大小为准计算）
        private int EffectiveTotalPages
        {
            get
            {
                if (this.TotalPages > 0)
                    return this.TotalPages;
                if (this.TotalCount > 0 && this.PageSize > 0)
                    // Math.Ceiling向上取整（天花板函数）
                    return (int)Math.Ceiling((double)TotalCount / PageSize);
                return 1; // 至少一页
            }
        }

        // 内部命令，用于上一页、下一页或页码点击时变更当前页，及校验当前页码是否合法
        public RelayCommand<int> InternalPageChangedCommand { get; private set; }


        // ==================== 构造函数 ====================
        public Pagination()
        {
            // 内部命令Command用于切换当前页
            this.InternalPageChangedCommand = new RelayCommand<int>((int page) =>
            {
                // 将传入的参数页码page赋值给当前页CurrentPage
                this.CurrentPage = page;
                // 生成最终显示的页码按钮（带...）
                // this.GeneratePageItems();
                // 通知ViewModel（对外暴露的ICommand），当前总页、总数、页大小等变更了，需要重新加载数据
                // this.PageChangedCommand?.Execute(this.CurrentPage);

            });

            // 最后做初始化
            InitializeComponent();
        }


        // ==================== 方法 ====================
        // 方法：静态属性变化回调
        private static void OnPagingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // 只在页大小，当前页码变动时才需要调用外部方法，否则只刷新页码列表即可，防止循环调用外部方法
            if(e.Property == PageSizeProperty || e.Property == CurrentPageProperty)
            {
                // 通知ViewModel（对外暴露的ICommand），当前总页、总数、页大小等变更了，需要重新加载数据
                ((Pagination)d).PaginationChangedCommand?.Execute(((Pagination)d).CurrentPage);
            }

            // 只要总页数、当前页、页大小等变化，就重新生成页码列表（将参数 d 转为此控件，并调用此控件的重新计算 PageItems 方法）
            ((Pagination)d).GeneratePageItems();
        }

        /// <summary>
        /// 根据当前 TotalPages 和 CurrentPage，重新计算 PageItems 集合。
        /// 算法逻辑：
        /// - 按钮始最多为9个
        /// - 始终显示第一页和最后一页。
        /// - 显示当前页前后各2页。
        /// - 如果出现断层（页码差大于1），用省略号代替。省略号按钮可一次性跳5页
        /// - 总页数 ≤ 9 时全部显示，不使用省略号。
        /// </summary>
        private void GeneratePageItems()
        {
            // =============== 页码规则校验 ===============
            // 当前页不能小于1
            if (this.CurrentPage < 1) this.CurrentPage = 1;
            // 当前页不能大于总页数
            if (this.CurrentPage > this.EffectiveTotalPages) this.CurrentPage = this.EffectiveTotalPages;
            /*
            // 判断是否到达首页或尾页，如果达到将前进/后退按钮设置为不可用
            if (this.CurrentPage <= 1 && this.CurrentPage < this.EffectiveTotalPages)
            {
                // 如果当前页码小于等于1，且比最大页码小，表明到第一页了，禁用上一页按钮，开放下一页按钮
                PreviousPageButton.IsEnabled = false;
                NextPageButton.IsEnabled = true;
            }
            else if (this.CurrentPage >= this.EffectiveTotalPages && this.CurrentPage > 1)
            {
                // 如果当前页码大于等于最大页且当前页码大于1，表明到最后一页了，禁用下一页按钮，开发上一页按钮
                PreviousPageButton.IsEnabled = true;
                NextPageButton.IsEnabled = false;
            }
            else if(this.CurrentPage == 1 && this.CurrentPage == this.EffectiveTotalPages)
            {
                // 如果当前页码等于1且等于最大页码，则表明只有1页，上一页与下一页按钮全部禁用
                PreviousPageButton.IsEnabled = false;
                NextPageButton.IsEnabled = false;
            }
            else
            {
                // 剩下的情况，当前页不是第一页也不是尾页，则上一页和下一页按钮均可用
                PreviousPageButton.IsEnabled = true;
                NextPageButton.IsEnabled = true;
            }
            */

            // 更新上一页、下一页按钮状态是否可用
            PreviousPageButton.IsEnabled = this.CurrentPage > 1;
            NextPageButton.IsEnabled = this.CurrentPage < this.EffectiveTotalPages;
            // 更新总条数TextBlock
            TotalCountTextBlock.Text = this.TotalCount > 0 ? $"共 {this.TotalCount} 条" : "";
            // =============================================

            // 清理PageItems列表
            this.PageItems.Clear();

            // 拿到总页数和当前页
            int total = this.EffectiveTotalPages;
            int current = this.CurrentPage;

            // 如果页数 <= 9，则全部显示
            if(total <= 9)
            {
                for(int i = 1; i <= total; i++)
                {
                    this.PageItems.Add(new PageItem()
                    {
                        DisplayText = i.ToString(),
                        PageNumber = i,
                        IsEnabled = true,
                        IsCurrent = (i == current)
                    });

                }
                return;
            }

            // 中间连续的可见页码按钮数量：中间显示的连续页码个数（包括当前页），取5可保证最多9个按钮
            int visiblePageCount = 5;
            int pageOffset = visiblePageCount / 2; // 2

            // 计算窗口起始和结束，确保窗口包含当前页，且不超过总页数范围
            int start = Math.Max(2, current - pageOffset);  // 强制中间区域最小从 2 开始，避免和首页重复
            int end = Math.Min(total - 1, start + visiblePageCount - 1);  // 强制中间区域最大到 总页数-1，避免和尾页重复

            // 如果可见页码按钮数量不足 visiblePageCount，调整起始位置（尽量向右靠，但不超过总页数）
            if (end - start + 1 < visiblePageCount)
            {
                start = Math.Max(2, end - visiblePageCount + 1);
            }

            // 第一页
            PageItems.Add(new PageItem
            {
                DisplayText = "1",
                PageNumber = 1,
                IsEnabled = true,
                IsCurrent = (current == 1)
            });

            // 左边是否需要省略号
            if (start > 2)
            {
                PageItems.Add(new PageItem
                {
                    DisplayText = "...",
                    // 左侧的省略号表示从当前页一次向前滚5页
                    PageNumber = current - visiblePageCount >= 1 ? current - visiblePageCount : 1,
                    IsEnabled = true
                });
            }

            // 中间连续页码
            for (int i = start; i <= end; i++)
            {
                PageItems.Add(new PageItem
                {
                    DisplayText = i.ToString(),
                    PageNumber = i,
                    IsEnabled = true,
                    IsCurrent = (i == current)
                });
            }

            // 右边是否需要省略号
            if (end < total - 1)
            {
                PageItems.Add(new PageItem
                {
                    DisplayText = "...",
                    // 右侧的省略号表示表示从当前页一次向后滚5页
                    PageNumber = current + visiblePageCount <= total ? current + visiblePageCount : total,
                    IsEnabled = true
                });
            }

            // 最后一页
            PageItems.Add(new PageItem
            {
                DisplayText = total.ToString(),
                PageNumber = total,
                IsEnabled = true,
                IsCurrent = (current == total)
            });
        }
    }
}
