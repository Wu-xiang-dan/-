using Newtonsoft.Json;
using NoteBook.Data;
using NoteBook.DTO;
using NoteBook.DTOS;
using NoteBook.HttpClients;
using NoteBook.MegEvents;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;

namespace NoteBook.ViewModels
{
    class LoginViewModel : BindableBase, IDialogAware
    {
        public string Title => "Login";
        private readonly IDataService _dataService;
        private readonly HttpRestClient _restClicent;
        //事件聚合器
        private readonly IEventAggregator _eventAggregator;
        public event Action<IDialogResult> RequestClose;
        public bool CanCloseDialog() => true;
        
        public DelegateCommand LoginCommand { get; set; }
        public DelegateCommand ShowRegistInfoCommand { get; set; }
        public DelegateCommand ShowLoginInfoCommand { get; set; }
        public DelegateCommand RegisterCommand { get; set; }

        private string _account;

        public string Account
        {
            get { return _account; }
            set { _account = value;
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
            set { _accountInfo = value;
                RaisePropertyChanged(nameof(Accountinfo));
            }
        }

        public LoginViewModel(HttpRestClient httpRestClicent,IEventAggregator eventAggregator,IDataService dataService)
        {
            _dataService = dataService;
            ShowRegistInfoCommand = new DelegateCommand(() => SelectedIndex = 1);
            ShowLoginInfoCommand = new DelegateCommand(() => SelectedIndex = 0);
            LoginCommand = new DelegateCommand(Login);
            RegisterCommand = new DelegateCommand(RegisterUser);
            Accountinfo = new AccountInfoDTO();
            _restClicent = httpRestClicent;
            _eventAggregator = eventAggregator;
            Account = "huge258";          
        }
        public async void Login()
        {
            if (string.IsNullOrEmpty(Account) || string.IsNullOrEmpty(Password))
            {
                _eventAggregator.GetEvent<MesEvent>().Publish("请输入完整信息");
                return;
            }
            Password=Md5Helper.GetMd5(Password);
            ApiRequest request = new ApiRequest() { Method = RestSharp.Method.GET, Route = $"Login?username={Account}&password={Password}"};
            _eventAggregator.GetEvent<MesEvent>().Publish("登录中");
            var res= await Task.Run(()=> _restClicent.Execute(request));
            if (res.ResultCode == Result.Success)
            { //成功
                var loginResut=JsonConvert.DeserializeObject<LoginResultDTO>(res.ResultData.ToString());
                _dataService.SetID(loginResut.AccountId);
                _dataService.LoadData();
                _eventAggregator.GetEvent<MesEvent>().Publish("登录成功");
                DialogParameters Pairs = new DialogParameters();
                Pairs.Add("AccountName", loginResut.AccountName);
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK,Pairs));
            }
            else if (res.ResultCode == Result.Error)
            {
                _eventAggregator.GetEvent<MesEvent>().Publish("error");
            }
            else if (res.ResultCode == Result.NotFound)
            {
                _eventAggregator.GetEvent<MesEvent>().Publish("服务器崩溃丢失，启用本地模式");
                var loginResut = new LoginResultDTO() { AccountId = 0, AccountName = "Ruby" };
                _dataService.SetID(loginResut.AccountId);
                _dataService.LoadData();
                DialogParameters Pairs = new DialogParameters();
                Pairs.Add("AccountName", loginResut.AccountName);
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK, Pairs));
            }
            else if (res.ResultCode == Result.Failed)
            {
                _eventAggregator.GetEvent<MesEvent>().Publish("用户名或密码错误");
            }
        }
        public void RegisterUser()
        {
            if(string.IsNullOrEmpty(Accountinfo.AccountName)||
                string.IsNullOrEmpty(Accountinfo.AccountName)||
                string.IsNullOrEmpty(Accountinfo.AccountName)||
                string.IsNullOrEmpty(Accountinfo.AccountName))
            {
                _eventAggregator.GetEvent<MesEvent>().Publish("请输入完整信息");
                return;
            }
            if(Accountinfo.Password!=Accountinfo.ComfirmPassword)
            {
                _eventAggregator.GetEvent<MesEvent>().Publish("密码不一致");
                return;
            }
            Accountinfo.Password = Md5Helper.GetMd5(Accountinfo.Password);
            ApiRequest request= new ApiRequest() {Method=RestSharp.Method.POST,Route= "Register",Paramters=Accountinfo };
            ApiResponse res = _restClicent.Execute(request);
            if (res.ResultCode == Result.Success) { //成功
                  
                SelectedIndex = 0;
            }
            else if (res.ResultCode ==Result.Error)
            {
                _eventAggregator.GetEvent<MesEvent>().Publish("error");
            }
            else if (res.ResultCode == Result.NotFound)
            {
                _eventAggregator.GetEvent<MesEvent>().Publish("服务器崩溃");
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
