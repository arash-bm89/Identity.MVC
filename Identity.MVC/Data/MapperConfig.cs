using AutoMapper;
using Identity.MVC.Models.DTO;
using Identity.MVC.Models;

namespace Identity.MVC.Data
{
    public class MapperConfig: Profile
    {
        public MapperConfig()
        {
            CreateMap<User, UserDTO>().ReverseMap();
        }
    }
}
