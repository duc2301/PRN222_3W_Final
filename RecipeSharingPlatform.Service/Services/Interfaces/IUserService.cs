using RecipeSharingPlatform.Service.DTOs.RequestDTOs;
using RecipeSharingPlatform.Service.DTOs.ResponseDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeSharingPlatform.Service.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<UserResponseDTO>> GetAll();
        Task<UserResponseDTO> GetById(int id);
        Task<UserResponseDTO> Update(UpdateUserDTO user);
        Task<UserResponseDTO> Create(CreateUserDTO user);
        Task<UserResponseDTO> Delete(int id);
    }
}
