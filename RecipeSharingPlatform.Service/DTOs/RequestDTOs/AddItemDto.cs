using System.ComponentModel.DataAnnotations;

namespace RecipeSharingPlatform.Service.DTOs.RequestDTOs
{
    public class AddItemDto
    {
        [Required]
        public string Name { get; set; } = null!;

        [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
        public decimal Quantity { get; set; }

        [Required]
        public string Unit { get; set; } = null!;

        public string? Category { get; set; }
    }
}

