namespace NoteBook.API.DTOS
{
    public class WaitDTO
    {
        public int Id { get; set; }
        public int AccountInfoId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int Status { get; set; }
    }
}
