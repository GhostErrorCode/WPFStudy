using MemoDesktop.ApiResponses;
using MemoDesktop.Dtos.User;
using MemoDesktop.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace MemoDesktop.ViewModels.User
{
    public class LoginViewModel : BindableBase, IDialogAware
    {
        // 注意：Prism8.0+已经移除了Title属性，现在Title属性需要自己写
        public string Title { get => "Memo_Login"; }

        public DialogCloseListener RequestClose { get; set; }

        //================字段与属性===================
        // 用户API服务
        private readonly IUserApiService _userApiService;

        // 用户账户
        private string _account;
        public string Account
        {
            get {  return _account; }
            set { _account = value; RaisePropertyChanged(); }
        }
        // 用户密码(这里不加通知更改的话，可能不会更新登录页面)
        private string _password;
        public string Password
        {
            get { return _password; }
            set { _password = value; RaisePropertyChanged(); }
        }

        // 登录页面/注册页面选择
        private int _selectedIndex = 0;
        public int SelectedIndex
        {
            get => _selectedIndex;
            set { _selectedIndex = value; RaisePropertyChanged(); }
        }

        // 注册账户实体
        private RegisterUserDto _registerUserDto = new RegisterUserDto();
        public RegisterUserDto RegisterUserDto
        {
            get { return _registerUserDto; }
            set { _registerUserDto = value; RaisePropertyChanged(); }
        }
        
        //================命令===================
        // 登录按钮Command
        public DelegateCommand LoginCommand { get; private set; }
        // 登出按钮Command
        public DelegateCommand LoginOutCommand { get; private set; }
        // 注册按钮Command
        public DelegateCommand RegisterCommand { get; private set; }

        // 返回登录页面Command
        public DelegateCommand ReturnLoginCommand { get; private set; }
        // 进入注册页面Command
        public DelegateCommand GoRegisterCommand { get; private set; }

        //================构造函数===================
        public LoginViewModel(IUserApiService userApiService)
        {
            // 初始化Command
            this.LoginCommand = new DelegateCommand(async () => await Login());
            this.LoginOutCommand = new DelegateCommand(LoginOut);
            this.RegisterCommand = new DelegateCommand(async () => await Register());
            this.ReturnLoginCommand = new DelegateCommand(ReturnLogin);
            this.GoRegisterCommand = new DelegateCommand(GoRegister);

            // 初始化服务
            this._userApiService = userApiService;
        }

        // 登录方法
        private async Task Login()
        {
            // 先判断用户名和密码是否为空
            if(string.IsNullOrWhiteSpace(this._account) || string.IsNullOrWhiteSpace(this._password)) { return; }
            // 用户输入后，判断用户名与密码是否正确
            ApiResponse<UserDto> loginApiResponse = await this._userApiService.LoginUserAsync(new LoginUserDto()
            {
                Account = this._account,
                Password = this._password
            });

            // 如果后端返回的是登录成功
            if (loginApiResponse.IsSuccess)
            {
                // 登录成功，关闭此窗口并返回ButtonResult.OK
                this.RequestClose.Invoke(new DialogResult(ButtonResult.OK));
                // 这里不能使用RequestClose?.Invoke，因为DialogCloseListener是值类型的结构体，只有引用类型才能用?.
            }
            else
            {
                // 登录失败...
            }
        }

        // 注册方法
        private async Task Register()
        {
            // 先判断用户名、昵称和密码是否为空
            if(string.IsNullOrWhiteSpace(this._registerUserDto.Account)
                || string.IsNullOrWhiteSpace(this._registerUserDto.UserName)
                || string.IsNullOrWhiteSpace(this._registerUserDto.Password)
                || string.IsNullOrWhiteSpace(this._registerUserDto.ConfirmPassword))
                { return; }
            // 再判断两次密码是否一致
            if(this._registerUserDto.Password != this._registerUserDto.ConfirmPassword) { return; }

            // 判断无问题后，开始调用后台注册服务
            ApiResponse<UserDto> registerApiResponse = await this._userApiService.RegisterUserAsync(this._registerUserDto);
            // 如果后端返回的是登录成功
            if (registerApiResponse.IsSuccess)
            {
                // 可选：自动将注册成功的账户填入登录界面
                this.Account = registerApiResponse.Data.Account;
                // 注册成功，返回登录页面
                this.SelectedIndex = 0;
            }
            else
            {
                // 登录失败...
            }
        }

        // 登出方法
        private void LoginOut()
        {
            // 关闭界面
            this.RequestClose.Invoke(new DialogResult(ButtonResult.No));
        }

        // 进入注册页面
        private void GoRegister() => this.SelectedIndex = 1;
        // 返回登录页面
        private void ReturnLogin() => this.SelectedIndex = 0;

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
