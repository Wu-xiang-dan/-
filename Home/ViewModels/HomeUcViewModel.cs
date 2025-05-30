using NoteBook.DTOS;
using Home.Models;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestSharp;
using NoteBook.Data;
using NoteBook.HttpClients;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Windows;
using Prism.Commands;
using Prism.Services.Dialogs;
using DryIoc;
using System.Linq;
using NoteBook.ViewModels;
using NoteBook.Models;
namespace Home.ViewModels
{
    class HomeUcViewModel : BindableBase, INavigationAware
    {
        public HomeUcViewModel(IDialogService dialogService, IRegionManager regionManager, IDataService dataServicers, IContainer container)
        {
            _dialogService = dialogService;
            _dataService = dataServicers;
            _regionManager = regionManager;
            ChangeWaitingStateCommand = new DelegateCommand<WaitVieModel>(ChangeWaitingStateExecute);
            ChangeMemoStateCommand = new DelegateCommand<MemoViewModel>(ChangeMemoStateExecute);
            ShowAddWaitDialogCommand = new DelegateCommand(ShowAddWaitDialog);
            ShowAddMemoDialogCommand = new DelegateCommand(ShowAddMemoDialog);
            ShowEditWaitDialogCommand = new DelegateCommand<WaitVieModel>(ShowEditWaitDialogExecute);
            ShowEditMemoDialogCommand = new DelegateCommand<MemoViewModel>(ShowEditMemoDialogExecute);
            NavigateCommand = new DelegateCommand<StackPlaneInfo>(NavigateExecute);
            InitStackPlaneList();
        }
        public DelegateCommand<MemoViewModel> ChangeMemoStateCommand { get; set; }
        public DelegateCommand<WaitVieModel> ChangeWaitingStateCommand { get; set; }
        public DelegateCommand ShowAddWaitDialogCommand { get; set; }
        public DelegateCommand ShowAddMemoDialogCommand { get; set; }
        public DelegateCommand<WaitVieModel> ShowEditWaitDialogCommand { get; set; }
        public DelegateCommand<MemoViewModel> ShowEditMemoDialogCommand { get; set; }
        public DelegateCommand<StackPlaneInfo> NavigateCommand { get; set; }
        private readonly IDataService _dataService;
        private IRegionNavigationJournal _navigationJournal;
        private readonly IRegionManager _regionManager;
        private readonly IDialogService _dialogService;
        private StateWaitDTO StateWaitDTO = new StateWaitDTO();
        private List<StackPlaneInfo> _stackPlaneList;
        private ObservableCollection<WaitVieModel> _notResolveWaitInfoList;
        private ObservableCollection<MemoViewModel> _memoInfoList=new ObservableCollection<MemoViewModel>();
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
        }
        public ObservableCollection<WaitVieModel> NotResolveWaitInfoList
        {
            get { return _notResolveWaitInfoList; }
            set
            {
                SetProperty(ref _notResolveWaitInfoList, value);
                RaisePropertyChanged(nameof(NotResolveWaitInfoList));
            }
        }
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
                NotResolveWaitInfoList = new ObservableCollection<WaitVieModel>(_dataService.ViewWaitList.Where(t=>t.Status==0));

            }
            catch (Exception)
            {

            }
        }
        public void GetMemoinfoList()
        {
            try
            {
                MemoInfoList = new ObservableCollection<MemoViewModel>(_dataService.ViewMemoList);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #region DelegateCommand Execute
        public void ShowEditWaitDialogExecute(WaitVieModel waitInfoDTO)
        {
            DialogParameters pairs = new DialogParameters();
            pairs.Add("oldValue", waitInfoDTO);
            _dialogService.Show("EditWaitUcView", pairs, callback =>
            {
                if (callback.Result == ButtonResult.OK)
                {
                    WaitVieModel OldValue = callback.Parameters.GetValue<WaitVieModel>("oldValue");
                    WaitVieModel newValue = callback.Parameters.GetValue<WaitVieModel>("newValue");

                    if (OldValue.Status != newValue.Status || OldValue.Content != newValue.Content || OldValue.Title != newValue.Title)
                    {
                        waitInfoDTO.Title = newValue.Title;
                        waitInfoDTO.Content = newValue.Content;
                        waitInfoDTO.Status = newValue.Status;
                        if (newValue.Status==1)
                        NotResolveWaitInfoList.Remove(OldValue);

                        RefreshStackPlaneList();
                        try
                        {
                            ApiResponse response = _dataService.UpDataWait(WaitVieModel.ConvertToWaitInfoDTO(newValue));
                        }
                        catch (Exception)
                        {
                            //通知                       
                        }
                    }
                }
            });
        }
        public void ShowEditMemoDialogExecute(MemoViewModel memoInfoDTO)
        {
            DialogParameters pairs = new DialogParameters();
            pairs.Add("oldValue", memoInfoDTO);
            _dialogService.Show("EditMemoUcView", pairs, callback =>
            {
                if (callback.Result == ButtonResult.OK)
                {
                    MemoInfoDTO OldValue = callback.Parameters.GetValue<MemoInfoDTO>("oldValue");
                    MemoInfoDTO newValue = callback.Parameters.GetValue<MemoInfoDTO>("newValue");
                    if (OldValue.Content != newValue.Content || OldValue.Title != newValue.Title)
                    {
                        memoInfoDTO.Title = newValue.Title;
                        memoInfoDTO.Content = newValue.Content;
                        RefreshStackPlaneList();
                        ApiResponse response = _dataService.UpDataMemo(MemoViewModel.ConvertToMemoInfoDTO(memoInfoDTO));
                    }
                }
            });
        }
        /// <summary>
        /// 改变WaitStatus
        /// </summary>
        /// <param name="waitInfoDTO"></param>
        public async void ChangeWaitingStateExecute(WaitVieModel waitInfoDTO)
        {
            waitInfoDTO.dataStatus = DataStatus.Alter;

            await Task.Delay(300);
            NotResolveWaitInfoList.Remove(waitInfoDTO);

        }
        private void ChangeMemoStateExecute(MemoViewModel model)
        {
            
        }
        public void ShowAddMemoDialog()
        {
            _dialogService.ShowDialog("AddMemoUcView", callback =>
            {
                if (callback.Result == ButtonResult.OK)
                {
                    var memoInfoDTO = callback.Parameters.GetValue<MemoViewModel>("MemoInfoDTO");                
                    MemoInfoList.Add(memoInfoDTO);                   
                    RefreshStackPlaneList();
                    ApiResponse response = _dataService.AddMemo(MemoViewModel.ConvertToMemoInfoDTO(memoInfoDTO));
                    if (response.ResultCode == Result.Success)
                    {
                        //添加成功
                    }
                }
            });
        }
        private void ShowAddWaitDialog()
        {
            _dialogService.ShowDialog("AddWaitUcView", (callback) =>
             {
                 if (callback.Result == ButtonResult.OK)
                 {
                     var waitInfoDTO = callback.Parameters.GetValue<WaitVieModel>("WaitInfoDTO");
                     if (waitInfoDTO.Status==0)
                     NotResolveWaitInfoList.Add(new WaitVieModel() {Content=waitInfoDTO.Content,Id=waitInfoDTO.Id,Status=waitInfoDTO.Status,Title=waitInfoDTO.Title });
                     var response = _dataService.AddWait(WaitVieModel.ConvertToWaitInfoDTO(waitInfoDTO));
                     if (response.ResultCode == Result.Success)
                     {
                         if (waitInfoDTO.Status == 1)
                         {
                             StateWaitDTO.FinishCount += 1;
                         }
                         StateWaitDTO.WaitCount += 1;
                     }
                     RefreshStackPlaneList();
                 }
             });
        }
        #endregion

        #region Command Method
        /// <summary>
        /// 刷新 界面Plane数据
        /// </summary>
        private void RefreshStackPlaneList()
        {
            try
            {
                StateWaitDTO.FinishCount =_dataService.GetWaitFinishCount();
                StateWaitDTO.WaitCount = _dataService.GetWaitCount();
            }
            catch (Exception)
            {

            }
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
        #endregion
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }
        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

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
    }
}
