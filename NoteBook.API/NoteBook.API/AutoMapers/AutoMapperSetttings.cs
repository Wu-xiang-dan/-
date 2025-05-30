using AutoMapper;
using NoteBook.API.DataModel;
using System.Runtime;
using NoteBook.API.DTOS;
namespace NoteBook.API.AutoMapers
{
    public class AutoMapperSetttings:Profile
    {
        public AutoMapperSetttings()
        {
            //DTO 转实体类
            CreateMap<Accountinfo, AccountInfoDTO>().ReverseMap();
            CreateMap<WaitDTO,WaitInfo>().ReverseMap();
            CreateMap<MemoDTO, NewMemoinfo>().ReverseMap();
        }
    }
}
