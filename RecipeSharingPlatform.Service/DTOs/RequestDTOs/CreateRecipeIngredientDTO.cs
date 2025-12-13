using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeSharingPlatform.Service.DTOs.RequestDTOs
{
    public class CreateRecipeIngredientDTO
    {
        public string IngredientName { get; set; } = null!;
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = null!;
        public int OrderIndex { get; set; }
    }
}
