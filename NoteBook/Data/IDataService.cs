using NoteBook.DTOS;
using NoteBook.HttpClients;
using NoteBook.ViewModels;
using Prism.DryIoc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteBook.Data
{
    public interface IDataService
    {

        //上传数据
        ApiResponse UploadMemos();
        ApiResponse UploadWaits();

        ApiResponse AddMemo(MemoInfoDTO memoInfoDTO);
        ApiResponse AddWait(WaitInfoDTO waitInfoDTO);

        //改数据
        ApiResponse UpDataWait(WaitInfoDTO waitInfoDTO);
        ApiResponse UpDataMemo(MemoInfoDTO memoInfoDTO);

        ApiResponse DeleteWait(WaitInfoDTO waitInfoDTO);
        ApiResponse DeleteMemo(MemoInfoDTO memoInfoDTO);
        /// <summary>
        /// Memo 列表
        /// </summary>
        ObservableCollection<MemoViewModel> ViewMemoList { get; set; }
        ObservableCollection<WaitVieModel> ViewWaitList { get; set; }
        void LoadData();
        int GetMemoCount();
        int GetWaitFinishCount();  
        int GetWaitCount();
        void SetID(int id);
    }
}
