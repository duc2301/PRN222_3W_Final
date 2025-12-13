using RecipeSharingPlatform.Repository.Basic.Interfaces;
using RecipeSharingPlatform.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeSharingPlatform.Repository.Repositories.Interfaces
{
    public interface ICategoryRepository 
    {
        Task<List<Category>> GetAllAsync();
    }
}
