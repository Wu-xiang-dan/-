using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteBook.HttpClients
{
    public enum Result
    {
        Success = 0,
        Error = 1,
        NotFound = 404,
        Failed=500
    }
    class ApiResponse
    {
        public Result ResultCode { get; set; }
        public string? Message { get; set; }
        public object? ResultData { get; set; }
    }
}
