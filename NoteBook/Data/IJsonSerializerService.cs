using NoteBook.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteBook.Data
{
    public interface IJsonSerializerService
    {
        void SaveMemosToJson(List<MemoViewModel> memos);
        void SaveWaitsToJson(List<WaitVieModel> waits);
        Task<List<WaitVieModel>> LoadingWaitsJsonAsync();
        Task<List<MemoViewModel>> LoadingMemosJsonAsync();
    }
}
