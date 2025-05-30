using Microsoft.EntityFrameworkCore;

namespace NoteBook.API.DataModel
{
    public class NoteBookDBContext:DbContext
    {
        public NoteBookDBContext(DbContextOptions<NoteBookDBContext> options):base(options)
        {

        }  
        public DbSet<Accountinfo> Accountinfos { get; set; }
        public DbSet<WaitInfo> WaitInfos { get; set; }
        public DbSet<NewMemoinfo> MemoInfos { get; set; }
    }
}
