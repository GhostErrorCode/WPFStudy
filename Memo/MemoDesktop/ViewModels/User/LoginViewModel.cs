using System;
using System.Collections.Generic;
using System.Text;

namespace MemoDesktop.ViewModels.User
{
    public class LoginViewModel : BindableBase, IDialogAware
    {
        // 注意：Prism8.0+已经移除了Title属性，现在Title属性需要在View自己写
        public string Title
        {
            get { return "Memo_Login"; }
        }

        public DialogCloseListener RequestClose { get; set; }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            
        }
    }
}
