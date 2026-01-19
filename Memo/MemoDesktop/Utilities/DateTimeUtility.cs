using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Threading;

namespace MemoDesktop.Utilities
{
    /// <summary>
    /// 日期时间工具类 - 提供实时更新的日期和时间功能
    /// 说明：这是一个全局可访问的静态工具类，用于获取当前日期和时间
    /// </summary>
    public static class DateTimeUtility
    {
        // 私有字段：存储当前时间的字符串表示
        private static string _currentTime;

        /// <summary>
        /// 当前日期和时间属性
        /// 格式：2024-01-15 14:30:45（年-月-日 时:分:秒）
        /// 特性：这是一个只读属性，外部只能获取，不能设置
        /// </summary>
        public static string CurrentDateTime
        {
            get
            {
                // 懒加载模式：第一次访问属性时才初始化计时器
                // 检查_currentTime是否为null，如果是则说明计时器还未初始化
                if (_currentTime == null)
                {
                    // 调用初始化方法，创建并启动计时器
                    InitializeTimer();
                }
                return _currentTime;
            }
            private set
            {
                // 设置时间值
                _currentTime = value;

                // 重要：每次时间更新时触发TimeChanged事件
                // 这个事件的含义：通知所有订阅者（监听者）时间已经发生变化
                // 在MVVM架构中，ViewModel可以订阅这个事件来更新自己的绑定属性
                // 这样，多个ViewModel可以同时监听时间变化，实现数据同步更新
                TimeChanged?.Invoke();
            }
        }

        /// <summary>
        /// 时间变化事件
        /// 事件含义：当CurrentDateTime属性的值发生变化时触发此事件
        /// 订阅者：任何需要知道时间变化的对象都可以订阅此事件
        /// 典型用途：
        /// 1. ViewModel订阅此事件，当时间变化时更新自己的绑定属性
        /// 2. 多个页面/控件可以同时订阅，实现时间同步显示
        /// 3. 需要定时执行某些逻辑的组件可以订阅此事件
        /// </summary>
        public static event Action TimeChanged;

        /// <summary>
        /// 初始化计时器方法（私有方法，仅在内部调用）
        /// 作用：创建并配置一个每秒触发一次的计时器
        /// </summary>
        private static void InitializeTimer()
        {
            // 设置初始时间值
            // 使用DateTime.Now获取系统当前时间，并格式化为指定格式
            CurrentDateTime = DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss");

            // 创建DispatcherTimer实例
            // DispatcherTimer是WPF特有的计时器，它在UI线程上触发事件
            // 这意味着我们可以在Tick事件处理中直接更新UI元素，无需跨线程调用
            DispatcherTimer timer = new DispatcherTimer();

            // 设置计时器间隔为1秒（1000毫秒）
            timer.Interval = TimeSpan.FromSeconds(1);

            // 注册Tick事件处理程序
            // 使用lambda表达式简化事件处理器的定义
            // 每次计时器触发时（每秒一次）：
            // 1. 获取当前系统时间
            // 2. 格式化时间字符串
            // 3. 更新CurrentDateTime属性值
            // 4. 自动触发TimeChanged事件通知所有订阅者
            timer.Tick += (sender, e) =>
            {
                CurrentDateTime = DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss");
            };

            // 启动计时器，开始每秒更新时间
            timer.Start();
        }
    }
}
