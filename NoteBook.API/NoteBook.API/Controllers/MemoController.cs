using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;
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
        public IActionResult GetMemoList([FromQuery]int id)
        {
            Response response = new Response();
            try
            {
                var list = _dbContext.MemoInfos
                  .Where(t => t.AccountInfoId == id).
                  Select(t => new MemoDTO()
                  {
                      Title = t.Title,
                      Content = t.Content,
                      AccountInfoId = t.AccountInfoId,
                      MemoID = t.MemoID
                  }).ToList();
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
        [HttpDelete]
        public IActionResult DeleteMemos([FromQuery]List<int> ids,[FromQuery]int Account_id)
        {
            Response response = new Response();
            try
            {
                if (ids == null || ids.Count == 0)
                {
                    response.Message = "未提供要删除的ID列表";
                    response.ResultCode = Result.Failed;
                    return Ok(response);
                }

                var memosToDelete = _dbContext.MemoInfos.Where(t => ids.Contains(t.MemoID)&&t.AccountInfoId==Account_id).ToList();

                if (memosToDelete.Any())
                {
                    _dbContext.MemoInfos.RemoveRange(memosToDelete);
                    _dbContext.SaveChanges();
                    response.Message = $"成功删除 {memosToDelete.Count} 条数据";
                    response.ResultCode = Result.Success;
                    return Ok(response);
                }
                else
                {
                    response.Message = "未找到指定ID的数据";
                    response.ResultCode = Result.NotFound;
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DeleteMemos 控制器方法中抛出异常: {ex.Message}");
                response.Message = "服务器异常";
                response.ResultCode = Result.Error;
                return Ok(response);
            }
        }
        [HttpPost]
        public IActionResult AddMemos([FromBody] List<MemoDTO> memos)
        {
            Response response = new Response();
            try
            {
                if (memos == null || !memos.Any())
                {
                    response.Message = "未提供要添加的备忘录列表";
                    response.ResultCode = Result.Failed;
                    return Ok(response);
                }
                var memoEntities = _mapper.Map<List<NewMemoinfo>>(memos);
                _dbContext.MemoInfos.AddRange(memoEntities);
                _dbContext.SaveChanges();
                response.Message = $"成功添加 {memoEntities.Count} 条备忘录";
                response.ResultCode = Result.Success;
                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AddMemos 控制器方法中抛出异常: {ex.Message}");
                response.Message = "服务器异常";
                response.ResultCode = Result.Error;
                return Ok(response);
            }

        }
        [HttpPut]
        public IActionResult AlterMemos([FromBody] List<MemoDTO> memos, [FromQuery]int Account_id)
        {
            Response response = new Response();
            try
            {
                if (memos == null || !memos.Any())
                {
                    response.Message = "未提供要修改的备忘录列表";
                    response.ResultCode = Result.Failed;
                    return Ok(response);
                }
                foreach (var memo in memos)
                {
                    var existingMemo = _dbContext.MemoInfos.FirstOrDefault(t => t.MemoID == memo.MemoID&&t.AccountInfoId==Account_id);
                    if (existingMemo != null)
                    {
                        existingMemo.Title = memo.Title;
                        existingMemo.Content = memo.Content;
                        existingMemo.AccountInfoId = memo.AccountInfoId;
                    }
                }
                _dbContext.SaveChanges();
                response.Message = $"成功修改 {memos.Count} 条备忘录";
                response.ResultCode = Result.Success;
                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AlterMemos 控制器方法中抛出异常: {ex.Message}");
                response.Message = "服务器异常";
                response.ResultCode = Result.Error;
                return Ok(response);
            }
        }
    }
}
