using Newtonsoft.Json;
using NoteBook.DTOS;
using NoteBook.HttpClients;
using RestSharp;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteBook.DataService
{
    class DataServicers : IDataService
    {
        private readonly HttpRestClient _httpRestClient;
        private ApiRequest _apiRequest;
        private ApiResponse _apiResponse;
        public DataServicers(HttpRestClient httpRestClient, ApiRequest apiRequest, ApiResponse apiResponse)
        {
            _httpRestClient = httpRestClient;
            _apiRequest = apiRequest;
            _apiResponse = apiResponse;
        }
        public List<MemoInfoDTO> _memoList{
            get;
            
            set;
        }


        public List<MemoInfoDTO> GetMemoList()
        {
             _apiRequest = new ApiRequest() { Method = Method.GET, Route = "Memo/GetMemoList" };
             _apiResponse = _httpRestClient.Execute(_apiRequest);
            if (_apiResponse.ResultCode == Result.Success)
            {
               _memoList = JsonConvert.DeserializeObject<List<MemoInfoDTO>>(_apiResponse.ResultData.ToString());
                return _memoList;
            }
            else
            {
                //失败通知3
            }
            return null;
        }
    }
}
