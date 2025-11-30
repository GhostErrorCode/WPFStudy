using PrismEventAggregator.Events;
using PrismEventAggregator.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace PrismEventAggregator.ViewModels
{
    /// <summary>
    /// 订阅者视图模型
    /// 负责订阅和处理人员信息事件
    /// </summary>
    public class SubscriberViewModel : BindableBase
    {
        /// <summary>
        /// 显示的消息内容
        /// </summary>
        private String _displayMessage;

        /// <summary>
        /// 显示消息的公共属性
        /// 用于UI界面绑定显示
        /// </summary>
        public String DisplayMessage
        {
            get { return this._displayMessage; }
            set { SetProperty(ref _displayMessage, value); }
        }

        /// <summary>
        /// 构造函数，订阅人员信息事件
        /// </summary>
        /// <param name="eventAggregator">事件聚合器实例</param>
        public SubscriberViewModel(IEventAggregator eventAggregator)
        {
            // 初始化显示消息
            this._displayMessage = "等待接收人员信息...";

            // 订阅人员信息事件，当事件发布时调用HandlePersonInfo方法
            eventAggregator.GetEvent<PersonInfoEvent>().Subscribe(this.HandlePersonInfo);
        }

        /// <summary>
        /// 处理接收到的人员信息
        /// 事件发布时会自动调用此方法
        /// </summary>
        /// <param name="personInfo">接收到的人员信息对象</param>
        private void HandlePersonInfo(PersonInfo personInfo)
        {
            // 更新显示消息，显示接收到的人员信息
            this.DisplayMessage = $"已接收到人员信息：{personInfo.ToString()}";
            Debug.WriteLine(this.GetHashCode());
        }
    }
}
