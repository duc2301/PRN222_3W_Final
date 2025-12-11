using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeSharingPlatform.Service.DTOs.RequestDTOs
{
    public class CreateUserDTO
    {
        public string Username { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string? FullName { get; set; }

        public string? Bio { get; set; }

        public string? ProfileImage { get; set; }
    }
}
