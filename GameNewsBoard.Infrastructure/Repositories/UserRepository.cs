using GameNewsBoard.Application.IRepository;
using GameNewsBoard.Domain.Entities;
using GameNewsBoard.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GameNewsBoard.Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<bool> ExistsByUsernameAsync(string username)
        {
            return await _dbSet.AnyAsync(u => u.Username == username);
        }
    }
}