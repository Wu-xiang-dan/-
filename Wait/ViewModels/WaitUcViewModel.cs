using ImTools;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NoteBook.Data;
using NoteBook.DTOS;
using NoteBook.HttpClients;
using NoteBook.Models;
using NoteBook.ViewModels;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace Wait.ViewModels
{
    class WaitUcViewModel : BindableBase, INavigationAware
    {
       // private List<WaitInfoDTO> DataBase = new List<WaitInfoDTO>();
        private ObservableCollection<WaitVieModel> _waitInfoList = new ObservableCollection<WaitVieModel>();
        private IDataService _dataService;
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
        private Visibility _visibility = Visibility.Hidden;
        public Visibility Visibility
        {
            get { return _visibility; }
            set
            {
                _visibility = value;
                RaisePropertyChanged(nameof(Visibility));
            }
        }
        public WaitUcViewModel(IDataService dataService)
        {
            _dataService = dataService;
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
            Visibility= WaitInfoList.Count == 0 ? Visibility = Visibility.Visible : Visibility = Visibility.Hidden;

        }
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Status=navigationContext.Parameters.GetValue<string>("SelectIndex");
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
            WaitInfoList.Add(new WaitVieModel() {Content=Wait.Content,Status=Convert.ToInt32(Status),Title=Wait.Title });
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
