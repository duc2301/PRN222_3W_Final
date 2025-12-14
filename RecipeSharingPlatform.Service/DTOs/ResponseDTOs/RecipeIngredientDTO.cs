using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeSharingPlatform.Service.DTOs.ResponseDTOs
{
    public class RecipeIngredientDTO
    {
        public int IngredientId { get; set; }
        public string IngredientName { get; set; } = null!;
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = null!;
        public int? OrderIndex { get; set; }
    }
}
