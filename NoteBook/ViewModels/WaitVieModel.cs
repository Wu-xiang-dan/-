using NoteBook.DTOS;
using NoteBook.Models;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteBook.ViewModels
{
    public class WaitVieModel : BindableBase
    {
        public int Id { get; set; }
        public int AccountInfoId { get; set; }
        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        private string _content;
        public string Content
        {
            get { return _content; }
            set { SetProperty(ref _content, value); }
        }

        private int _status;
        public int Status { get { return _status; } set { SetProperty(ref _status, value); } }
        public DataStatus dataStatus = DataStatus.Normal;

        public WaitVieModel Clone()
        {
            return new WaitVieModel
            {
                Id  = this.Id,
                AccountInfoId = this.AccountInfoId,
                Title = this.Title,
                Content = this.Content,
                dataStatus = this.dataStatus // 值类型直接赋值
            };
        }

        public static WaitInfoDTO ConvertToWaitInfoDTO(WaitVieModel wait)
        {
            if (wait == null)
            {
                return null;
            }

            WaitInfoDTO waitInfoDTO = new WaitInfoDTO()
            {
                AccountInfoId = wait.AccountInfoId,
                Content = wait.Content,
                Title = wait.Title,
                Id = wait.Id
            };
            return waitInfoDTO;
        }
    }
}
