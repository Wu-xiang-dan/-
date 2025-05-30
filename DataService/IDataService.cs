using NoteBook.DTOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteBook.DataService
{
    interface IDataService
    {
        /// <summary>
        /// 获取Memo
        /// </summary>
        /// <param name="memoList"></param>
        /// <returns></returns>
        List<MemoInfoDTO> GetMemoList();
        List<MemoInfoDTO> _memoList { get;set; }

    }
}
