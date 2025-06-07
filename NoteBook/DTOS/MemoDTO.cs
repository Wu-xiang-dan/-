using NoteBook.Models;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteBook.DTOS
{
    /// <summary>
    /// 备忘录DTO
    /// </summary>
    public class MemoDTO
    {
        public int MemoID { get; set; }
        public int AccountInfoId { get; set; }
        public string Title
        {
            get;set;
        }
        public string Content
        {
            get;set;
        }
        public DataStatus status = DataStatus.Normal;
    }
}
