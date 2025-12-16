using System.Collections.Generic;

namespace RecipeSharingPlatform.Service.DTOs
{
    public class IngredientLineViewModel
    {
        public string Name { get; set; } = null!;
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = null!;
    }

    public class IngredientCalculatorViewModel
    {
        public int RecipeId { get; set; }
        public int BaseServings { get; set; }
        public IEnumerable<IngredientLineViewModel> Ingredients { get; set; } = new List<IngredientLineViewModel>();
    }
}

