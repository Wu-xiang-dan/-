using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation.Provider;
using static System.Net.WebRequestMethods;

namespace NoteBook.HttpClients
{
    /// <summary>
    /// 调用api tool class
    /// </summary>
    public class HttpRestClient
    {
        public readonly RestClient RestClient;
        public readonly string UrlBase = "http://localhost:7173/api/";
        public HttpRestClient()
        {
            RestClient = new RestClient(UrlBase);
        }
        public async Task<ApiResponse> ExecuteAsync(ApiRequest request, CancellationToken token)
        {
            try
            {
                var client = RestClient; 
                var restRequest = new RestRequest(request.Route, request.Method);
                // 设置内容类型
                restRequest.AddHeader("Content-Type", request.ContentType);
                if (request.Paramters != null)
                {
                    if (request.Method == Method.GET || request.Method == Method.DELETE)
                    {
                        // GET/DELETE 请求：将参数添加到 URL
                        if (request.Paramters is IDictionary<string, object> dictParams)
                        {
                            foreach (var param in dictParams)
                            {
                                restRequest.AddQueryParameter(param.Key, param.Value?.ToString());
                            }
                        }
                        else if (request.Paramters is IEnumerable<int> intList)
                        {
                            restRequest.AddQueryParameter("ids", string.Join(",", intList));
                        }
                    }
                    else
                    {
                        restRequest.AddJsonBody(request.Paramters);
                    }
                }
                var response = await client.ExecuteAsync(restRequest, token);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return JsonConvert.DeserializeObject<ApiResponse>(response.Content);
                }
                else
                {
                    return new ApiResponse
                    {
                        ResultCode = Result.Error,
                        Message = $"HTTP错误: {response.StatusCode}, 详情: {response.Content}"
                    };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    ResultCode = Result.Error,
                    Message = $"请求失败: {ex.Message}"
                };
            }
        }
    }
}
