using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using RecipeSharingPlatform.Repository.DbContexts;
using RecipeSharingPlatform.Repository.Models;
using RecipeSharingPlatform.Service.DTOs.RequestDTOs;
using RecipeSharingPlatform.Service.DTOs.ResponseDTOs;
using RecipeSharingPlatform.Service.Services.Interfaces;

namespace RecipeSharingPlatform.Service.Services
{
    public class ShoppingListService : IShoppingListService
    {
        private readonly RecipeSharingDbContext _context;
        private const string DefaultCategory = "Khác";

        public ShoppingListService(RecipeSharingDbContext context)
        {
            _context = context;
        }

        public async Task<ShoppingListDto> GetOrCreateAsync(string userId)
        {
            var uid = ParseUserId(userId);
            var list = await GetOrCreateListInternalAsync(uid, includeItems: true);
            return MapToDto(list);
        }

        public async Task AddRecipeToListAsync(string userId, int recipeId, int targetServings)
        {
            var uid = ParseUserId(userId);
            if (targetServings <= 0)
            {
                throw new ArgumentException("targetServings must be greater than zero.");
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            var recipe = await _context.Recipes
                .Include(r => r.RecipeIngredients)
                .FirstOrDefaultAsync(r => r.RecipeId == recipeId);

            if (recipe == null)
            {
                throw new InvalidOperationException("Recipe not found.");
            }

            var list = await GetOrCreateListInternalAsync(uid, includeItems: true);

            var baseServings = recipe.Servings.GetValueOrDefault(targetServings);
            if (baseServings <= 0)
            {
                baseServings = targetServings;
            }

            var ratio = (decimal)targetServings / baseServings;

            foreach (var ingredient in recipe.RecipeIngredients)
            {
                var scaledQty = decimal.Round(ingredient.Quantity * ratio, 2, MidpointRounding.AwayFromZero);
                if (scaledQty <= 0)
                {
                    continue;
                }

                var normalizedName = Normalize(ingredient.IngredientName);
                var normalizedUnit = Normalize(ingredient.Unit);
                var normalizedCategory = NormalizeCategory(null);

                var existing = list.ShoppingListItems.FirstOrDefault(i =>
                    string.Equals(i.IngredientName, normalizedName, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(i.Unit, normalizedUnit, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(NormalizeCategory(i.Category), normalizedCategory, StringComparison.OrdinalIgnoreCase));

                if (existing != null)
                {
                    existing.Quantity += scaledQty;
                }
                else
                {
                    list.ShoppingListItems.Add(new ShoppingListItem
                    {
                        IngredientName = normalizedName,
                        Quantity = scaledQty,
                        Unit = normalizedUnit,
                        Category = normalizedCategory,
                        ShoppingListId = list.ShoppingListId,
                        RecipeId = recipeId,
                        IsChecked = false,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            list.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }

        public async Task<ShoppingListItemDto> AddManualItemAsync(string userId, AddItemDto dto)
        {
            var uid = ParseUserId(userId);
            ValidateManualItem(dto);

            var list = await GetOrCreateListInternalAsync(uid, includeItems: true);

            var normalizedName = Normalize(dto.Name);
            var normalizedUnit = Normalize(dto.Unit);
            var normalizedCategory = NormalizeCategory(dto.Category);

            var existing = list.ShoppingListItems.FirstOrDefault(i =>
                string.Equals(i.IngredientName, normalizedName, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(i.Unit, normalizedUnit, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(NormalizeCategory(i.Category), normalizedCategory, StringComparison.OrdinalIgnoreCase));

            if (existing != null)
            {
                existing.Quantity += dto.Quantity;
                await _context.SaveChangesAsync();
                list.UpdatedAt = DateTime.UtcNow;
                return MapItemToDto(existing);
            }
            else
            {
                var newItem = new ShoppingListItem
                {
                    IngredientName = normalizedName,
                    Quantity = dto.Quantity,
                    Unit = normalizedUnit,
                    Category = normalizedCategory,
                    ShoppingListId = list.ShoppingListId,
                    IsChecked = false,
                    CreatedAt = DateTime.UtcNow
                };
                list.ShoppingListItems.Add(newItem);
                list.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return MapItemToDto(newItem);
            }
        }

        public async Task ToggleItemAsync(string userId, int itemId, bool isChecked)
        {
            var uid = ParseUserId(userId);
            var item = await _context.ShoppingListItems
                .Include(i => i.ShoppingList)
                .FirstOrDefaultAsync(i => i.ItemId == itemId && i.ShoppingList.UserId == uid);

            if (item == null)
            {
                throw new InvalidOperationException("Item not found or not owned by user.");
            }

            item.IsChecked = isChecked;
            item.ShoppingList.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteItemAsync(string userId, int itemId)
        {
            var uid = ParseUserId(userId);
            var item = await _context.ShoppingListItems
                .Include(i => i.ShoppingList)
                .FirstOrDefaultAsync(i => i.ItemId == itemId && i.ShoppingList.UserId == uid);

            if (item == null)
            {
                throw new InvalidOperationException("Item not found or not owned by user.");
            }

            _context.ShoppingListItems.Remove(item);
            item.ShoppingList.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task ClearCompletedAsync(string userId)
        {
            var uid = ParseUserId(userId);
            var items = await _context.ShoppingListItems
                .Include(i => i.ShoppingList)
                .Where(i => i.ShoppingList.UserId == uid && i.IsChecked == true)
                .ToListAsync();

            if (items.Count == 0)
            {
                return;
            }

            _context.ShoppingListItems.RemoveRange(items);
            var list = items.First().ShoppingList;
            list.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<GroupedItemsDto>> GetGroupedItemsAsync(string userId)
        {
            var uid = ParseUserId(userId);

            // Ensure list exists
            await GetOrCreateListInternalAsync(uid, includeItems: false);

            var items = await _context.ShoppingListItems
                .Where(i => i.ShoppingList.UserId == uid)
                .AsNoTracking()
                .ToListAsync();

            var grouped = items
                .GroupBy(i => NormalizeCategory(i.Category))
                .OrderBy(g => g.Key)
                .Select(g => new GroupedItemsDto
                {
                    Category = g.Key,
                    Items = g.OrderBy(i => i.IngredientName)
                        .Select(MapItemToDto)
                        .ToList()
                })
                .ToList();

            return grouped;
        }

        private ShoppingListDto MapToDto(ShoppingList list)
        {
            return new ShoppingListDto
            {
                ShoppingListId = list.ShoppingListId,
                Items = list.ShoppingListItems.Select(MapItemToDto).ToList()
            };
        }

        private ShoppingListItemDto MapItemToDto(ShoppingListItem item)
        {
            return new ShoppingListItemDto
            {
                ItemId = item.ItemId,
                Name = item.IngredientName,
                Quantity = item.Quantity,
                Unit = item.Unit,
                Category = NormalizeCategory(item.Category),
                IsChecked = item.IsChecked,
                RecipeId = item.RecipeId
            };
        }

        private async Task<ShoppingList> GetOrCreateListInternalAsync(int userId, bool includeItems)
        {
            var query = _context.ShoppingLists.AsQueryable();
            if (includeItems)
            {
                query = query.Include(l => l.ShoppingListItems);
            }

            var list = await query.FirstOrDefaultAsync(l => l.UserId == userId);
            if (list != null)
            {
                return list;
            }

            var newList = new ShoppingList
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.ShoppingLists.AddAsync(newList);
            await _context.SaveChangesAsync();
            return newList;
        }

        private static int ParseUserId(string userId)
        {
            if (!int.TryParse(userId, out var uid))
            {
                throw new ArgumentException("Invalid user id", nameof(userId));
            }
            return uid;
        }

        private static string Normalize(string value)
        {
            return value?.Trim() ?? string.Empty;
        }

        private static string NormalizeCategory(string? category)
        {
            var normalized = category?.Trim();
            return string.IsNullOrWhiteSpace(normalized) ? DefaultCategory : normalized;
        }

        private static void ValidateManualItem(AddItemDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (string.IsNullOrWhiteSpace(dto.Name)) throw new ArgumentException("Name is required", nameof(dto.Name));
            if (string.IsNullOrWhiteSpace(dto.Unit)) throw new ArgumentException("Unit is required", nameof(dto.Unit));
            if (dto.Quantity <= 0) throw new ArgumentException("Quantity must be greater than zero", nameof(dto.Quantity));
        }
    }
}

