using Newtonsoft.Json;
using NoteBook.DTOS;
using NoteBook.HttpClients;
using NoteBook.ViewModels;
using RestSharp;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoteBook.Models;
using System.Windows;
using ImTools;
namespace NoteBook.Data
{
    public class DataService : IDataService
    {
        private int id = 0;
        private readonly IJsonSerializerService _jsonSerializerService;
        private readonly HttpRestClient _httpRestClient;
        private ApiRequest _apiRequest;
        private ApiResponse _apiResponse;
        public DataService(HttpRestClient httpRestClient, ApiRequest apiRequest, ApiResponse apiResponse, IJsonSerializerService jsonSerializerService)
        {
            _jsonSerializerService = jsonSerializerService;
            _httpRestClient = httpRestClient;
            _apiRequest = apiRequest;
            _apiResponse = apiResponse;
        }
        private List<MemoViewModel> _memoList
        {
            get;
            set;
        }
        private List<WaitVieModel> _waitList { get; set; }

        private ObservableCollection<MemoViewModel> _viewMemoList;
        public ObservableCollection<MemoViewModel> ViewMemoList
        {
            get { return _viewMemoList; }
            set { _viewMemoList = value; }
        }

        private ObservableCollection<WaitVieModel> _viewWaitList;
        public ObservableCollection<WaitVieModel> ViewWaitList
        {
            get { return _viewWaitList; }
            set { _viewWaitList = value; }
        }

