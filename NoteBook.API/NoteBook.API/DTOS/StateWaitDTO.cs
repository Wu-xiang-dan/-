namespace NoteBook.API.DTOS
{
    //统计代办模型
    public class StateWaitDTO
    {
        public int WaitCount { get; set; } //代办数量
        public int FinishCount { get; set; } //已完成数量
        public string FinishRate
        {
            get {
                if (WaitCount==0)
                {
                    return "0.00%";
                }
                return (FinishCount * 100.00 / WaitCount).ToString("f2")+"%";
            }
        }
    }
}
