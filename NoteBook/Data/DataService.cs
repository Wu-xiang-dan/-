using Newtonsoft.Json;
using NoteBook.DTOS;
using NoteBook.HttpClients;
using NoteBook.ViewModels;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using NoteBook.Models;
using System.Threading;
using System.Web;
using System.Windows.Xps.Serialization;
using System.Diagnostics;
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
        /// 设置用户id
        /// </summary>
        /// <param name="id"></param>
        public void SetID(int id)
        {
            this.id = id;
        }
        public int GetID()
        {
            return id;
        }
        /// <summary>
        /// 载入 数据
        /// </summary>
        /// <summary>
        /// 载入数据
        /// </summary>
        public async Task<string> LoadDataAsync()
        {
            var tasks = new List<(Task<ApiResponse> task, string name)> { (GetMemoListAsync(id), "备忘录列表"), (GetWaitListAsync(id), "等待列表") };
            try
            {
                await Task.WhenAll(tasks.Select(t => t.task));
                var islocaldata = tasks.Find(t => t.task.Result.ResultCode != Result.Success);
                if (islocaldata.task==null)
                {
                    return "载入云端数据成功";
                }
            }
            catch
            {
                var errorMessages = new List<string>();

                foreach (var (task, name) in tasks)
                {
                    if (task.IsFaulted)
                    {
                        // 获取异常信息（包含内部异常）
                        var ex = task.Exception?.Flatten().InnerExceptions.FirstOrDefault();
                        errorMessages.Add($"{name}加载失败: {ex?.Message ?? "未知错误"}");
                    }
                }
                if (errorMessages.Any())
                {
                    string fullErrorMessage = string.Join(Environment.NewLine, errorMessages);
                    return $"数据加载错误:{Environment.NewLine}{fullErrorMessage}";
                }
            }
            return "Not Connection Loading Local Data";
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

        public async Task<List<ApiResponse>> UploadWaitsAsync()
        {
            // 筛选需要操作的数据
            var deleteWaitIds = ViewWaitList
                .Where(t => t.dataStatus == DataStatus.Delete)
                .Select(t => t.Id)
                .ToList();

            var addWaits = ViewWaitList
                .Where(t => t.dataStatus == DataStatus.Add)
                .Select(t => WaitVieModel.ConvertToWaitDTO(t))
                .ToList();

            var alterWaits = ViewWaitList
                .Where(t => t.dataStatus == DataStatus.Alter)
                .Select(t => WaitVieModel.ConvertToWaitDTO(t))
                .ToList();

            // 存储所有任务及其描述
            var tasks = new List<(Task<ApiResponse> Task, string Operation)>();

            // 删除待办事项
            if (deleteWaitIds.Any())
            {
                tasks.Add(CreateDeleteWaitsTask(deleteWaitIds));
            }

            // 添加待办事项
            if (addWaits.Any())
            {
                tasks.Add(CreateAddWaitsTask(addWaits));
            }

            // 修改待办事项
            if (alterWaits.Any())
            {
                tasks.Add(CreateAlterWaitsTask(alterWaits));
            }

            return await ExecuteUploadTasks(tasks);
        }

        public async Task<List<ApiResponse>> UploadMemosAsync()
        {
            // 筛选需要操作的数据
            var deleteMemoIds = ViewMemoList
                .Where(t => t.dataStatus == DataStatus.Delete)
                .Select(t => t.MemoID)
                .ToList();

            var addMemos = ViewMemoList
                .Where(t => t.dataStatus == DataStatus.Add)
                .Select(MemoViewModel.ConvertToMemoDTO)
                .ToList();

            var alterMemos = ViewMemoList
                .Where(t => t.dataStatus == DataStatus.Alter)
                .Select(MemoViewModel.ConvertToMemoDTO)
                .ToList();

            // 存储所有任务及其描述
            var tasks = new List<(Task<ApiResponse> Task, string Operation)>();

            // 删除备忘录
            if (deleteMemoIds.Any())
            {
                tasks.Add(CreateDeleteMemoTask(deleteMemoIds));
            }

            // 添加备忘录
            if (addMemos.Any())
            {
                tasks.Add(CreateAddMemoTask(addMemos));
            }

            // 修改备忘录
            if (alterMemos.Any())
            {
                tasks.Add(CreateAlterMemoTask(alterMemos));
            }

            return await ExecuteUploadTasks(tasks);
        }

        private (Task<ApiResponse>, string) CreateDeleteMemoTask(List<int> memoIds)
        {
            using var cts = new CancellationTokenSource(10000);

            // 手动构建查询字符串
            var queryParams = string.Join("&",
                memoIds.Select(id => $"ids={id}")) +
                $"&Account_id={id}";

            return (
                _httpRestClient.ExecuteAsync(
                    new ApiRequest
                    {
                        Method = Method.DELETE,
                        Route = $"Memo/DeleteMemos?{queryParams}", // 直接将参数拼接到路由中
                                                                   // 移除Paramters，因为参数已包含在URL中
                    },
                    cts.Token
                ),
                "DeleteMemos"
            );
        }

        private (Task<ApiResponse>, string) CreateAddMemoTask(List<MemoDTO> memos)
        {
            
            using var cts = new CancellationTokenSource(10000);
            return (
                _httpRestClient.ExecuteAsync(
                    new ApiRequest
                    {
                        Method = Method.POST,
                        Route = "Memo/AddMemos",
                        Paramters = memos
                    },
                    cts.Token
                ),
                "AddMemos"
            );
        }

        private (Task<ApiResponse>, string) CreateAlterMemoTask(List<MemoDTO> memos)
        {
            using var cts = new CancellationTokenSource(10000);

            return (
                _httpRestClient.ExecuteAsync(
                    new ApiRequest
                    {
                        Method = Method.PUT,
                        Route = $"Memo/AlterMemos?Account_id={id}", 
                        Paramters = memos,
                        ContentType = "application/json"
                    },
                    cts.Token
                ),
                "AlterMemos"
            );
        }

        private (Task<ApiResponse>, string) CreateDeleteWaitsTask(List<int> waitIds)
        {
            using var cts = new CancellationTokenSource(10000);

            // 手动构建查询字符串
            var queryParams = string.Join("&",
                waitIds.Select(id => $"ids={id}")) +
                $"&Account_id={id}";

            return (
                _httpRestClient.ExecuteAsync(
                    new ApiRequest
                    {
                        Method = Method.DELETE,
                        Route = $"Wait/DeleteWaits?{queryParams}", 
                                                                  
                    },
                    cts.Token
                ),
                "DeleteWaits"
            );
        }

        private (Task<ApiResponse>, string) CreateAddWaitsTask(List<WaitDTO> waits)
        {
            using var cts = new CancellationTokenSource(10000);

            return (
                _httpRestClient.ExecuteAsync(
                    new ApiRequest
                    {
                        Method = Method.POST,
                        Route = "Wait/AddWaits",
                        Paramters = waits,
                        ContentType = "application/json"
                    },
                    cts.Token
                ),
                "AddWaits"
            );
        }

        private (Task<ApiResponse>, string) CreateAlterWaitsTask(List<WaitDTO> waits)
        {
            using var cts = new CancellationTokenSource(10000);

            return (
                _httpRestClient.ExecuteAsync(
                    new ApiRequest
                    {
                        Method = Method.PUT,
                        Route = $"Wait/AlterWaits?Account_id={id}", 
                        Paramters = waits, 
                        ContentType = "application/json"
                    },
                    cts.Token
                ),
                "AlterWaits"
            );
        }

        private async Task<List<ApiResponse>> ExecuteUploadTasks(List<(Task<ApiResponse> Task, string Operation)> tasks)
        {
            var apiResponses = new List<ApiResponse>();

            if (!tasks.Any())
            {
                apiResponses.Add(new ApiResponse
                {
                    ResultCode = Result.Success,
                    Message = "没有需要上传的数据"
                });
                return apiResponses;
            }

            try
            {
                var responses = await Task.WhenAll(tasks.Select(t => t.Task));
                apiResponses.AddRange(responses);

                if (responses.All(t => t.ResultCode == Result.Success)){

                    ViewWaitList = new ObservableCollection<WaitVieModel>(
                        ViewWaitList.Where(t => t.dataStatus != DataStatus.Delete).ToList()
                    );

                    ViewWaitList.Where(t => t.dataStatus == DataStatus.Add)
                        .ToList().ForEach(t => t.dataStatus = DataStatus.Normal);

                    ViewWaitList.Where(t => t.dataStatus == DataStatus.Alter)
                        .ToList().ForEach(t => t.dataStatus = DataStatus.Normal);
                }
            }
            catch (ArgumentException ex) when (ex.Message.Contains("Paramters"))
            {
                apiResponses.Add(new ApiResponse
                {
                    ResultCode = Result.Error,
                    Message = "请求参数格式错误"
                });
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("操作已取消");
                apiResponses.Add(new ApiResponse
                {
                    ResultCode = Result.Canceled,
                    Message = "请求超时或已取消"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"上传过程中发生错误: {ex.Message}");
                apiResponses.Add(new ApiResponse
                {
                    ResultCode = Result.Error,
                    Message = "上传过程中发生错误"
                });
            }

            return apiResponses;
        }

        private async Task<ApiResponse> GetMemoListAsync(int accountId)
        {
            try
            {
                using var cts = new CancellationTokenSource(10000);

                var response = await _httpRestClient.ExecuteAsync(
                    new ApiRequest
                    {
                        Method = Method.GET,
                        Route = "Memo/GetMemoList",
                        Paramters =new Dictionary<string, object>() { { "id",accountId} } 
                    },
                    cts.Token
                );

                if (response.ResultCode == Result.Success)
                {
                    _memoList = JsonConvert.DeserializeObject<List<MemoViewModel>>(response.ResultData?.ToString() ?? string.Empty);
                    ViewMemoList = new ObservableCollection<MemoViewModel>(_memoList?.Select(t => t.Clone()).ToList() ?? new List<MemoViewModel>());
                    return response;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"获取备忘录列表失败: {ex.Message}");
            }

            // 回退到本地数据
            _memoList = await _jsonSerializerService.LoadingMemosJsonAsync();
            ViewMemoList = new ObservableCollection<MemoViewModel>(_memoList?.Select(t => t.Clone()).ToList() ?? new List<MemoViewModel>());

            return new ApiResponse
            {
                ResultCode = Result.NotFound,
                Message = "未找到服务器，启用本地数据"
            };
        }

        private async Task<ApiResponse> GetWaitListAsync(int accountId)
        {
            try
            {
                using var cts = new CancellationTokenSource(10000);

                var response = await _httpRestClient.ExecuteAsync(
                    new ApiRequest
                    {
                        Method = Method.GET,
                        Route = "Wait/GetWaitings",
                        Paramters =new Dictionary<string, object>() { { "id",accountId} }
                    },
                    cts.Token
                );

                if (response.ResultCode == Result.Success)
                {
                    _waitList = JsonConvert.DeserializeObject<List<WaitVieModel>>(response.ResultData?.ToString() ?? string.Empty);
                    ViewWaitList = new ObservableCollection<WaitVieModel>(_waitList?.Select(t => t.Clone()).ToList() ?? new List<WaitVieModel>());
                    return response;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取待办事项列表失败: {ex.Message}");
            }

            // 回退到本地数据
            _waitList = await _jsonSerializerService.LoadingWaitsJsonAsync();
            ViewWaitList = new ObservableCollection<WaitVieModel>(_waitList?.Select(t => t.Clone()).ToList() ?? new List<WaitVieModel>());

            return new ApiResponse
            {
                ResultCode = Result.NotFound,
                Message = "未找到服务器，启用本地数据"
            };
        }
    }
}
