using Microsoft.VisualBasic;
using Newtonsoft.Json;
using NoteBook.Data;
using NoteBook.DTOS;
using NoteBook.Extension;
using NoteBook.HttpClients;
using NoteBook.MegEvents;
using NoteBook.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace NoteBook.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Material Design ";
        private readonly IRegionManager _regionManager;//区域管理器
        private readonly IEventAggregator _eventAggregator;
        private readonly IModuleCatalog _moduleCatalog;//模块目录
        private readonly IDataService _dataService;//数据服务接口
        private IRegionNavigationJournal _navigationJournal;////导航历史记录 
        private List<LeftMenuInfo> _lefMenuList;       //左侧控件模块
        public DelegateCommand<LeftMenuInfo> NavigateCommand { get; set; }
        public DelegateCommand GoBackCommand { get; set; }
        public DelegateCommand GoForwardCommand { get; set; }
        public AsyncDelegateCommand SaveCommandAsync { get; set; }
        public List<LeftMenuInfo> LeftMenuList
        {
            get { return _lefMenuList; }
            set
            {
                _lefMenuList = value;
                RaisePropertyChanged(nameof(LeftMenuList));
            }
        }
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        public MainWindowViewModel(IRegionManager regionManager, IModuleCatalog moduleCatalog, IDataService dataService, IEventAggregator eventAggregator)
        {
            LeftMenuList = new List<LeftMenuInfo>();
            NavigateCommand = new DelegateCommand<LeftMenuInfo>(Navigate);
            GoBackCommand = new DelegateCommand(GoBack);
            GoForwardCommand = new DelegateCommand(GoForward);
            _regionManager = regionManager;
            _moduleCatalog = moduleCatalog;
            _dataService = dataService;
            _eventAggregator = eventAggregator;
            SaveCommandAsync = new AsyncDelegateCommand(SaveAsync);
            InitModules();
        }
        private void InitModules()
        {
            var dirModuleCatalog = _moduleCatalog as DirectoryModuleCatalog;
            _moduleCatalog.Initialize();
            foreach (var mod in dirModuleCatalog.Items)
            {
                var tempModule = mod as ModuleInfo;
                switch (tempModule.ModuleName)
                {
                    case "Home":
                        LeftMenuList.Add(new LeftMenuInfo() { Icon = "Home", MenuName = "首页", ViewName = "HomeUc" });
                        break;
                    case "Wait":
                        LeftMenuList.Add(new LeftMenuInfo() { Icon = "NotebookOutline", MenuName = "待办事项", ViewName = "WaitUc" });
                        break;
                    case "Memo":
                        LeftMenuList.Add(new LeftMenuInfo() { Icon = "NotebookPlus", MenuName = "备忘录", ViewName = "MemoUc" });
                        break;
                    case "Settings":
                        LeftMenuList.Add(new LeftMenuInfo() { Icon = "Cog", MenuName = "设置", ViewName = "SettingsUc" });
                        break;
                    default:
                        break;
                }
            }

        }
        private void GoBack()
        {
            if (_navigationJournal != null && _navigationJournal.CanGoBack)
            {
                _navigationJournal.GoBack();
                Title = "Material Material " + _navigationJournal.CurrentEntry?.Uri.ToString() ?? "Material Material";
            }

        }
        private void GoForward()
        {
            if (_navigationJournal != null && _navigationJournal.CanGoForward)
            {
                _navigationJournal.GoForward();
                Title = "Material Material " + _navigationJournal.CurrentEntry?.Uri.ToString() ?? "Material Material";
            }
        }
        private void Navigate(LeftMenuInfo menu)
        {
            if (menu == null || string.IsNullOrEmpty(menu.ViewName)) return;

            _regionManager.RequestNavigate("ContentRegion", $"{menu.ViewName}View", callback =>
            {
                _navigationJournal = callback.Context.NavigationService.Journal;
                Title = "Material Material " + _navigationJournal.CurrentEntry?.Uri.ToString() ?? "Material Material";


            });
        }
        public void DefNavigate(string AccountName)
        {
            NavigationParameters parameters = new NavigationParameters();
            parameters.Add("AccountName", AccountName);
            _regionManager.RequestNavigate("ContentRegion", "HomeUcView", callback =>
            {
                _navigationJournal = callback.Context.NavigationService.Journal;
                Title = "Material Material " + _navigationJournal.CurrentEntry?.Uri.ToString() ?? "Material Material";
            }, parameters);
        }
        private async Task SaveAsync()
        {

            try
            {
                _eventAggregator.GetEvent<MesEvent>().Publish("正在保存数据请稍等");
                var res = await Task.WhenAll(_dataService.UploadMemosAsync(),_dataService.UploadWaitsAsync());
                var failedResponses = res.SelectMany(t => t)
                           .Where(item => item.ResultCode != Result.Success)
                           .ToList();
                if (failedResponses.Count == 0)
                {
                    _eventAggregator.GetEvent<MesEvent>().Publish("保存成功");
                    return;
                }
                foreach (var item in failedResponses)
                {
                    _eventAggregator.GetEvent<MesEvent>().Publish(item.Message);
                }
            }
            catch (Exception ex)
            {
                _eventAggregator.GetEvent<MesEvent>().Publish($"异常:{ex.Message}");
            }
        }
    }
}
