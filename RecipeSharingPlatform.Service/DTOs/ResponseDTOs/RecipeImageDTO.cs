using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeSharingPlatform.Service.DTOs.ResponseDTOs
{
    public class RecipeImageDTO
    {
        public int ImageId { get; set; }
        public string ImageUrl { get; set; } = null!;
        public int? ImageOrder { get; set; }
    }
}
