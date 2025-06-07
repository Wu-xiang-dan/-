using NoteBook.Data;
using NoteBook.DTOS;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.Linq;
using NoteBook.ViewModels;
using NoteBook.Models;
using Prism.Events;
using NoteBook.MegEvents;
namespace Memo.ViewModels
{
    class MemoUcViewModel : BindableBase, INavigationAware
    {
        private IDataService _dataService;
        private IEventAggregator _eventAggregator;
        private ObservableCollection<MemoViewModel> _memoInfoList;//Memo显示数据
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
        public DelegateCommand AddMomoCommad { get; set; }
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
        private MemoViewModel _memo;
        public MemoViewModel Memo
        {
            get { return _memo; }
            set { _memo = value;
               RaisePropertyChanged(nameof(Memo));
            }
        }
        public MemoUcViewModel(IDataService dataService,IEventAggregator eventAggregator)
        {
            this._dataService = dataService;
            _eventAggregator = eventAggregator;
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
                _eventAggregator.GetEvent<MesEvent>().Publish("未查询到相关备忘录数据!");
                MemoInfoList = SearchResult;
                return;
            }
            MemoInfoList = SearchResult;
        }
        public void AddMomoExcute()
        {
            if (string.IsNullOrEmpty(Memo.Content) || string.IsNullOrEmpty(Memo.Title))
            {
                return;
            }
            MemoViewModel newmemo = Memo.Clone();
            newmemo.dataStatus = DataStatus.Add;
            newmemo.AccountInfoId = _dataService.GetID();
            MemoInfoList.Add(new MemoViewModel() { Content = Memo.Content, Title= Memo.Title });
            _dataService.ViewMemoList.Add(newmemo);
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
