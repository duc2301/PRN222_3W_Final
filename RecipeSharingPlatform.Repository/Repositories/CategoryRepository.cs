using Microsoft.EntityFrameworkCore;
using RecipeSharingPlatform.Repository.Basic;
using RecipeSharingPlatform.Repository.DbContexts;
using RecipeSharingPlatform.Repository.Models;
using RecipeSharingPlatform.Repository.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeSharingPlatform.Repository.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly RecipeSharingDbContext _context;

        public CategoryRepository(RecipeSharingDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await _context.Categories
                .OrderBy(c => c.CategoryName)
                .ToListAsync();
        }

    }
}
