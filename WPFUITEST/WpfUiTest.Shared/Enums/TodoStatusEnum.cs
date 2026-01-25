using System;
using System.Collections.Generic;
using System.Text;

namespace WpfUiTest.Shared.Enums
{
    /// <summary>
    /// 待办事项状态枚举（精简版）
    /// 仅包含业务所需的“待办”和“已完成”两种状态
    /// 数据库中存储枚举的int值：0=待办，1=已完成
    /// </summary>
    public enum TodoStatusEnum
    {
        /// <summary>
        /// 待办（默认状态）
        /// 待办事项创建后未完成的初始状态
        /// 数据库存储值：0
        /// </summary>
        Pending = 0,

        /// <summary>
        /// 已完成
        /// 待办事项处理完毕的状态
        /// 数据库存储值：1
        /// </summary>
        Completed = 1
    }
}
