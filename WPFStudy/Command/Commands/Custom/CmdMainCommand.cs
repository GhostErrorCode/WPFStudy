using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace WPFStudy.Command.Commands.Custom
{
    public class CmdMainCommand : ICommand
    {
        /// <summary>
        /// 当命令的可执行状态发生变化时触发的事件
        /// </summary>
        public event EventHandler? CanExecuteChanged;

        // 这里定义一个委托，用于运行外部方法
        private Action executeAction;
        public CmdMainCommand(Action action)
        {
            // 获取到外部方法并存入委托中,方便Execute方法调用
            this.executeAction = action;
        }

        /// <summary>
        /// 检查命令是否可以执行的方法
        /// </summary>
        /// <param name="parameter">命令参数</param>
        /// <returns>如果命令可以执行返回 true，否则返回 false</returns>
        public bool CanExecute(object? parameter)
        {
            return true;  // 目前暂时返回true
        }

        /// <summary>
        /// 执行命令的方法
        /// </summary>
        /// <param name="parameter">命令参数</param>
        public void Execute(object? parameter)
        {
            // 这里要执行一个方法
            // 方法来源于外部，所以要执行一个委托
            this.executeAction.Invoke();
        }
    }
}
