using AutoMapper;
using RecipeSharingPlatform.Repository.Models;
using RecipeSharingPlatform.Service.DTOs.RequestDTOs;
using RecipeSharingPlatform.Service.DTOs.ResponseDTOs;

namespace RecipeSharingPlatform.Service.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User mappings
            CreateMap<User, UserResponseDTO>().ReverseMap();
            CreateMap<CreateUserDTO, User>().ReverseMap();
            CreateMap<UpdateUserDTO, User>().ReverseMap();

            CreateMap<CreateUserDTO, UserResponseDTO>().ReverseMap();
            CreateMap<UpdateUserDTO, UserResponseDTO>().ReverseMap();
        }
    }
}
