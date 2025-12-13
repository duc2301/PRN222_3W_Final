using AutoMapper;
using RecipeSharingPlatform.Repository.Repositories.Interfaces;
using RecipeSharingPlatform.Repository.UnitOfWork.Interface;
using RecipeSharingPlatform.Service.DTOs.ResponseDTOs;
using RecipeSharingPlatform.Service.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeSharingPlatform.Service.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepo;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepo, IMapper mapper)
        {
            _categoryRepo = categoryRepo;
            _mapper = mapper;
        }

        public async Task<List<CategoryResponseDTO>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepo.GetAllAsync();
            return _mapper.Map<List<CategoryResponseDTO>>(categories);
        }
    }
}
