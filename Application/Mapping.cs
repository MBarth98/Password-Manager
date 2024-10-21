using Application.DTOs;
using AutoMapper;
using Domain;

namespace Application;

public class Mapping : Profile
{
    public Mapping()
    {
        // Auth
        CreateMap<RegisterCreate, User>()
            .ForMember(x => x.PasswordHash, opt => opt.MapFrom(src => src.Password));
        CreateMap<User, LoginResponse>();
        CreateMap<LoginRequest, User>()
            .ForMember(x => x.PasswordHash, opt => opt.MapFrom(src => src.Password));
        
        // user password
        CreateMap<PasswordCreate, Password>();
        CreateMap<PasswordUpdate, Password>();
    }
}