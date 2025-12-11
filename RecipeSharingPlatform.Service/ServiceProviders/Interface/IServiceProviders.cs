using AutoMapper;
using RecipeSharingPlatform.Service.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeSharingPlatform.Service.ServiceProviders.Interface
{
    public interface IServiceProviders
    {
        IUserService UserService { get; }
        IAuthService AuthService { get; }
    }
}
