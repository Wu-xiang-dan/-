using NoteBook.Data;
using NoteBook.DTOS;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using NoteBook.HttpClients;
using NoteBook.ViewModels;
using NoteBook.Models;
namespace Memo.ViewModels
{
    class MemoUcViewModel : BindableBase, INavigationAware
    {
        private IDataService _dataService;
        private ObservableCollection<MemoViewModel> _memoInfoList;//Memo显示数据
        private Visibility _visibility=Visibility.Hidden;
        public Visibility Visibility
        {
            get { return _visibility; }
            set { _visibility = value;
                RaisePropertyChanged(nameof(Visibility));
            }
        }
        private string _searchTitle;
        public string SearchTitle
        {
            get { return _searchTitle; }
            set
            {
                _searchTitle = value;
                RaisePropertyChanged(nameof(SearchTitle));
            }
        }
        private bool _IsShowRightDialog;
        public bool IsShowRightDialog
        {
            get { return _IsShowRightDialog; }
            set
            {
                _IsShowRightDialog = value;
                RaisePropertyChanged(nameof(IsShowRightDialog));
            }
        }
        public DelegateCommand ShowAddMemoDialogCommand { get; set; }
        public DelegateCommand ShowRightDialogCommand { get; set; }
        public DelegateCommand UpDataMemoListCommand { get; set; }
        public DelegateCommand<MemoViewModel> DeleteMemoCommand { get; set; }
        public ObservableCollection<MemoViewModel> MemoInfoList
        {
            get { return _memoInfoList; }
            set
            {
                SetProperty(ref _memoInfoList, value);
                RaisePropertyChanged(nameof(MemoInfoList));
            }
        }
        public MemoUcViewModel(IDataService dataService)
        {
            this._dataService = dataService;
            UpDataMemoListCommand = new DelegateCommand(UpDataMemoList);
            ShowAddMemoDialogCommand = new DelegateCommand(ShowAddMemoDialog);
            DeleteMemoCommand = new DelegateCommand<MemoViewModel>(DeleteWaitEecute);
        }
        public void ShowAddMemoDialog()
        { 
            IsShowRightDialog = true;
        }
        public void UpDataMemoList()
        {
            if(SearchTitle==null)
                return;

            if (SearchTitle=="")
            {
                MemoInfoList = new ObservableCollection<MemoViewModel>(MemoInfoList);
                return;
            }    
            var SearchResult= new ObservableCollection<MemoViewModel>(MemoInfoList.Where(t => t.Title.Contains(SearchTitle)));
            if (SearchResult.Count==0)
            {
                Visibility= Visibility.Visible;
                MemoInfoList = SearchResult;
                return;
            }
            MemoInfoList = SearchResult;
            Visibility = Visibility.Hidden;
        }
        public void DeleteWaitEecute(MemoViewModel memo)
        {
            MemoInfoList.Remove(memo);
            memo.dataStatus = DataStatus.Delete;
        }
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            MemoInfoList = new ObservableCollection<MemoViewModel>(_dataService.ViewMemoList.Where(t => t.dataStatus != DataStatus.Delete));
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
