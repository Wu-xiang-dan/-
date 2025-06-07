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
    public class WaitDTO
    {
        public int Id { get; set; }
        public int AccountInfoId { get; set; }
        public string Title
        {
            get; set;
        }
        public string Content
        {
            get; set;
        }
        public int Status { get; set; }
    }
}
