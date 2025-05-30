using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NoteBook.API.DataModel
{
    /// <summary>
    /// 代办事项数据模型
    /// </summary>
    [Table("WaitInfo")]
    public class WaitInfo
    {
        [Key]
        public int Waitid { get; set; }
        [ForeignKey("AccountInfo")]
        public int AccountInfoId { get; set; } 
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreateTime { get; set; }=DateTime.Now;
        public virtual Accountinfo AccountInfo { get; set; } 
        //0表示未完成，1表示已完成
        public int Status { get; set; }
    }
}
