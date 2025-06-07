using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NoteBook.API.ApiResponse;
using NoteBook.API.DataModel;
using NoteBook.API.DTOS;
using System.Collections.Immutable;
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
        [HttpGet]
        public IActionResult GetWaitings([FromQuery] int id)
        {
            Response response = new Response();
            try
            {
                var WaitList = from A in _dbContext.WaitInfos
                               where A.AccountInfoId == id
                               select new WaitDTO { Content = A.Content, Title = A.Title, Status = A.Status, Id = A.Waitid };
                if (WaitList == null)
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
        [HttpDelete]
        public IActionResult DeleteWaits([FromQuery] List<int> ids,[FromQuery] int Account_id)
        {
            Response response = new Response();
            try
            {
                var del_waits = _dbContext.WaitInfos.Where(t => ids.Contains(t.Waitid)&&t.AccountInfoId==Account_id).ToList();
                if (del_waits.Count > 0)
                {
                    _dbContext.WaitInfos.RemoveRange(del_waits);
                    _dbContext.SaveChanges();
                    response.Message = "删除成功";
                    response.ResultCode = Result.Success;
                    return Ok(response);
                }
                else
                {
                    response.Message = "未找到待办事项";
                    response.ResultCode = Result.NotFound;
                }
            }
            catch (Exception)
            {
                response.ResultCode = Result.Error;
                response.Message = "服务器异常";
            }
            return Ok(response);
        }
        [HttpPost]
        public IActionResult AddWaits([FromBody] List<WaitDTO> waits)
        {
            Response response = new Response();
            try
            {
                List<WaitInfo> waitInfos = _mapper.Map<List<WaitInfo>>(waits);
                foreach (var wait in waitInfos)
                {
                    _dbContext.WaitInfos.Add(wait);
                }
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
        [HttpPut]
        public IActionResult AlterWaits([FromBody] List<WaitDTO> waits, [FromQuery] int Account_id)
        {
            Response response = new Response();
            try
            {
                foreach (var wait in waits)
                {
                    var dbinfo = _dbContext.WaitInfos.FirstOrDefault(t => t.Waitid == wait.Id && t.AccountInfoId == Account_id);
                    if (dbinfo != null)
                    {
                        dbinfo.Title = wait.Title;
                        dbinfo.Content = wait.Content;
                        dbinfo.Status = wait.Status;
                    }
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
    }
}
