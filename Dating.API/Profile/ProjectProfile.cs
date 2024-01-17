using AutoMapper;
using Model.DTO;
using Model.Enitities;

namespace Dating.API.Profiles
{
    public class ProjectProfile : Profile
    {
        public ProjectProfile()
        {
            CreateMap<ApplicationUser, DisplayFindUserDTO>().ReverseMap();
            CreateMap<ApplicationUser, DisplayUserWithRoleDto>().ReverseMap();
            CreateMap<ApplicationUser, UserTimeAvailableDto>().ReverseMap();
            CreateMap<Timers, AddTimeDto>().ReverseMap();
            CreateMap<ApplicationUser, SignUp>()
                .ForMember(dest => dest.ConfirmPassword, opt => opt.Ignore())
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<ApplicationUser, UpdateUserDto>().ReverseMap();
            CreateMap<CallRecord, AddCallRecord>().ReverseMap();
        }
    }
}