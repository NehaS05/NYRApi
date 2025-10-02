using Microsoft.EntityFrameworkCore;
using NYR.API.Data;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;

namespace NYR.API.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet
                .Include(u => u.Role)
                .Include(u => u.Customer)
                .Include(u => u.Location)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(u => u.Role)
                .Include(u => u.Customer)
                .Include(u => u.Location)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(int roleId)
        {
            return await _dbSet
                .Include(u => u.Role)
                .Include(u => u.Customer)
                .Include(u => u.Location)
                .Where(u => u.RoleId == roleId)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUsersByCustomerAsync(int customerId)
        {
            return await _dbSet
                .Include(u => u.Role)
                .Include(u => u.Customer)
                .Include(u => u.Location)
                .Where(u => u.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUsersByLocationAsync(int locationId)
        {
            return await _dbSet
                .Include(u => u.Role)
                .Include(u => u.Customer)
                .Include(u => u.Location)
                .Where(u => u.LocationId == locationId)
                .ToListAsync();
        }

        public override async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _dbSet
                .Include(u => u.Role)
                .Include(u => u.Customer)
                .Include(u => u.Location)
                .ToListAsync();
        }

        public override async Task<User?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(u => u.Role)
                .Include(u => u.Customer)
                .Include(u => u.Location)
                .FirstOrDefaultAsync(u => u.Id == id);
        }
    }
}
