using PrismEventAggregator.Events;
using PrismEventAggregator.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace PrismEventAggregator.ViewModels
{
    /// <summary>
    /// 主窗口的视图模型
    /// 包含发布事件的命令和逻辑
    /// </summary>
    public class MainWindowViewModel : BindableBase
    {
        /// <summary>
        /// 事件聚合器实例
        /// 用于事件的发布和订阅管理
        /// </summary>
        private readonly IEventAggregator _eventAggregator;

        /// <summary>
        /// 发布事件的命令
        /// 绑定到UI按钮的Command属性
        /// </summary>
        public DelegateCommand PublishEventCommand { get; private set; }

        /// <summary>
        /// 构造函数，通过依赖注入获取事件聚合器实例
        /// </summary>
        /// <param name="eventAggregator">事件聚合器实例</param>
        public MainWindowViewModel(IEventAggregator eventAggregator)
        {
            // 保存事件聚合器实例的引用
            this._eventAggregator = eventAggregator;

            // 初始化发布事件命令，绑定到PublishEvent方法
            this.PublishEventCommand = new DelegateCommand(this.PublishEvent);
        }

        /// <summary>
        /// 发布事件的方法
        /// 创建人员信息并通过事件聚合器发布
        /// </summary>
        private void PublishEvent()
        {
            // 创建新的人员信息对象
            PersonInfo personInfo = new PersonInfo()
            {
                Name = "张三",
                Age = 25,
                Sex = "男"
            };

            // 通过事件聚合器获取事件并发布人员信息
            this._eventAggregator.GetEvent<PersonInfoEvent>().Publish(personInfo);
        }
    }
}
