using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation.Provider;
using static System.Net.WebRequestMethods;

namespace NoteBook.HttpClients
{
    /// <summary>
    /// 调用api tool class
    /// </summary>
    class HttpRestClient
    {
        public readonly RestClient RestClient;
        public readonly string UrlBase = "http://localhost:7173/api/";
        public HttpRestClient()
        {
            RestClient = new RestClient();
        }
        public ApiResponse Execute(ApiRequest apiRequest)
        {
            //定义http请求并赋值请求方式
            RestRequest request = new RestRequest(apiRequest.Method);
            request.AddHeader("Content-Type", apiRequest.ContentType);
            if (apiRequest.Paramters != null)
            {
                request.AddParameter("param",JsonConvert.SerializeObject(apiRequest.Paramters),ParameterType.RequestBody);
            }
            RestClient.BaseUrl = new Uri(UrlBase+apiRequest.Route);
            //发起请求
            var res = RestClient.Execute(request);
            if (res.StatusCode == System.Net.HttpStatusCode.OK) { 
              return JsonConvert.DeserializeObject<ApiResponse>(res.Content);
            }
            else
            {
                return new ApiResponse() { ResultCode = Result.Error,Message="服务器丢失"};
            }
        }
    }
}
