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
        ObservableCollection<MemoViewModel> ViewMemoList { get; set; }
        ObservableCollection<WaitVieModel> ViewWaitList { get; set; }
        Task<List<ApiResponse>> UploadMemosAsync();// 上传备忘录
        Task<List<ApiResponse>> UploadWaitsAsync();// 上传待办事项
        Task<string> LoadDataAsync();//同步数据或加载本地数据
        int GetMemoCount();
        int GetWaitFinishCount();
        int GetWaitCount();
        void SetID(int id);// 设置当前用户ID
        int GetID();
    }
}
