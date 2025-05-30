using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NoteBook.API.DataModel;
using NoteBook.API.DTOS;
using NoteBook.API.ApiResponse;
using NoteBook.API.AutoMapers;
using AutoMapper;
namespace NoteBook.API.Controllers
{
    [Route("api/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly NoteBookDBContext _dbContext;
        private readonly IMapper _mapper;
        public AccountController(NoteBookDBContext db,IMapper mapper)
        {
            _dbContext=db;
            _mapper = mapper;
        }
        [HttpPost]
        public IActionResult Register(AccountInfoDTO accountInfoDTO)
        {
            Response response = new Response();//响应要返回的数据
            try
            {
               var dbAccount=_dbContext.Accountinfos.Where((Accountinfo d) => d.Account == accountInfoDTO.Account).FirstOrDefault();
                if (dbAccount != null)
                {
                    response.ResultCode = Result.Error;
                    response.Message = "账号已经被注册";
                    return Ok(response);
                }
                Accountinfo accountInfo = _mapper.Map<Accountinfo>(accountInfoDTO);
                _dbContext.Accountinfos.Add(accountInfo);
                if (_dbContext.SaveChanges() > 0)
                {
                    response.ResultCode = Result.Success;
                    response.Message = "注册成功";
                }
                else
                {
                    response.ResultCode = Result.Error;
                    response.Message = "注册失败";
                }
            }
            catch (Exception)
            { 
                response.ResultCode = Result.Error;
                response.Message = "服务器异常";
            }
            return  Ok(response);
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Login(string username, string password) { 
            Response response=new Response() { };
            try
            {
                var abAccount = _dbContext.Accountinfos.Where((Accountinfo ac) => ac.Account == username && ac.Password == password).FirstOrDefault();
                
                if (abAccount == null)
                {
                    response.ResultCode = Result.Failed;
                    response.Message = "账号或密码错误";
                }
                else
                {
                    response.ResultCode = Result.Success;
                    response.ResultData = new LoginResultDTO() {AccountName=abAccount.AccountName,AccountId=abAccount.AccountId};
                    response.Message = "登录成功";
                }
            }
            catch (Exception)
            {
                response.ResultCode = Result.Error;
                response.Message = "服务器异常";
            }
            return Ok(response);
        }
    }
}
