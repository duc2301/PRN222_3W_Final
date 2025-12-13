using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeSharingPlatform.Service.DTOs.RequestDTOs
{
    public class CreateRecipeStepDTO
    {
        public int StepNumber { get; set; }
        public string StepDescription { get; set; } = null!;
        public string? ImageUrl { get; set; }
    }
}
