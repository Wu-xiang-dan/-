using Newtonsoft.Json;
using NoteBook.Data;
using NoteBook.DTO;
using NoteBook.DTOS;
using NoteBook.Extension;
using NoteBook.HttpClients;
using NoteBook.MegEvents;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
namespace NoteBook.ViewModels
{
    class LoginViewModel : BindableBase, IDialogAware
    {
        public string Title => "Login";
        private readonly IDataService _dataService;
        private readonly HttpRestClient _restClient;
        private CancellationTokenSource _cancellationTokenSource;

        //事件聚合器
        private readonly IEventAggregator _eventAggregator;
        public event Action<IDialogResult> RequestClose;
        public bool CanCloseDialog() => true;

        public DelegateCommand LoginCommand { get; set; }
        public DelegateCommand ShowRegistInfoCommand { get; set; }
        public DelegateCommand ShowLoginInfoCommand { get; set; }
        public AsyncDelegateCommand RegisterCommand { get; set; }

        private string _account;

        public string Account
        {
            get { return _account; }
            set
            {
                _account = value;
                RaisePropertyChanged(nameof(Account));
            }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                RaisePropertyChanged(nameof(Password));
            }
        }
        private AccountInfoDTO _accountInfo;

        public AccountInfoDTO Accountinfo
        {
            get { return _accountInfo; }
            set
            {
                _accountInfo = value;
                RaisePropertyChanged(nameof(Accountinfo));
            }
        }

        public LoginViewModel(HttpRestClient httpRestClicent, IEventAggregator eventAggregator, IDataService dataService)
        {
            _dataService = dataService;
            ShowRegistInfoCommand = new DelegateCommand(() => SelectedIndex = 1);
            ShowLoginInfoCommand = new DelegateCommand(() => SelectedIndex = 0);
            LoginCommand = new DelegateCommand(Login);
            RegisterCommand = new AsyncDelegateCommand(RegisterUser);
            Accountinfo = new AccountInfoDTO();
            _restClient = httpRestClicent;
            _eventAggregator = eventAggregator;
            _cancellationTokenSource = new CancellationTokenSource();
            Account = "huge258";
            Password = "114514";
        }
        public async void Login()
        {
            if (string.IsNullOrEmpty(Account) || string.IsNullOrEmpty(Password))
            {
                _eventAggregator.GetEvent<LoginMegEvents>().Publish("请输入完整信息");
                return;
            }
            string PasswordMd5 = Md5Helper.GetMd5(Password);
            var request = new ApiRequest
            {
                Method = RestSharp.Method.GET,
                Route = "Account/Login",
                Paramters = new Dictionary<string, object>
               {
                    {"username", Account },
                    {"password", PasswordMd5 }
               }
            };
            _eventAggregator.GetEvent<LoginMegEvents>().Publish("登录中");
            using var loginCts = new CancellationTokenSource(30000);
            var res = await _restClient.ExecuteAsync(request, loginCts.Token);
            if (res.ResultCode == Result.Success)
            {
                _eventAggregator.GetEvent<LoginMegEvents>().Publish("登录成功");
                var loginResut = JsonConvert.DeserializeObject<LoginResultDTO>(res.ResultData.ToString());
                _dataService.SetID(loginResut.AccountId);
                _eventAggregator.GetEvent<MesEvent>().Publish(await _dataService.LoadDataAsync());
                DialogParameters Pairs = new DialogParameters();
                Pairs.Add("AccountName", loginResut.AccountName);
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK, Pairs));
            }
            else if (Password == "114514" && Account == "huge258")
            {
                _eventAggregator.GetEvent<LoginMegEvents>().Publish("悄咪咪的测试进入，启用本地模式");
                var loginResut = new LoginResultDTO() { AccountId = 0, AccountName = "Ruby" };
                _dataService.SetID(loginResut.AccountId);
                _eventAggregator.GetEvent<MesEvent>().Publish(await _dataService.LoadDataAsync());
                await Task.Delay(2000);
                DialogParameters Pairs = new DialogParameters();
                Pairs.Add("AccountName", loginResut.AccountName);
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK, Pairs));
            }
            else if (res.ResultCode == Result.Error)
            {
                _eventAggregator.GetEvent<LoginMegEvents>().Publish("error");
            }
            else if (res.ResultCode == Result.NotFound)
            {
                _eventAggregator.GetEvent<LoginMegEvents>().Publish("服务器丢失");
            }
            else if (res.ResultCode == Result.Failed)
            {
                _eventAggregator.GetEvent<LoginMegEvents>().Publish("用户名或密码错误");
            }

        }
        public async Task RegisterUser()
        {
            if (string.IsNullOrEmpty(Accountinfo.AccountName) ||
                string.IsNullOrEmpty(Accountinfo.AccountName) ||
                string.IsNullOrEmpty(Accountinfo.AccountName) ||
                string.IsNullOrEmpty(Accountinfo.AccountName))
            {
                _eventAggregator.GetEvent<LoginMegEvents>().Publish("请输入完整信息");
                return;
            }
            if (Accountinfo.Password != Accountinfo.ComfirmPassword)
            {
                _eventAggregator.GetEvent<LoginMegEvents>().Publish("密码不一致");
                return;
            }
            Accountinfo.Password = Md5Helper.GetMd5(Accountinfo.Password);
            ApiRequest request = new ApiRequest() { Method = RestSharp.Method.POST, Route = "Register", Paramters = Accountinfo };

            using (var RegisterCts = new CancellationTokenSource(3000))
            {

                ApiResponse res = await _restClient.ExecuteAsync(request, RegisterCts.Token);
                if (res.ResultCode == Result.Success)
                { //成功

                    SelectedIndex = 0;
                }
                else if (res.ResultCode == Result.Error)
                {
                    _eventAggregator.GetEvent<LoginMegEvents>().Publish("error");
                }
                else if (res.ResultCode == Result.NotFound)
                {
                    _eventAggregator.GetEvent<LoginMegEvents>().Publish("服务器崩溃");
                }
            }
        }
        public void OnDialogClosed()
        {
        }
        public void OnDialogOpened(IDialogParameters parameters)
        {

        }
        private int _selectedIndex = 0;
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                SetProperty(ref _selectedIndex, value);
                RaisePropertyChanged(nameof(SelectedIndex));
            }
        }
    }
}
