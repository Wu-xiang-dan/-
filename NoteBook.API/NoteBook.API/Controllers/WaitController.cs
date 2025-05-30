using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NoteBook.API.ApiResponse;
using NoteBook.API.DataModel;
using NoteBook.API.DTOS;
using System.Globalization;

namespace NoteBook.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class WaitController : ControllerBase
    {
        private readonly NoteBookDBContext _dbContext;
        private readonly IMapper _mapper;
        public WaitController(NoteBookDBContext noteBookDBContext, IMapper mapper)
        {
            _mapper = mapper;
            _dbContext = noteBookDBContext;
        }
        /// <summary>
        /// 待办事项统计
        /// </summary>
        /// <returns>代办事项统计结果</returns>
        [HttpGet]
        public IActionResult StateWait()
        {
            Response response = new Response();//响应要返回的数据
            try
            {
                var list = _dbContext.WaitInfos.ToList();
                var finishList = list.Where(t => t.Status == 1).ToList();
                StateWaitDTO stateWaitDTO = new StateWaitDTO() { WaitCount = list.Count, FinishCount = finishList.Count };
                response.ResultCode = Result.Success;
                response.Message = "获取成功";
                response.ResultData = stateWaitDTO;
            }
            catch (Exception)
            {
                response.ResultCode = Result.NotFound;
                response.Message = "服务器异常";
            }
            return Ok(response);
        }
        [HttpPost]
        public IActionResult AddWait(WaitDTO waitDTO)
        {
            Response response = new Response();
            try
            {         
                 WaitInfo wait = _mapper.Map<WaitInfo>(waitDTO);
                wait.AccountInfoId = 1;
                _dbContext.WaitInfos.Add(wait);
                if (_dbContext.SaveChanges() > 0)
                {
                    response.ResultCode = Result.Success;
                    response.Message = "添加成功";
                }
                else
                {
                    response.ResultCode = Result.Error;
                    response.Message = "添加失败";
                }
            }
            catch (Exception)
            {
                response.ResultCode = Result.NotFound;
                response.Message = "服务器异常";
            }
            return Ok(response);
        }

        /// <summary>
        /// 获取待办状态的事项
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetWaitings(int id)
        {
            Response response = new Response();
            try
            {
                var WaitList = from A in _dbContext.WaitInfos
                               where A.AccountInfoId == id
                               select new WaitDTO { Content = A.Content, Title = A.Title, Status = A.Status, Id = A.Waitid };
                if (WaitList==null)
                {
                    response.ResultData = null;
                    response.ResultCode = Result.Failed;
                    response.Message = "未查询到事件";
                    return Ok(response);
                }
                response.ResultData = WaitList.ToList();
                response.ResultCode = Result.Success;
                response.Message = "查询成功";
            }
            catch (Exception)
            {
                response.ResultCode = Result.NotFound;
                response.Message = "服务器丢失";
            }

            return Ok(response);

        }
        /// <summary>
        /// 更待办状态
        /// </summary>
        /// <param name="waitDTO"></param>
        /// <returns></returns>
        [HttpPut]
        public IActionResult ChangeWaitingState(WaitDTO waitDTO)
        {
            Response response = new Response();
            try
            {
                var dbinfo = _dbContext.WaitInfos.Where(t => t.Waitid == waitDTO.Id).FirstOrDefault();

                if (dbinfo == null)
                {
                    response.ResultCode = Result.Error;
                    response.Message = "没有找到该代办事项";
                    return Ok(response);
                }
                dbinfo.Status = waitDTO.Status;
                _dbContext.SaveChanges();
                response.ResultCode = Result.Success;
                response.Message = "修改成功";
            }
            catch (Exception)
            {
                response.ResultCode = Result.NotFound;
                response.Message = "服务器异常";

            }
            return Ok(response);
        }
        /// <summary>
        /// 更新编辑WaitDTO
        /// </summary>
        /// <param name="waitDTO"></param>
        /// <returns></returns>
        [HttpPut]
        public IActionResult UpDataWait(WaitDTO waitDTO)
        {
            Response response = new Response();
            try
            {
                var dbinfo = _dbContext.WaitInfos.Find(waitDTO.Id);
                if(dbinfo == null)
                {
                    response.ResultCode = Result.Failed;
                    response.Message = "未找到此条数据";
                    return Ok(response);    
                }
                dbinfo.Status = waitDTO.Status;
                dbinfo.Content = waitDTO.Content;
                dbinfo.Title = waitDTO.Title;
                _dbContext.SaveChanges();
                response.ResultCode = Result.Success;
                response.Message = "修改成功";
            }
            catch(Exception)
            {
                response.ResultCode = Result.NotFound;
                response.Message = "服务器异常";
            }
            return Ok(response);
        }
        [HttpGet]
        public IActionResult QueryWaitList(string? Title,string? Status)
        {
            Response response = new Response();
            try
            {
                //懒加载
                var query=from A in _dbContext.WaitInfos
                          select new WaitDTO { Title = A.Title,
                              Content = A.Content,
                              Status = A.Status,
                              Id = A.Waitid
                          };
                if (Title!=null)
                {
                    query=query.Where(t => t.Title.Contains(Title));
                }
                if (Status!=null)
                {
                    query = query.Where(t=>t.Status.ToString()==Status);
                }
                response.ResultCode = Result.Success;
                response.Message = "查询成功";
                response.ResultData = query.ToList();
            }
            catch (Exception)
            {

                response.ResultCode = Result.NotFound;
                response.Message = "服务器异常";
                response.ResultData = null;
            }
            return Ok(response);
        }
        [HttpDelete]
        public IActionResult DeleteWait(int id)
        {
            Response response = new Response();
            try
            {
                var del_wait = _dbContext.WaitInfos.Where(t => t.Waitid ==id).FirstOrDefault();
                if (del_wait != null)
                {
                    _dbContext.WaitInfos.Remove(del_wait);
                    _dbContext.SaveChanges();
                    response.Message = "删除成功";
                    response.ResultCode = Result.Success;
                    return Ok(response);
                }
            }
            catch (MissingMethodException ex)
            {
                Console.WriteLine($"DeleteWait 控制器方法中抛出 MissingMethodException: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                response.ResultCode = Result.Error;
                response.Message = "服务器异常";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DeleteWait 控制器方法中抛出其他异常: {ex.Message}");
                response.ResultCode = Result.Error;
                response.Message = "服务器异常";
            }
            response.Message = "未找到该数据";
            response.ResultCode = Result.NotFound;
            return Ok(response);
        }

        /// <summary>
        /// 批量上传备忘录
        /// </summary>
        /// <param name="waits"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UploadMemos(List<MemoDTO> memos)
        {
            Response response = new Response();
            List<MemoDTO> drityData = new List<MemoDTO>();
            foreach (var memo in memos)
            {
                try
                {
                    var enitymemo = _mapper.Map<MemoDTO>(memo);
                    switch (memo.status)
                    {
                        case DataStatus.Normal:
                            break;
                        case DataStatus.Delete:
                            _dbContext.MemoInfos.Remove(_dbContext.MemoInfos.Where(t => t.MemoID == memo.MemoID).FirstOrDefault());
                            break;
                        case DataStatus.Add:
                            _dbContext.Add(enitymemo);
                            break;
                        case DataStatus.Alter:
                            var dbinfo = _dbContext.MemoInfos.Where(t => t.MemoID == memo.MemoID).FirstOrDefault();
                            dbinfo.Title= memo.Title;
                            dbinfo.Content = memo.Content;
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception)
                {
                    drityData.Add(memo);

                    continue;
                }

            }
            if (drityData.Count != 0)
            {
                response.Message = $"上传失败公{drityData.Count}条数据为成功上传";
                response.ResultCode = Result.Error;
                response.ResultData = drityData;
                return Ok(response);
            }
            response.ResultCode = Result.Success;
            response.Message = "上传成功";
            response.ResultData = drityData;
            return Ok(response);
        }
    }
}
