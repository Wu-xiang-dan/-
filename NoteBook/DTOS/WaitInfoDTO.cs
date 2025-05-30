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
    /// 代办事项DTO
    /// </summary>
   public class WaitInfoDTO
    {
        
        public int Id { get; set; }
        public int AccountInfoId { get; set; }
        private string _title;
        public string Title { get;set;
        }
        private string _content;
        public string Content
        {
            get;set;
        }
        private int _status;
        public int Status { get; set; }
        public DataStatus status = DataStatus.Normal;
    }
}
