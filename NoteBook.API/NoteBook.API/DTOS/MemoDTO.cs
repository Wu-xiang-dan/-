namespace NoteBook.API.DTOS
{
    public class MemoDTO
    {
        public int MemoID { get; set; }
        public int AccountInfoId { get; set; }
        public int AccountID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DataStatus status = DataStatus.Normal;
    }
    public enum DataStatus
    {
        Normal = 0,
        Delete = 1,
        Add = 2,
        Alter = 3
    }
}
