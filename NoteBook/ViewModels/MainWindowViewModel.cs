using Newtonsoft.Json;
using NoteBook.DTOS;
using NoteBook.Models;
using Prism.Commands;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Xps.Serialization;

namespace NoteBook.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Prism Application";
        private readonly IRegionManager _regionManager;//区域管理器
        private readonly IModuleCatalog _moduleCatalog;//模块目录
        private IRegionNavigationJournal _navigationJournal;////导航历史记录 
        private List<LeftMenuInfo> _lefMenuList;       //左侧控件模块
        public DelegateCommand<LeftMenuInfo> NavigateCommand { get; set; }
        public DelegateCommand GoBackCommand { get; set; }
        public DelegateCommand GoForwardCommand { get; set; }
        public List<LeftMenuInfo> LeftMenuList
        {
            get { return _lefMenuList;}
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
        public MainWindowViewModel(IRegionManager regionManager, IModuleCatalog moduleCatalog)
        {
            LeftMenuList = new List<LeftMenuInfo>();
            NavigateCommand = new DelegateCommand<LeftMenuInfo>(Navigate);
            GoBackCommand = new DelegateCommand(GoBack);
            GoForwardCommand = new DelegateCommand(GoForward);
            _regionManager = regionManager;
            _moduleCatalog = moduleCatalog;
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
                _navigationJournal.GoBack();

        }
        private void GoForward()
        {
            if (_navigationJournal != null && _navigationJournal.CanGoForward)
                _navigationJournal.GoForward();
        }
        private void Navigate(LeftMenuInfo menu)
        {
            if (menu == null || string.IsNullOrEmpty(menu.ViewName)) return;

            _regionManager.RequestNavigate("ContentRegion", $"{menu.ViewName}View", callback => { 
               _navigationJournal=callback.Context.NavigationService.Journal;

            });
        }
        public void DefNavigate(string AccountName)
        {
            NavigationParameters parameters = new NavigationParameters();
            parameters.Add("AccountName", AccountName);
            _regionManager.RequestNavigate("ContentRegion", "HomeUcView", callback => {
                _navigationJournal = callback.Context.NavigationService.Journal;
            },parameters);
        }

    }
}
