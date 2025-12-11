using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeSharingPlatform.Service.DTOs.ResponseDTOs
{
    public class LoginResponseDTO
    {
        public string Username { get; set; } = null!;

        public string? ProfileImage { get; set; }

        public string? Role { get; set; }
    }
}
