using AutoMapper;
using RecipeSharingPlatform.Repository.UnitOfWork.Interface;
using RecipeSharingPlatform.Service.DTOs.ResponseDTOs;
using RecipeSharingPlatform.Service.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeSharingPlatform.Service.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AuthService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<LoginResponseDTO> Login(string username, string password)
        {
            var user = await _unitOfWork.UserRepository.Login(username, password);
            if (user == null)
            {
                return null;
            }
            else return _mapper.Map<LoginResponseDTO>(user);
        }

        public Task<UserResponseDTO> Register(string username, string password)
        {
            throw new NotImplementedException();
        }
    }
}
