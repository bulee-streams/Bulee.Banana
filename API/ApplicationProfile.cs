using API.Models;
using API.Models.ViewModels;
using AutoMapper;

namespace API
{
    public class ApplicationProfile : Profile
    {
        public ApplicationProfile()
        {
            CreateMap<RegiserViewModel, User>();
        }
    }
}
