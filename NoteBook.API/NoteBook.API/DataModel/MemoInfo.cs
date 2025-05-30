using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NoteBook.API.DataModel
{
    [Table("NewMemoInfo")]
    public class NewMemoinfo
    {   
        [Key]
        public int MemoID { get; set; }
        [ForeignKey("AccountInfo")]
        public int AccountInfoId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreateTime { get; set; } = DateTime.Now;
        public virtual Accountinfo AccountInfo { get; set; } 
    }
}
