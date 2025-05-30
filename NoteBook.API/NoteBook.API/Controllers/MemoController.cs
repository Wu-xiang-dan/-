using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NoteBook.API.ApiResponse;
using NoteBook.API.DataModel;
using NoteBook.API.DTOS;

namespace NoteBook.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MemoController : ControllerBase
    {
        private readonly NoteBookDBContext _dbContext;
        private readonly IMapper _mapper;
        public MemoController(NoteBookDBContext noteBookDBContext, IMapper mapper)
        {
            _mapper = mapper;
            _dbContext = noteBookDBContext;
        }
        /// <summary>
        ///  获取备忘录列表
        /// </summary>
        /// <param name="id">用户id</param>
        /// <returns>对应Memo列表</returns>
        [HttpGet]
        public IActionResult GetMemoList(int id)
        {
            Response response = new Response();
            try
            {
                var list = _dbContext.Accountinfos
                  .Where(t => t.AccountId == id).SelectMany(t => t.Memos).
                  Select(t => new MemoDTO()
                  {
                      Title = t.Title,
                      Content = t.Content,
                      AccountID = t.AccountInfoId,
                      MemoID = t.MemoID
                  });
                response.ResultCode = Result.Success;
                response.ResultData = list;
                response.Message = "获取成功";
                return Ok(response);
            }
            catch (Exception)
            {
                response.ResultCode = Result.NotFound;
                response.ResultData = null;
                response.Message = "服务器异常";
            }
            return Ok(response);
        }
        /// <summary>
        /// 添加备忘录列表
        /// </summary>
        /// <param name="MemoDTO">DTOJson流</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AddMemo(MemoDTO memo)
        {
            Response response = new Response();
            try
            {
                _dbContext.MemoInfos.Add(_mapper.Map<NewMemoinfo>(memo));
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

        [HttpPost]
        public IActionResult UpdateMemo(NewMemoinfo MemoInfo)
        {
            Response response = new Response();
            try
            {
                var memo = _dbContext.MemoInfos.FirstOrDefault(t => t.MemoID == MemoInfo.MemoID);
                if (memo != null)
                {
                    memo.Title = MemoInfo.Title;
                    memo.Content = MemoInfo.Content;
                }

                if (_dbContext.SaveChanges() > 0)
                {
                    response.ResultCode = Result.Success;
                    response.Message = "修改成功";
                }
                else
                {
                    response.ResultCode = Result.Error;
                    response.Message = "修改失败";
                }
            }
            catch (Exception)
            {
                response.ResultCode = Result.NotFound;
                response.Message = "服务器异常";
            }
            return Ok(response);
        }
        [HttpDelete]
        public IActionResult DeleteMemo(int id)
        {
            Response response = new Response();
            try
            {
                var del_memo = _dbContext.MemoInfos.Where(t => t.MemoID == id).FirstOrDefault();
                if (del_memo != null)
                {
                    _dbContext.MemoInfos.Remove(del_memo);
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

        [HttpPost]
        public IActionResult UploadWaits(List<WaitDTO> waits)
        {
            Response response = new Response();
            List<WaitDTO> drityData = new List<WaitDTO>();
            foreach (var wait in waits)
            {
                try
                {
                    var waitInfo = _mapper.Map<WaitInfo>(wait);
                    _dbContext.WaitInfos.Add(waitInfo);
                }
                catch (Exception)
                {
                    drityData.Add(wait);
                    continue;
                }

            }
            if (drityData.Count!=0)
            {
                response.Message = $"上传失败公{drityData.Count}条数据为成功上传";
                response.ResultCode = Result.Error;
                response.ResultData = drityData;
                return Ok(response);
            }
            response.ResultCode= Result.Success;
            response.Message = "上传成功";
            response.ResultData = drityData;
            return Ok(response);
        }
    }
}
