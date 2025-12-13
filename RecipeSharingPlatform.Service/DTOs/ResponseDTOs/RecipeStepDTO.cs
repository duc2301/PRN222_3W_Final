using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeSharingPlatform.Service.DTOs.ResponseDTOs
{
    public class RecipeStepDTO
    {
        public int StepId { get; set; }
        public int StepNumber { get; set; }
        public string StepDescription { get; set; } = null!;
        public string? ImageUrl { get; set; }
    }
}
