using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace WPFStudy.Command.ViewModels.Base
{
    public class CmdMainViewModelBase : INotifyPropertyChanged
    {
        // 这个类是为了实现 然后通知UI元素更改他已经绑定的值
        public event PropertyChangedEventHandler? PropertyChanged;

        // 写一个类，需要自己去调用
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            // [CallerMemberName] string propertyName = "" 这是一个C#编译器特性，主要用于自动获取调用方法的成员名称，在WPF的MVVM模式中特别有用
            // 如果不为空，则调用
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
