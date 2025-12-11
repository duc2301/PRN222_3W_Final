using AutoMapper;
using RecipeSharingPlatform.Repository.UnitOfWork.Interface;
using RecipeSharingPlatform.Service.ServiceProviders.Interface;
using RecipeSharingPlatform.Service.Services;
using RecipeSharingPlatform.Service.Services.Interfaces;

namespace RecipeSharingPlatform.Service.ServiceProviders
{
    public class ServiceProviders : IServiceProviders
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ServiceProviders(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        private IUserService _userService;
        public IUserService UserService => _userService ?? new UserService(_unitOfWork, _mapper);

        private IAuthService _authService;
        public IAuthService AuthService => _authService ?? new AuthService(_unitOfWork, _mapper);

    }
}
