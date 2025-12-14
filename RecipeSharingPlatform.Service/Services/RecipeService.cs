using AutoMapper;
using RecipeSharingPlatform.Repository.Basic.Interfaces;
using RecipeSharingPlatform.Repository.Models;
using RecipeSharingPlatform.Repository.UnitOfWork.Interface;
using RecipeSharingPlatform.Service.DTOs.RequestDTOs;
using RecipeSharingPlatform.Service.DTOs.ResponseDTOs;
using RecipeSharingPlatform.Service.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeSharingPlatform.Service.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IGenericRepository<RecipeIngredient> _ingredientRepo;
        private readonly IGenericRepository<RecipeStep> _stepRepo;
        private readonly IGenericRepository<RecipeImage> _imageRepo;

        public RecipeService(IUnitOfWork unitOfWork, IMapper mapper,
        IGenericRepository<RecipeIngredient> ingredientRepo,
        IGenericRepository<RecipeStep> stepRepo,
        IGenericRepository<RecipeImage> imageRepo)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _ingredientRepo = ingredientRepo;
            _stepRepo = stepRepo;
            _imageRepo = imageRepo;
        }

        public async Task<PagedResultDTO<RecipeListItemDTO>> GetRecipesAsync(RecipeListFilterDTO filter)
        {
            var (items, totalCount) = await _unitOfWork.RecipeRepository.GetPagedRecipesAsync(
                filter.Search,
                filter.CategoryId,
                filter.SortBy,
                filter.Page <= 0 ? 1 : filter.Page,
                filter.PageSize <= 0 ? 12 : filter.PageSize);

            var dtoItems = _mapper.Map<List<RecipeListItemDTO>>(items);

            return new PagedResultDTO<RecipeListItemDTO>
            {
                Items = dtoItems,
                TotalItems = totalCount,
                Page = filter.Page,
                PageSize = filter.PageSize
            };
        }

        public async Task<List<RecipeListItemDTO>> GetUserRecipesAsync(int userId, int? take = null)
        {
            var recipes = await _unitOfWork.RecipeRepository.GetByUserAsync(userId, take);
            return _mapper.Map<List<RecipeListItemDTO>>(recipes);
        }

        public async Task<RecipeDetailDTO?> GetRecipeDetailAsync(int recipeId, int? currentUserId)
        {
            var recipe = await _unitOfWork.RecipeRepository.GetDetailByIdAsync(recipeId);
            if (recipe == null)
            {
                return null;
            }

            // Chỉ cho xem nếu public hoặc là chủ sở hữu
            if (recipe.IsPublic != true && (!currentUserId.HasValue || recipe.UserId != currentUserId.Value))
            {
                return null;
            }

            // Tăng view count
            recipe.ViewCount = (recipe.ViewCount ?? 0) + 1;
            _unitOfWork.RecipeRepository.Update(recipe);
            await _unitOfWork.SaveChangesAsync();

            var dto = _mapper.Map<RecipeDetailDTO>(recipe);
            return dto;
        }

        public async Task<int> CreateRecipeAsync(CreateRecipeRequestDTO request)
        {
            var recipe = new Recipe
            {
                UserId = request.UserId,
                CategoryId = request.CategoryId,
                Title = request.Title,
                Description = request.Description,
                PrepTime = request.PrepTime,
                CookTime = request.CookTime,
                Servings = request.Servings,
                Difficulty = request.Difficulty,
                IsPublic = request.IsPublic,
                ViewCount = 0,
                CreatedAt = DateTime.UtcNow
            };

            if (request.ImageUrls != null && request.ImageUrls.Count > 0)
            {
                int order = 0;
                foreach (var url in request.ImageUrls)
                {
                    recipe.RecipeImages.Add(new RecipeImage
                    {
                        ImageUrl = url,
                        ImageOrder = order++,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            if (request.Ingredients != null && request.Ingredients.Count > 0)
            {
                int idx = 0;
                foreach (var ing in request.Ingredients)
                {
                    recipe.RecipeIngredients.Add(new RecipeIngredient
                    {
                        IngredientName = ing.IngredientName,
                        Quantity = ing.Quantity,
                        Unit = ing.Unit,
                        OrderIndex = ing.OrderIndex, // hoặc idx++ nếu bạn muốn đánh lại
                      
                    });
                    idx++;
                }
            }

            if (request.Steps != null && request.Steps.Count > 0)
            {
                foreach (var step in request.Steps)
                {
                    recipe.RecipeSteps.Add(new RecipeStep
                    {
                        StepNumber = step.StepNumber,
                        StepDescription = step.StepDescription,
                        ImageUrl = step.ImageUrl,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            // CHỈ dùng RecipeRepository, không dùng GenericRepository
            await _unitOfWork.RecipeRepository.CreateAsync(recipe);

            // Tên hàm save có thể khác, nếu bạn đang dùng SaveAsync / CommitAsync thì đổi cho đúng
            await _unitOfWork.SaveChangesAsync();

            return recipe.RecipeId;
        }

        public async Task<UpdateRecipeDTO?> GetRecipeForEditAsync(int recipeId, int currentUserId)
        {
            var recipe = await _unitOfWork.RecipeRepository.GetByIdAsync(recipeId);
            if (recipe == null)
            {
                return null;
            }

            if (recipe.UserId != currentUserId)
            {
                // không phải owner -> không cho edit
                return null;
            }

            var dto = new UpdateRecipeDTO
            {
                RecipeId = recipe.RecipeId,
                UserId = recipe.UserId,
                CategoryId = recipe.CategoryId,
                Title = recipe.Title,
                Description = recipe.Description,
                PrepTime = recipe.PrepTime,
                CookTime = recipe.CookTime,
                Servings = recipe.Servings,
                Difficulty = recipe.Difficulty,
                IsPublic = recipe.IsPublic ?? true
            };

            // Ingredients
            var allIngredients = await _ingredientRepo.GetAllAsync();
            dto.Ingredients = allIngredients
                .Where(i => i.RecipeId == recipe.RecipeId)
                .OrderBy(i => i.OrderIndex)
                .Select(i => new UpdateRecipeIngredientDTO
                {
                    IngredientName = i.IngredientName,
                    Quantity = i.Quantity,
                    Unit = i.Unit
                })
                .ToList();

            // Steps
            var allSteps = await _stepRepo.GetAllAsync();
            dto.Steps = allSteps
                .Where(s => s.RecipeId == recipe.RecipeId)
                .OrderBy(s => s.StepNumber)
                .Select(s => new UpdateRecipeStepDTO
                {
                    StepNumber = s.StepNumber,
                    StepDescription = s.StepDescription
                })
                .ToList();

            // Images
            var allImages = await _imageRepo.GetAllAsync();
            dto.ImageUrls = allImages
                .Where(img => img.RecipeId == recipe.RecipeId)
                .OrderBy(img => img.ImageOrder)
                .Select(img => img.ImageUrl)
                .ToList();

            return dto;
        }


        public async Task<bool> UpdateRecipeAsync(UpdateRecipeDTO request)
        {
            var recipe = await _unitOfWork.RecipeRepository.GetByIdAsync(request.RecipeId);
            if (recipe == null)
            {
                return false;
            }

            // chỉ owner mới được sửa
            if (recipe.UserId != request.UserId)
            {
                return false;
            }

            // ====== cập nhật thông tin cơ bản ======
            recipe.Title = request.Title;
            recipe.Description = request.Description;
            recipe.CategoryId = request.CategoryId;
            recipe.PrepTime = request.PrepTime;
            recipe.CookTime = request.CookTime;
            recipe.Servings = request.Servings;
            recipe.Difficulty = request.Difficulty;
            recipe.IsPublic = request.IsPublic;
            recipe.UpdatedAt = DateTime.UtcNow;

            request.Ingredients ??= new List<UpdateRecipeIngredientDTO>();
            request.Steps ??= new List<UpdateRecipeStepDTO>();

            // ====== INGREDIENTS: xoá hết cũ, thêm lại mới ======
            var allIngredients = await _ingredientRepo.GetAllAsync();
            var ingredientsToRemove = allIngredients
                .Where(i => i.RecipeId == recipe.RecipeId)
                .ToList();

            foreach (var ing in ingredientsToRemove)
            {
                _ingredientRepo.Remove(ing);
            }

            int orderIndex = 0;
            foreach (var ingDto in request.Ingredients)
            {
                if (string.IsNullOrWhiteSpace(ingDto.IngredientName))
                    continue;   // bỏ dòng trống

                var newIng = new RecipeIngredient
                {
                    RecipeId = recipe.RecipeId,
                    IngredientName = ingDto.IngredientName.Trim(),
                    Quantity = ingDto.Quantity,
                    Unit = ingDto.Unit?.Trim() ?? string.Empty,
                    OrderIndex = orderIndex++
                };

                await _ingredientRepo.CreateAsync(newIng);
            }

            // ====== STEPS: xoá hết cũ, thêm lại mới ======
            var allSteps = await _stepRepo.GetAllAsync();
            var stepsToRemove = allSteps
                .Where(s => s.RecipeId == recipe.RecipeId)
                .ToList();

            foreach (var step in stepsToRemove)
            {
                _stepRepo.Remove(step);
            }

            int stepNumber = 1;
            foreach (var stepDto in request.Steps)
            {
                if (string.IsNullOrWhiteSpace(stepDto.StepDescription))
                    continue;

                var newStep = new RecipeStep
                {
                    RecipeId = recipe.RecipeId,
                    StepNumber = stepNumber++,
                    StepDescription = stepDto.StepDescription.Trim()
                };

                await _stepRepo.CreateAsync(newStep);
            }

            // ====== IMAGES: xoá hết cũ, thêm lại mới theo list ImageUrls ======
            var allImages = await _imageRepo.GetAllAsync();
            var imagesToRemove = allImages
                .Where(i => i.RecipeId == recipe.RecipeId)
                .ToList();

            foreach (var img in imagesToRemove)
            {
                _imageRepo.Remove(img);
            }

            if (request.ImageUrls != null && request.ImageUrls.Any())
            {
                int imageOrder = 0;
                foreach (var url in request.ImageUrls.Where(u => !string.IsNullOrWhiteSpace(u)))
                {
                    var newImg = new RecipeImage
                    {
                        RecipeId = recipe.RecipeId,
                        ImageUrl = url,
                        ImageOrder = imageOrder++,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _imageRepo.CreateAsync(newImg);
                }
            }

         

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteRecipeAsync(int recipeId, int currentUserId)
        {
            var recipe = await _unitOfWork.RecipeRepository.GetByIdAsync(recipeId);
            if (recipe == null)
            {
                return false;
            }

            // chỉ owner được xoá
            if (recipe.UserId != currentUserId)
            {
                return false;
            }

            // Xoá child record (nếu DB chưa cấu hình cascade)
            var allIngredients = await _ingredientRepo.GetAllAsync();
            foreach (var ing in allIngredients.Where(i => i.RecipeId == recipeId))
            {
                _ingredientRepo.Remove(ing);
            }

            var allSteps = await _stepRepo.GetAllAsync();
            foreach (var step in allSteps.Where(s => s.RecipeId == recipeId))
            {
                _stepRepo.Remove(step);
            }

            var allImages = await _imageRepo.GetAllAsync();
            foreach (var img in allImages.Where(i => i.RecipeId == recipeId))
            {
                _imageRepo.Remove(img);
            }

            
            _unitOfWork.RecipeRepository.Remove(recipe);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
        public async Task<bool> IncreaseViewCountAsync(int recipeId)
        {
            var recipe = await _unitOfWork.RecipeRepository.GetByIdAsync(recipeId);
            if (recipe == null)
            {
                return false;
            }

            // giả định property tên ViewCount (int?)
            recipe.ViewCount = (recipe.ViewCount ?? 0) + 1;

            _unitOfWork.RecipeRepository.Update(recipe);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
