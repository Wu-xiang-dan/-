namespace NoteBook.API.ApiResponse
{
    public enum Result{
        Success = 0,
        Error = 1,
        NotFound = 404,
        Failed = 500
    }
    /// <summary>
    /// 接受的 模型
    /// </summary>
    public class Response
    {

        public Result ResultCode { get; set; }
        public string? Message { get; set; }
        public object? ResultData { get; set; }
    }
}
