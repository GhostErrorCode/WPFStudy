using System;
using System.Collections.Generic;
using System.Text;

namespace PrismEventAggregator.Models
{
    /// <summary>
    /// 人员信息模型类
    /// 包含人员的基本信息属性
    /// </summary>
    public class PersonInfo
    {
        /// <summary>
        /// 人员姓名
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// 人员年龄
        /// </summary>
        public Int32 Age { get; set; }

        /// <summary>
        /// 人员性别
        /// </summary>
        public String Sex { get; set; }

        /// <summary>
        /// 重写ToString方法，返回格式化的人员信息
        /// </summary>
        /// <returns>格式化的字符串</returns>
        public override String ToString()
        {
            String report = $"姓名：{Name}，年龄：{Age}，性别：{Sex}";
            return report;
        }
    }
}
