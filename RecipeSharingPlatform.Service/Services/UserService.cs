using AutoMapper;
using RecipeSharingPlatform.Repository.Models;
using RecipeSharingPlatform.Repository.UnitOfWork.Interface;
using RecipeSharingPlatform.Service.DTOs.RequestDTOs;
using RecipeSharingPlatform.Service.DTOs.ResponseDTOs;
using RecipeSharingPlatform.Service.Services.Interfaces;


namespace RecipeSharingPlatform.Service.Services
{
    public class UserService : IUserService
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<UserResponseDTO> Create(CreateUserDTO user)
        {
            var userEntity = _mapper.Map<User>(user);
            userEntity.CreatedAt = DateTime.UtcNow;
            var userResponse = await _unitOfWork.UserRepository.CreateAsync(userEntity);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<UserResponseDTO>(userResponse);
        }

        public async Task<UserResponseDTO> Delete(int id)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            _unitOfWork.UserRepository.Remove(user);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<UserResponseDTO>(user);
        }

        public async Task<List<UserResponseDTO>> GetAll()
        {
            var users = await _unitOfWork.UserRepository.GetAllAsync();
            return _mapper.Map<List<UserResponseDTO>>(users);
        }

        public async Task<UserResponseDTO> GetById(int id)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            return _mapper.Map<UserResponseDTO>(user);
        }

        public async Task<UserResponseDTO> Update(UpdateUserDTO user)
        {
            var updatedUser = _mapper.Map<User>(user);
            updatedUser.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.UserRepository.Update(updatedUser);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<UserResponseDTO>(user);
        }
    }
}
