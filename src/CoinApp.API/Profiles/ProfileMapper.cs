using AutoMapper;
using CoinApp.API.DTOs;
using CoinApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoinApp.API.Profiles
{
    public class ProfileMapper : Profile
    {
        public ProfileMapper()
        {
            //CreateMap<UserModel, UserModelView>().ForMember(x => x.Role, opt => opt.Ignore())
            //    .ForMember(x => x.Employee, opt => opt.Ignore()).ReverseMap();
            //CreateMap<EditUserModel, UserModelView>().ReverseMap();
            CreateMap<UserDTO, User>().ReverseMap();
            CreateMap<CreateUserDTO, User>().ReverseMap();

        }
    }

}
