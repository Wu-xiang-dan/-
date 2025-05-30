using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteBook.DTOS
{
    //接收api统计的代办事项模型
    public class StateWaitDTO
    {
        public int WaitCount { get; set; } //代办数量
        public int FinishCount { get; set; } //已完成数量
        public string FinishRate { get; set; }//完成比例
    }
}
