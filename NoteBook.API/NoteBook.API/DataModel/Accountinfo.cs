using NoteBook.API.DTOS;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace NoteBook.API.DataModel
{
    [Table("AccountInfo")]
    public class Accountinfo
    {
        [Key]
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public virtual ICollection<NewMemoinfo> Memos { get; set; } 
        public virtual ICollection<WaitInfo> Waits { get; set; } 
    }
}
