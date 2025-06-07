using NoteBook.Data;
using NoteBook.Models;
using NoteBook.ViewModels;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using NoteBook.MegEvents;
using System.Windows;


namespace Wait.ViewModels
{
    class WaitUcViewModel : BindableBase, INavigationAware
    {
        private ObservableCollection<WaitVieModel> _waitInfoList = new ObservableCollection<WaitVieModel>();
        private IDataService _dataService;
        private IEventAggregator _eventAggregator;
        private bool _isShowRightDialog;
        public DelegateCommand ShowRightDialogCommand { get; set; }
        public DelegateCommand UpDateWaitListCommand { get; set; }
        public DelegateCommand AddWaitCommand { get; set; }
        public DelegateCommand<WaitVieModel> DeleteWaitCommand { get; set; }
        public bool IsShowRightDialog
        {
            get { return _isShowRightDialog; }
            set
            {
                _isShowRightDialog = value;
                RaisePropertyChanged(nameof(IsShowRightDialog));
            }
        }
        public ObservableCollection<WaitVieModel> WaitInfoList
        {
            get { return _waitInfoList; }
            set
            {
                SetProperty(ref _waitInfoList, value);
                RaisePropertyChanged(nameof(WaitInfoList));
            }
        }
        private string _searchWaitTitle;
        private string _status; //0表示全部，1表示已完成，2表示未完成
        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                RaisePropertyChanged(nameof(Status));
            }
        }
        public string SearchWaitTitle
        {
            get { return _searchWaitTitle; }
            set
            {
                _searchWaitTitle = value;
                RaisePropertyChanged(nameof(SearchWaitTitle));
            }
        }
        private WaitVieModel _wait=new WaitVieModel();
        public WaitVieModel Wait
        {
            get { return _wait; }
            set { _wait = value; }
        }
        public WaitUcViewModel(IDataService dataService,IEventAggregator eventAggregator)
        {
            _dataService = dataService;
            _eventAggregator= eventAggregator;
            InitWaitInfoList();
            ShowRightDialogCommand = new DelegateCommand(ShowRightDialog);
            UpDateWaitListCommand = new DelegateCommand(UpdateWaitInfoList);
            AddWaitCommand = new DelegateCommand(AddWaitExecute);
            DeleteWaitCommand = new DelegateCommand<WaitVieModel>(DeleteWaitEecute);
        }

        public void ShowRightDialog()
        {
            IsShowRightDialog = true;
        }
        private void InitWaitInfoList()
        {
            try
            {
                WaitInfoList = new ObservableCollection<WaitVieModel>(_dataService.ViewWaitList);
            }
            catch (Exception)
            {

                throw;
            }
            return;
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        public void UpdateWaitInfoList()
        {
            if (string.IsNullOrEmpty(Status) && string.IsNullOrEmpty(SearchWaitTitle))
            {
                return;
            }
            //Status和SearchWaitTitle都不为空时
            if (!string.IsNullOrEmpty(Status) && !string.IsNullOrEmpty(SearchWaitTitle))
            {
                if (Status == "0")
                {
                    WaitInfoList = new ObservableCollection<WaitVieModel>(_dataService.ViewWaitList.Where(t => t.Title.Contains(SearchWaitTitle)&&t.dataStatus!=DataStatus.Delete));
                }
                else if (Status == "1")
                {
                    WaitInfoList = new ObservableCollection<WaitVieModel>(_dataService.ViewWaitList.Where(t => t.Title.Contains(SearchWaitTitle) && t.Status == 1&&t.dataStatus!=DataStatus.Delete));
                }
                else
                {
                    WaitInfoList = new ObservableCollection<WaitVieModel>(_dataService.ViewWaitList.Where(t => t.Title.Contains(SearchWaitTitle) && t.Status == 0 && t.dataStatus != DataStatus.Delete));
                }
            }
            else if (string.IsNullOrEmpty(SearchWaitTitle))
            {
                if (Status == "0")
                {
                    WaitInfoList = new ObservableCollection<WaitVieModel>(_dataService.ViewWaitList.Where(t=>t.dataStatus!=DataStatus.Delete));
                    return;
                }
                else if (Status == "1")
                {
                    WaitInfoList = new ObservableCollection<WaitVieModel>(_dataService.ViewWaitList.Where(t => t.Status == 1&&t.dataStatus!=DataStatus.Delete));
                }
                else
                {
                    WaitInfoList = new ObservableCollection<WaitVieModel>(_dataService.ViewWaitList.Where(t => t.Status == 0&&t.dataStatus!=DataStatus.Delete));
                }
            }
            else
            {
                WaitInfoList = new ObservableCollection<WaitVieModel>(_dataService.ViewWaitList.Where(t => t.Title.Contains(SearchWaitTitle) && t.dataStatus != DataStatus.Delete));
            }
            if(WaitInfoList.Count==0)
              _eventAggregator.GetEvent<MesEvent>().Publish("未查询到相关待办数据!");

        }
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Status=navigationContext.Parameters.GetValue<string>("SelectIndex");
            WaitInfoList = new ObservableCollection<WaitVieModel>(_dataService.ViewWaitList.Where(t => t.dataStatus != DataStatus.Delete));
            UpdateWaitInfoList();
        }
        public void AddWaitExecute()
        {
            if (string.IsNullOrEmpty(Wait.Content)||string.IsNullOrEmpty(Wait.Title))
            {
                return;
            }    
            WaitVieModel newWait =Wait.Clone();
            newWait.dataStatus=DataStatus.Add;
            newWait.AccountInfoId = _dataService.GetID();
            WaitInfoList.Add(new WaitVieModel() {Content=Wait.Content,Status=Convert.ToInt32(Wait.Status),Title=Wait.Title });
            _dataService.ViewWaitList.Add(newWait);
        }
        public void DeleteWaitEecute(WaitVieModel wait)
        {
            wait.dataStatus = DataStatus.Delete;
            WaitInfoList.Remove(wait);
            
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
