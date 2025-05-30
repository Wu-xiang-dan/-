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
   public class MemoViewModel:BindableBase
    {
        public int MemoID { get; set; }
        public int AccountInfoId { get; set; }
        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged(nameof(Title));
            }
        }
        private string _content;
        public string Content
        {
            get { return _content; }
            set
            {
                _content = value;
                RaisePropertyChanged(nameof(Content));
            }
        }
        public DataStatus dataStatus = DataStatus.Normal;

        /// <summary>
        /// 将ViewModel 转换为DTO
        /// </summary>
        /// <param name="memo"></param>
        /// <returns></returns>
        public static MemoInfoDTO ConvertToMemoInfoDTO(MemoViewModel memo)
        {
            if (memo == null)
            {
                return null;
            }

           MemoInfoDTO memoInfoDTO=new MemoInfoDTO()
           {
               AccountInfoId = memo.AccountInfoId,
               Content = memo.Content,
               Title = memo.Title,
               MemoID = memo.MemoID
           };
            return memoInfoDTO;
        }
        public MemoViewModel Clone()
        {
            return new MemoViewModel
            {
                MemoID = this.MemoID,
                AccountInfoId = this.AccountInfoId,
                Title = this.Title,
                Content = this.Content,
                dataStatus = this.dataStatus // 值类型直接赋值
            };
        }
    }
}
