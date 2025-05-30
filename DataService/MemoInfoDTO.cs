using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteBook.DTOS
{
    public interface MemoInfoDTO
    {
        /// <summary>
        /// 备忘录DTO
        /// </summary>
        class MemoInfoDTO
        {
            public int MemoID { get; set; }
            public string Title { get; set; }
            public string Content { get; set; }
        }
    }
}
