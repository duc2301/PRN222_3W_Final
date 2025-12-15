using AutoMapper;
using RecipeSharingPlatform.Repository.Models;
using RecipeSharingPlatform.Repository.Repositories.Interfaces;
using RecipeSharingPlatform.Service.DTOs.ResponseDTOs;
using RecipeSharingPlatform.Service.Services.Interfaces;

namespace RecipeSharingPlatform.Service.Services
{
    public class SavedRecipeService : ISavedRecipeService
    {
        private readonly ISavedRecipeRepository _savedRecipeRepository;
        private readonly IMapper _mapper; // Dùng AutoMapper để map sang DTO cho nhanh

        public SavedRecipeService(ISavedRecipeRepository savedRecipeRepository, IMapper mapper)
        {
            _savedRecipeRepository = savedRecipeRepository;
            _mapper = mapper;
        }

        public async Task<bool> ToggleSaveRecipeAsync(int recipeId, int userId)
        {
            var savedRecipe = await _savedRecipeRepository.GetSavedRecipeAsync(recipeId, userId);
            bool isSaved;

            if (savedRecipe != null)
            {
                // Đã lưu -> Xóa (Unsave)
                await _savedRecipeRepository.RemoveAsync(savedRecipe);
                isSaved = false;
            }
            else
            {
                // Chưa lưu -> Thêm (Save)
                var newSave = new SavedRecipe
                {
                    RecipeId = recipeId,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };
                await _savedRecipeRepository.AddAsync(newSave);
                isSaved = true;
            }

            await _savedRecipeRepository.SaveChangesAsync();
            return isSaved;
        }

        public async Task<bool> IsRecipeSavedAsync(int recipeId, int userId)
        {
            var savedRecipe = await _savedRecipeRepository.GetSavedRecipeAsync(recipeId, userId);
            return savedRecipe != null;
        }

        public async Task<List<RecipeListItemDTO>> GetSavedRecipesAsync(int userId)
        {
            var savedRecipes = await _savedRecipeRepository.GetSavedRecipesByUserIdAsync(userId);

            // Lấy list Recipe từ SavedRecipe và map sang DTO
            var recipes = savedRecipes.Select(sr => sr.Recipe).ToList();

            return _mapper.Map<List<RecipeListItemDTO>>(recipes);
        }
    }
}