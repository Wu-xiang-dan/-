using NoteBook.DTOS;
using Home.Models;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NoteBook.Data;
using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Services.Dialogs;
using DryIoc;
using System.Linq;
using NoteBook.ViewModels;
using NoteBook.Models;
using Prism.Events;
using NoteBook.MegEvents;
namespace Home.ViewModels
{
    class HomeUcViewModel : BindableBase, INavigationAware
    {
        public HomeUcViewModel(IDialogService dialogService, IRegionManager regionManager, IDataService dataServicers, IContainer container, IEventAggregator eventAggregator)
        {
            _dialogService = dialogService;
            _dataService = dataServicers;
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            ChangeWaitingStateCommand = new DelegateCommand<WaitVieModel>(ChangeWaitingStateExecute);
            DeleteMemoCommand = new DelegateCommand<MemoViewModel>(DeleteMemoExecute);
            ShowAddWaitDialogCommand = new DelegateCommand(ShowAddWaitDialog);
            ShowAddMemoDialogCommand = new DelegateCommand(ShowAddMemoDialog);
            ShowEditWaitDialogCommand = new DelegateCommand<WaitVieModel>(ShowEditWaitDialogExecute,(w)=>w!=null);
            ShowEditMemoDialogCommand = new DelegateCommand<MemoViewModel>(ShowEditMemoDialogExecute,(w)=>w!=null);
            NavigateCommand = new DelegateCommand<StackPlaneInfo>(NavigateExecute);
            InitStackPlaneList();
        }
        public DelegateCommand<MemoViewModel> DeleteMemoCommand { get; set; }
        public DelegateCommand<WaitVieModel> ChangeWaitingStateCommand { get; set; }
        public DelegateCommand ShowAddWaitDialogCommand { get; set; }
        public DelegateCommand ShowAddMemoDialogCommand { get; set; }
        public DelegateCommand<WaitVieModel> ShowEditWaitDialogCommand { get; set; }
        public DelegateCommand<MemoViewModel> ShowEditMemoDialogCommand { get; set; }
        public DelegateCommand<StackPlaneInfo> NavigateCommand { get; set; }
        private readonly IDataService _dataService;//数据服务
        private IRegionNavigationJournal _navigationJournal;
        private readonly IRegionManager _regionManager;
        private readonly IDialogService _dialogService;
        private readonly IEventAggregator _eventAggregator;//通知服务
        private StateWaitDTO StateWaitDTO = new StateWaitDTO();
        private List<StackPlaneInfo> _stackPlaneList;
        private ObservableCollection<WaitVieModel> _notResolveWaitInfoList;
        private ObservableCollection<MemoViewModel> _memoInfoList = new ObservableCollection<MemoViewModel>();
        private string _accountName;
        private string _memoCount;
        public string AccountName
        {
            get { return _accountName; }
            set
            {
                SetProperty(ref _accountName, value);
                RaisePropertyChanged(nameof(AccountName));
            }
        }
        public string MemoCount
        {
            get { return _memoCount; }
            set
            {
                _memoCount = value;
                RaisePropertyChanged(MemoCount);
            }
        }
        public ObservableCollection<MemoViewModel> MemoInfoList
        {
            get { return _memoInfoList; }
            set
            {
                SetProperty(ref _memoInfoList, value);
                RaisePropertyChanged(nameof(MemoInfoList));
            }
        }  //备忘录列表数据源
        public ObservableCollection<WaitVieModel> NotResolveWaitInfoList
        {
            get { return _notResolveWaitInfoList; }
            set
            {
                SetProperty(ref _notResolveWaitInfoList, value);
                RaisePropertyChanged(nameof(NotResolveWaitInfoList));
            }
        }  //待办事项列表数据源  
        public List<StackPlaneInfo> StackPlaneList
        {
            get { return _stackPlaneList; }
            set
            {
                SetProperty(ref _stackPlaneList, value);

            }
        } 
        /// <summary>
        ///   初始化待办事项数据
        /// </summary>
        public void GetNotResolveWaitInfoList()
        {
            try
            {
                NotResolveWaitInfoList = new ObservableCollection<WaitVieModel>(_dataService.ViewWaitList.Where(t => t.Status == 0&&t.dataStatus!=DataStatus.Delete));
            }
            catch (Exception e)
            {
                _eventAggregator.GetEvent<MesEvent>().Publish(e.Message);
            }
        }
        public void GetMemoinfoList()
        {
            try
            {
                MemoInfoList = new ObservableCollection<MemoViewModel>(_dataService.ViewMemoList);
            }
            catch (Exception e)
            {
                _eventAggregator.GetEvent<MesEvent>().Publish(e.Message);
            }
        }
        public void ShowEditWaitDialogExecute(WaitVieModel WaitDTO)
        {
            DialogParameters pairs = new DialogParameters();
            pairs.Add("oldValue", WaitDTO);
            _dialogService.Show("EditWaitUcView", pairs, callback =>
            {
                if (callback.Result == ButtonResult.OK)
                {
                    WaitVieModel OldValue = callback.Parameters.GetValue<WaitVieModel>("oldValue");
                    WaitVieModel newValue = callback.Parameters.GetValue<WaitVieModel>("newValue");

                    if (OldValue.Status != newValue.Status || OldValue.Content != newValue.Content || OldValue.Title != newValue.Title)
                    {
                        WaitDTO.Title = newValue.Title;
                        WaitDTO.Content = newValue.Content;
                        WaitDTO.Status = newValue.Status;
                        WaitDTO.dataStatus = DataStatus.Alter;
                        if (newValue.Status == 1)
                        {
                            OldValue.dataStatus = DataStatus.Delete;
                            NotResolveWaitInfoList.Remove(OldValue);
                        }
                        RefreshStackPlaneList();
                    }
                }
            });
        }
        public void ShowEditMemoDialogExecute(MemoViewModel MemoDTO)
        {
            DialogParameters pairs = new DialogParameters();
            pairs.Add("oldValue", MemoDTO);
            _dialogService.Show("EditMemoUcView", pairs, callback =>
            {
                if (callback.Result == ButtonResult.OK)
                {
                    MemoDTO OldValue = callback.Parameters.GetValue<MemoDTO>("oldValue");
                    MemoDTO newValue = callback.Parameters.GetValue<MemoDTO>("newValue");
                    if (OldValue.Content != newValue.Content || OldValue.Title != newValue.Title)
                    {
                        MemoDTO.Title = newValue.Title;
                        MemoDTO.Content = newValue.Content;
                        MemoDTO.dataStatus = DataStatus.Alter;
                        RefreshStackPlaneList();
                    }
                }
            });
        }
        /// <summary>
        /// 改变WaitStatus
        /// </summary>
        /// <param name="WaitDTO"></param>
        public async void ChangeWaitingStateExecute(WaitVieModel WaitDTO)
        {
            WaitDTO.dataStatus = DataStatus.Alter;
            WaitDTO.Status = 1;
            await Task.Delay(300);
            NotResolveWaitInfoList.Remove(WaitDTO);
            RefreshStackPlaneList();
        }
        private async void DeleteMemoExecute(MemoViewModel model)
        {
            model.dataStatus = DataStatus.Delete;
            await Task.Delay(300);
            MemoInfoList.Remove(model);
            _dataService.ViewMemoList.Remove(model);
            RefreshStackPlaneList();
        }
        public void ShowAddMemoDialog()
        {
            _dialogService.ShowDialog("AddMemoUcView", callback =>
            {
                if (callback.Result == ButtonResult.OK)
                {
                    var MemoDTO = callback.Parameters.GetValue<MemoViewModel>("MemoDTO");
                    MemoDTO.AccountInfoId = _dataService.GetID();
                    MemoDTO.dataStatus = DataStatus.Add;
                    _dataService.ViewMemoList.Add(MemoDTO);
                    MemoInfoList.Add(MemoDTO);
                    RefreshStackPlaneList();
                }
            });
        }
        private void ShowAddWaitDialog()
        {
            _dialogService.ShowDialog("AddWaitUcView", (callback) =>
             {
                 if (callback.Result == ButtonResult.OK)
                 {
                     var WaitDTO = callback.Parameters.GetValue<WaitVieModel>("WaitDTO");
                     WaitDTO.AccountInfoId = _dataService.GetID();
                     WaitDTO.dataStatus = DataStatus.Add;
                     if (WaitDTO.Status == 0)
                     {
                         NotResolveWaitInfoList.Add(WaitDTO);
                     }
                     _dataService.ViewWaitList.Add(WaitDTO);
                     RefreshStackPlaneList();
                 }
             });
        }
        /// <summary>
        /// 刷新 界面Plane数据
        /// </summary>
        private void RefreshStackPlaneList()
        {
            StateWaitDTO.FinishCount = _dataService.GetWaitFinishCount();
            StateWaitDTO.WaitCount = _dataService.GetWaitCount();
            if (StateWaitDTO.WaitCount != 0)
                StateWaitDTO.FinishRate = (StateWaitDTO.FinishCount * 100.00 / StateWaitDTO.WaitCount).ToString("f2") + "%";

            StackPlaneList[0].Result = StateWaitDTO.WaitCount.ToString();
            StackPlaneList[1].Result = StateWaitDTO.FinishCount.ToString();
            StackPlaneList[2].Result = StateWaitDTO.FinishRate;
            StackPlaneList[3].Result = _dataService.ViewMemoList.Count.ToString();
        }
        /// <summary>
        /// 初始化备忘录数据
        /// </summary>
        public void InitStackPlaneList()
        {
            StackPlaneList = new List<StackPlaneInfo>();
            StackPlaneList.Add(new StackPlaneInfo() { Icon = "ClockFast", ItemName = "汇总", Result = "1000", BackGroundColor = "#FF0CA0FF", TextView = "总计" });
            StackPlaneList.Add(new StackPlaneInfo() { Icon = "ClockCheckOutline", ItemName = "已完成", Result = "800", BackGroundColor = "#FF1ECA3A", TextView = "成功" });
            StackPlaneList.Add(new StackPlaneInfo() { Icon = "ChartLineVariant", ItemName = "完成比例", Result = "90%", BackGroundColor = "#FF02C6DC", TextView = "失 败" });
            StackPlaneList.Add(new StackPlaneInfo() { Icon = "PlayListStar", ItemName = "备忘录", Result = "200", BackGroundColor = "#FFFFA000", TextView = "失 败" });
            RefreshStackPlaneList();
        }
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters.ContainsKey("AccountName"))
            {
                DateTime now = DateTime.Now;
                string[] week = new string[] { "星期天", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
                string loginName = navigationContext.Parameters["AccountName"] as string;
                AccountName = "欢迎您: " + loginName + ",今天是" + now.ToString("yyyy年MM月dd日") + " " + week[(int)now.DayOfWeek];
                RefreshStackPlaneList();
            }
            GetNotResolveWaitInfoList();
            GetMemoinfoList();
            RefreshStackPlaneList();
        }
        public void NavigateExecute(StackPlaneInfo stackPlane)
        {
            NavigationParameters pair = new NavigationParameters();
            if (stackPlane.ItemName == "已完成")
            {
                pair.Add("SelectIndex", 1);
                _regionManager.RequestNavigate("ContentRegion", "WaitUcView", callback =>
                {
                    _navigationJournal = callback.Context.NavigationService.Journal;
                }, pair);
            }
            else if (stackPlane.ItemName == "汇总")
            {
                pair.Add("SelectIndex", 0);
                _regionManager.RequestNavigate("ContentRegion", "WaitUcView", callback =>
                {
                    _navigationJournal = callback.Context.NavigationService.Journal;
                }, pair);
            }
            else if (stackPlane.ItemName == "备忘录")
            {
                _regionManager.RequestNavigate("ContentRegion", "MemoUcView", callback =>
                {
                    _navigationJournal = callback.Context.NavigationService.Journal;
                }, pair);
            }
        }
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }
        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }
    }
}
