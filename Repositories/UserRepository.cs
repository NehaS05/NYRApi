using Microsoft.EntityFrameworkCore;
using NYR.API.Data;
using NYR.API.Helpers;
using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;
using System.Linq.Expressions;

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
                .Include(u => u.Warehouse)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(u => u.Role)
                .Include(u => u.Customer)
                .Include(u => u.Warehouse)
                .Include(u => u.Location)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(int roleId)
        {
            return await _dbSet
                .Include(u => u.Role)
                .Include(u => u.Customer)
                .Include(u => u.Location)
                .Include(u => u.Warehouse)
                .Where(u => u.RoleId == roleId)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUsersByCustomerAsync(int customerId)
        {
            return await _dbSet
                .Include(u => u.Role)
                .Include(u => u.Customer)
                .Include(u => u.Location)
                .Include(u => u.Warehouse)
                .Where(u => u.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUsersByLocationAsync(int locationId)
        {
            return await _dbSet
                .Include(u => u.Role)
                .Include(u => u.Customer)
                .Include(u => u.Location)
                .Include(u => u.Warehouse)
                .Where(u => u.LocationId == locationId)
                .ToListAsync();
        }

        public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
        {
            return await _dbSet
                .Include(u => u.Role)
                .Include(u => u.Customer)
                .Include(u => u.Location)
                .Include(u => u.Warehouse)
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        }

        public override async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _dbSet
                .Include(u => u.Role)
                .Include(u => u.Customer)
                .Include(u => u.Location)
                .Include(u => u.Warehouse)
                .ToListAsync();
        }

        public override async Task<User?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(u => u.Role)
                .Include(u => u.Customer)
                .Include(u => u.Location)
                .Include(u => u.Warehouse)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<(IEnumerable<User> Items, int TotalCount)> GetPagedAsync(PaginationParamsDto paginationParams)
        {
            var query = BuildBaseQuery();
            query = ApplySearchFilter(query, paginationParams.Search);
            
            var totalCount = await query.CountAsync();

            var sortFields = GetSortFields();
            query = query.ApplySorting(paginationParams.SortBy, paginationParams.SortOrder, sortFields, u => u.Name);
            query = query.ApplyPagination(paginationParams.PageNumber, paginationParams.PageSize);

            var items = await query.ToListAsync();
            return (items, totalCount);
        }

        private IQueryable<User> BuildBaseQuery()
        {
            return _dbSet
                .Include(u => u.Role)
                .Include(u => u.Customer)
                .Include(u => u.Location)
                .Include(u => u.Warehouse)
                .Where(u => u.IsActive);
        }

        private IQueryable<User> ApplySearchFilter(IQueryable<User> query, string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return query;

            var search = searchTerm.Trim();
            return query.Where(u =>
                EF.Functions.Like(u.Name, $"%{search}%") ||
                EF.Functions.Like(u.Email, $"%{search}%") ||
                (u.PhoneNumber != null && EF.Functions.Like(u.PhoneNumber, $"%{search}%")));
        }

        private Dictionary<string, Expression<Func<User, object>>> GetSortFields()
        {
            return new Dictionary<string, Expression<Func<User, object>>>
            {
                { "name", u => u.Name },
                { "email", u => u.Email },
                { "phonenumber", u => u.PhoneNumber ?? "" },
                { "role", u => u.Role.Name },
                { "createdat", u => u.CreatedAt }
            };
        }
        public async Task<IEnumerable<User>> GetDriversAssignedToVansAsync()
        {
            // Use LINQ with AsNoTracking for better performance while avoiding raw SQL navigation issues
            return await _dbSet
                .AsNoTracking()
                .Include(u => u.Role)
                .Include(u => u.Customer)
                .Include(u => u.Location)
                .Include(u => u.Warehouse)
                .Where(u => _context.Vans.Any(v => v.DriverId == u.Id))
                .OrderBy(u => u.Name)
                .ToListAsync();
        }
    }
}
