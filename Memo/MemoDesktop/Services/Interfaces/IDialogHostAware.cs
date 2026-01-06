using System;
using System.Collections.Generic;
using System.Text;

namespace MemoDesktop.Services.Interfaces
{
    public interface IDialogHostAware
    {
        public string DialogHostName { get; set; }

        public void OnDialogOpened(IDialogParameters parameters);

        public DelegateCommand SaveCommand { get; set; }

        public DelegateCommand CancelCommand { get; set; }
    }
}
