using RecipeSharingPlatform.Repository.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeSharingPlatform.Repository.UnitOfWork.Interface
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IRecipeRepository RecipeRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        ILikeRepository LikeRepository { get; }
        ISavedRecipeRepository SavedRecipeRepository { get; }
        Task<int> SaveChangesAsync();
    }
}
