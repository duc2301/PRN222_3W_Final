using AutoMapper;
using RecipeSharingPlatform.Repository.DbContexts;
using RecipeSharingPlatform.Repository.UnitOfWork.Interface;
using RecipeSharingPlatform.Service.ServiceProviders.Interface;
using RecipeSharingPlatform.Service.Services;
using RecipeSharingPlatform.Service.Services.Interfaces;

namespace RecipeSharingPlatform.Service.ServiceProviders
{
    public class ServiceProviders : IServiceProviders
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly RecipeSharingDbContext _dbContext;
        private readonly IMapper _mapper;

        public ServiceProviders(IUnitOfWork unitOfWork, RecipeSharingDbContext dbContext, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        private IUserService _userService;
        public IUserService UserService => _userService ?? new UserService(_unitOfWork, _mapper);

        private IAuthService _authService;
        public IAuthService AuthService => _authService ?? new AuthService(_unitOfWork, _mapper);

        private IShoppingListService _shoppingListService;
        public IShoppingListService ShoppingListService => _shoppingListService ?? new ShoppingListService(_dbContext);

    }
}
