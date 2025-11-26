using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using WPFStudy.Command.Commands.Custom;
using WPFStudy.Command.ViewModels.Base;

namespace WPFStudy.Command.ViewModels.Pages
{
    // 继承一下通知更改的基类
    public class CmdMainViewModel : CmdMainViewModelBase
    {
        // 定义Command，负责UI和业务的一个交互
        public CmdMainCommand? ShowCommand { get; set; }
        // 定义一个Name，用来学习通知更改，绑定了TextBox
        private string? name;  // 这个是字段,不对外暴露
        public string? Name  // 这个是属性，对外暴露
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }


        public CmdMainViewModel()
        {
            name = "我是文本框";
            // 给ShowCommand传入一个方法,可以让控件去调用这个ShowCommand命令从而调用了Show方法
            this.ShowCommand = new CmdMainCommand(Show);
        }


        // 为了实现功能
        // 可以被任何功能调用
        public void Show()
        {
            // 通知更新时必须使用属性
            // 这是一个非常经典的WPF数据绑定问题！根本原因在于WPF的数据绑定机制只监听属性变更，不监听字段变更
            Name = "按钮被点击了!";
            MessageBox.Show(Name);
        }
    }
}
