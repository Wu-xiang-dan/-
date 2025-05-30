using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteBook.HttpClients
{
    public class ApiRequest
    {
        //请求地址api路由地址
        public string Route { get; set; }
        //请求方式（get/post/delete/put）
        public Method Method { get; set; }

        public object Paramters { get; set; }
        /// <summary>
        /// 发送的 数据类型
        /// </summary>
        public string ContentType { get; set; } = "application/json";
    }
}
