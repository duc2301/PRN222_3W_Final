using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeSharingPlatform.Service.DTOs.ResponseDTOs
{
    public class CategoryResponseDTO
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
    }
}
