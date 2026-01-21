using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Navigation;

namespace MemoDesktop.ViewModels.User
{
    public class LoginViewModel : BindableBase, IDialogAware
    {
        // 注意：Prism8.0+已经移除了Title属性，现在Title属性需要在View自己写
        public string Title { get => "Memo_Login"; }

        public DialogCloseListener RequestClose { get; set; }

        //================字段与属性===================
        // 用户账户
        private string _account;
        public string Account
        {
            get {  return _account; }
            set { _account = value; }
        }
        // 用户密码
        private string _password;
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        //================命令===================
        // 登录按钮Command
        public DelegateCommand LoginCommand { get; private set; }
        // 登出按钮Command
        public DelegateCommand LoginOutCommand { get; private set; }

        //================构造函数===================
        public LoginViewModel()
        {
            // 初始化Command
            this.LoginCommand = new DelegateCommand(Login);
            this.LoginOutCommand = new DelegateCommand(LoginOut);
        }

        // 登录方法
        private void Login()
        {

        }
        // 登出方法
        private void LoginOut()
        {

        }








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
