using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Data;
using OrderManagementSystem.Models;
using OrderManagementSystem.Repositories.Interface;

namespace OrderManagementSystem.Repositories.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly OrderManagementDbContext _context;
        public UserRepository(OrderManagementDbContext context) => _context = context;

        public async Task<User> GetByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
    }
}
