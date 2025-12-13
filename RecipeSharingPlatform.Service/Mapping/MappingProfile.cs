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

            CreateMap<User, LoginResponseDTO>().ReverseMap();

            // Category
            CreateMap<Category, CategoryResponseDTO>();


            // Recipe -> RecipeListItemDTO
            CreateMap<Recipe, RecipeListItemDTO>()
               .ForMember(dest => dest.RecipeId, opt => opt.MapFrom(src => src.RecipeId))
               .ForMember(dest => dest.AuthorName, opt =>
                   opt.MapFrom(src => string.IsNullOrEmpty(src.User.FullName)
                       ? src.User.Username
                       : src.User.FullName))
               .ForMember(dest => dest.AuthorAvatar, opt => opt.MapFrom(src => src.User.ProfileImage))
               .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.CategoryName : null))
               .ForMember(dest => dest.LikesCount, opt => opt.MapFrom(src => src.RecipeLikes.Count))
               .ForMember(dest => dest.MainImageUrl, opt =>
                   opt.MapFrom(src =>
                       src.RecipeImages
                           .OrderBy(i => i.ImageOrder ?? 0)
                           .ThenBy(i => i.CreatedAt)
                           .Select(i => i.ImageUrl)
                           .FirstOrDefault()));

            // Details
            CreateMap<RecipeImage, RecipeImageDTO>();
            CreateMap<RecipeIngredient, RecipeIngredientDTO>();
            CreateMap<RecipeStep, RecipeStepDTO>();

            CreateMap<Recipe, RecipeDetailDTO>()
                .ForMember(dest => dest.AuthorName, opt =>
                    opt.MapFrom(src => string.IsNullOrEmpty(src.User.FullName)
                        ? src.User.Username
                        : src.User.FullName))
                .ForMember(dest => dest.AuthorAvatar, opt => opt.MapFrom(src => src.User.ProfileImage))
                .ForMember(dest => dest.CategoryName, opt =>
                    opt.MapFrom(src => src.Category != null ? src.Category.CategoryName : null))
                .ForMember(dest => dest.ViewCount, opt => opt.MapFrom(src => src.ViewCount ?? 0))
                .ForMember(dest => dest.LikesCount, opt => opt.MapFrom(src => src.RecipeLikes.Count))
                .ForMember(dest => dest.Images, opt =>
                    opt.MapFrom(src => src.RecipeImages
                        .OrderBy(i => i.ImageOrder ?? 0)
                        .ThenBy(i => i.CreatedAt)))
                .ForMember(dest => dest.Ingredients, opt =>
                    opt.MapFrom(src => src.RecipeIngredients
                        .OrderBy(i => i.OrderIndex ?? 0)
                        .ThenBy(i => i.IngredientId)))
                .ForMember(dest => dest.Steps, opt =>
                    opt.MapFrom(src => src.RecipeSteps
                        .OrderBy(s => s.StepNumber)))
                .ForMember(d => d.Comments, opt =>
                    opt.MapFrom(s => s.Comments
                        .OrderByDescending(c => c.CreatedAt)));


            CreateMap<Comment, RecipeCommentDTO>()
            .ForMember(d => d.AuthorName, opt =>
                opt.MapFrom(s => string.IsNullOrEmpty(s.User.FullName) ? s.User.Username : s.User.FullName))
            .ForMember(d => d.AuthorAvatar, opt => opt.MapFrom(s => s.User.ProfileImage));

            CreateMap<Category, CategoryResponseDTO>();


        }


    }
}
