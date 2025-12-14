using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeSharingPlatform.Service.DTOs.RequestDTOs
{
    public class RecipeListFilterDTO
    {
        public string? Search { get; set; }
        public int? CategoryId { get; set; }
        public string? SortBy { get; set; } = "newest"; // newest | mostViewed | mostLiked
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
    }
}
