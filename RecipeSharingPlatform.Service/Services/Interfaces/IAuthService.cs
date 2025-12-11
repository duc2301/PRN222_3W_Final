using RecipeSharingPlatform.Service.DTOs.ResponseDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeSharingPlatform.Service.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDTO> Login(string username, string password);
        Task<UserResponseDTO> Register(string username, string password);
    }
}