        /// <summary>
        /// 获取备忘录事项
        /// </summary>
        /// <returns></returns>
        private void GetMemoList(int id)
        {
            _apiRequest = new ApiRequest() { Method = Method.GET, Route = $"Memo/GetMemoList?id={id}" };
            _apiResponse = _httpRestClient.Execute(_apiRequest);
            if (_apiResponse.ResultCode == Result.Success)
            {
                _memoList = JsonConvert.DeserializeObject<List<MemoViewModel>>(_apiResponse.ResultData.ToString());
            }
            else
            {
                _memoList = _jsonSerializerService.LoadingMemosJson();
            }
            ViewMemoList = new ObservableCollection<MemoViewModel>(_memoList.Select(t => t.Clone()).ToList());
        }
        /// <summary>
        /// 获取待 办事项
        /// </summary>
        /// <returns></returns>
        private void GetWaitList(int id)
        {
            _apiRequest = new ApiRequest() { Method = Method.GET, Route = $"Wait/GetWaitings?id={id}" };
            _apiResponse = _httpRestClient.Execute(_apiRequest);
            if (_apiResponse.ResultCode == Result.Success)
            {
                _waitList = JsonConvert.DeserializeObject<List<WaitVieModel>>(_apiResponse.ResultData.ToString());
            }
            else
            {
                _waitList = _jsonSerializerService.LoadingWaitsJson();
            }
            ViewWaitList = new ObservableCollection<WaitVieModel>(_waitList.Select(t => t.Clone()).ToList());
        }
        /// <summary>
        /// 添加备忘录事项
        /// </summary>
        /// <param name="memoList">待添加的项</param>
        /// <returns>Response</returns>
        public ApiResponse AddMemo(MemoInfoDTO memo)
        {
            _memoList.Add(new MemoViewModel() { Title = memo.Title, Content = memo.Content, dataStatus = DataStatus.Add, AccountInfoId = id });

            _apiRequest = new ApiRequest() { Method = Method.POST, Route = "Memo/AddMemo", Paramters = memo };
            _apiResponse = _httpRestClient.Execute(_apiRequest);
            return _apiResponse;
        }
        public ApiResponse AddWait(WaitInfoDTO wait)
        {
            _waitList.Add(new WaitVieModel() { Title = wait.Title, Content = wait.Content, dataStatus = DataStatus.Add, AccountInfoId = id, Status = wait.Status });

            _apiRequest = new ApiRequest() { Method = Method.POST, Route = "Wait/AddWait", Paramters = wait };
            _apiResponse = _httpRestClient.Execute(_apiRequest);
            return _apiResponse;
        }
        public ApiResponse UpDataWait(WaitInfoDTO waitInfoDTO)
        {
            var db = _waitList.Where(t => t.Id == waitInfoDTO.Id).FirstOrDefault();
            db.Status = waitInfoDTO.Status;
            db.Title = waitInfoDTO.Title;
            db.Content = waitInfoDTO.Content;
            db.dataStatus = DataStatus.Alter;

            _apiRequest = new ApiRequest() { Method = Method.PUT, Paramters = waitInfoDTO, Route = "Wait/UpDataWait" };
            _apiResponse = _httpRestClient.Execute(_apiRequest);
            return _apiResponse;
        }
        public ApiResponse UpDataMemo(MemoInfoDTO memoInfoDTO)
        {
            var db = _memoList.Where(t => t.MemoID == memoInfoDTO.MemoID).FirstOrDefault();
            db.Title = memoInfoDTO.Title;
            db.Content = memoInfoDTO.Content;
            db.dataStatus = DataStatus.Alter;

            _apiRequest = new ApiRequest() { Method = Method.POST, Route = "Memo/UpdateMemo", Paramters = memoInfoDTO };
            _apiResponse = _httpRestClient.Execute(_apiRequest);
            return _apiResponse;
        }
        public ApiResponse DeleteMemo(MemoInfoDTO memo)
        {
            var db = _memoList.Where(t => t.MemoID == memo.MemoID).FirstOrDefault();
            _memoList.Remove(db);
            try
            {
                _apiRequest = new ApiRequest() { Method = Method.DELETE, Route = $"Memo/DeleteMemo?id={memo.MemoID}" };
                _apiResponse = _httpRestClient.Execute(_apiRequest);
                return _apiResponse;
            }
            catch (MissingMethodException ex)
            {
                Console.WriteLine($"DeleteWait 方法中抛出 MissingMethodException: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }
        }
        public ApiResponse DeleteWait(WaitInfoDTO waitInfoDTO)
        {
            var db = _waitList.Where(t => t.Id == waitInfoDTO.Id).FirstOrDefault();
            _waitList.Remove(db);
            try
            {
                _apiRequest = new ApiRequest() { Method = Method.DELETE, Route = $"Wait/DeleteWait?id={waitInfoDTO.Id}" };
                _apiResponse = _httpRestClient.Execute(_apiRequest);
                return _apiResponse;
            }
            catch (MissingMethodException ex)
            {
                Console.WriteLine($"DeleteWait 方法中抛出 MissingMethodException: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }
        }
        public ApiResponse UploadWaits()
        {
            _waitList = _jsonSerializerService.LoadingWaitsJson().Where(t => t.dataStatus != DataStatus.Delete).ToList();
            var waits = _waitList.Where(t => t.dataStatus != Models.DataStatus.Normal);
            _apiRequest = new ApiRequest() { Method = Method.POST, Route = "Wait/UploadWaits", Paramters = waits };
            _apiResponse = _httpRestClient.Execute(_apiRequest);
            if (_apiResponse.ResultCode == Result.NotFound)
            {
                return _apiResponse;
            }
            else if (_apiResponse.ResultCode == Result.Error)//部分或全部数据上传失败
            {
                // 上传失败
                return null;
            }
            else if (_apiResponse.ResultCode == Result.Success)
            {
                // 上传成功
                return _apiResponse;
            }
            return _apiResponse;
        }
        public ApiResponse UploadMemos()
        {
            _memoList = _jsonSerializerService.LoadingMemosJson().Where(t => t.dataStatus != DataStatus.Delete).ToList();
            var memos = _memoList.Where(t => t.dataStatus != Models.DataStatus.Normal);

            _apiRequest = new ApiRequest() { Method = Method.POST, Route = "Memo/UploadMemos", Paramters = memos };
            _apiResponse = _httpRestClient.Execute(_apiRequest);
            if (_apiResponse.ResultCode == Result.NotFound)
            {
                return _apiResponse;
            }
            else if (_apiResponse.ResultCode == Result.Error)//部分或全部数据上传失败
            {
                // 上传失败
                return null;
            }
            else if (_apiResponse.ResultCode == Result.Success)
            {
                return _apiResponse;
            }
            return _apiResponse;
        }
        /// <summary>
        /// 设置用户id
        /// </summary>
        /// <param name="id"></param>
        public void SetID(int id)
        {
            this.id = id;
        }

        /// <summary>
        /// 载入 数据
        /// </summary>
        public void LoadData()
        {
            GetMemoList(id);
            GetWaitList(id);
        }
        public int GetMemoCount()
        {
            return _viewMemoList.Count(t => t.dataStatus != DataStatus.Delete);
        }
        public int GetWaitCount()
        {
            return _viewWaitList.Count(t => t.dataStatus != DataStatus.Delete);
        }
        public int GetWaitFinishCount()
        {
            return _viewWaitList.Count(t => t.dataStatus != DataStatus.Delete && t.Status == 1);
        }
    }
}
