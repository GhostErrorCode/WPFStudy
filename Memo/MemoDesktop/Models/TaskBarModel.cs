using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Text;

namespace MemoDesktop.Models
{
    // 菜单栏的Model，包含图标，名称，命名空间
    public class TaskBarModel
    {
        /// <summary>
        /// 任务栏图标
        /// PackIconKind是UI组件的
        /// </summary>
        private PackIconKind _icon;
        public PackIconKind Icon
        {
            get { return _icon; } 
            set { _icon = value; }
        }

        /// <summary>
        /// 任务栏名称
        /// </summary>
        private string _title;
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        /// <summary>
        /// 任务说明
        /// </summary>
        private string _content;
        public string Content
        {
            get { return _content; }
            set { _content = value; }
        }

        /// <summary>
        /// 任务栏颜色
        /// </summary>
        private string _color;
        public string Color
        {
            get { return _color; }
            set { _color = value; }
        }


        /// <summary>
        /// 目标
        /// </summary>
        private string _target;
        public string Target
        {
            get { return _target; }
            set { _target = value; }
        }
    }
}
