using MaterialDesignThemes.Wpf;
using MemoDesktop.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MemoDesktop.ViewModels.Dialogs
{
    public class MsgDialogViewModel : BindableBase, IDialogHostAware
    {
        public string DialogHostName { get; set; }

        // 信息提示框标题和内容
        private string _title;
        public string Title
        {
            get { return _title; }
            set { _title = value; RaisePropertyChanged(); }
        }
        private string _content;
        public string Content
        {
            get { return _content; }
            set { _content = value; RaisePropertyChanged(); }
        }


        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }

        public MsgDialogViewModel()
        {
            this.SaveCommand = new DelegateCommand(Save);
            this.CancelCommand = new DelegateCommand(Cancel);
        }

        private void Save()
        {
            if (DialogHost.IsDialogOpen(DialogHostName))
            {
                DialogParameters param = new DialogParameters();

                DialogResult dialogResult = new DialogResult()
                {
                    Result = ButtonResult.OK,
                    Parameters = param
                };
                // DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.OK, param));
                DialogHost.Close(DialogHostName, dialogResult);
            }
        }

        private void Cancel()
        {
            if (DialogHost.IsDialogOpen(DialogHostName))
            {
                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.Cancel));
            }
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            // 接收传入的参数，消息提示框的标题和内容
            if (parameters.ContainsKey("Title")) { this.Title = parameters.GetValue<string>("Title"); }
            if (parameters.ContainsKey("Content")) { this.Content = parameters.GetValue<string>("Content"); }
        }
    }
}
