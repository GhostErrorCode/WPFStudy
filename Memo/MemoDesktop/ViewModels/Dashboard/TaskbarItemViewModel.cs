using MemoDesktop.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MemoDesktop.ViewModels.Dashboard
{
    // 任务栏每项的ViewModel，主要用来实现通知更改，符合MVVM模式
    public class TaskbarItemViewModel : BindableBase
    {
        // 任务栏Model
        private readonly TaskBarModel _taskBarModel;

        // 任务栏图标
        public string Icon
        {
            get { return this._taskBarModel.Icon; }
            set {  this._taskBarModel.Icon = value; RaisePropertyChanged(); }
        }

        // 任务栏标题
        public string Title
        {
            get { return this._taskBarModel.Title; }
            set { this._taskBarModel.Title = value; RaisePropertyChanged(); }
        }

        // 任务栏内容
        public string Content
        {
            get { return this._taskBarModel.Content; }
            set { this._taskBarModel.Content = value; RaisePropertyChanged(); }
        }

        // 任务栏颜色
        public string Color
        {
            get { return this._taskBarModel.Color; }
            set { this._taskBarModel.Color = value; RaisePropertyChanged(); }
        }

        // 任务栏目标(跳转)
        public string Target
        {
            get { return this._taskBarModel.Target; }
            set { this._taskBarModel.Target = value; RaisePropertyChanged(); }
        }

        // 构造函数
        public TaskbarItemViewModel(TaskBarModel taskBarModel)
        {
            this._taskBarModel = taskBarModel;
        }
    }
}
