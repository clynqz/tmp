using Anomaly.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Anomaly.Data.Repositories
{
    public class UsersRepository(DataContext context)
    {
        private readonly DataContext _context = context;

        public async Task<UserEntity?> GetUserByLoginAsync(string login)
        {
            var query = _context.Users.Where(user => user.Email == login || user.Nickname == login);

            var user = await query.SingleOrDefaultAsync();

            return user;
        }

        public async Task AddUserAsync(UserEntity user)
        {
            await _context.AddAsync(user);

            await _context.SaveChangesAsync();
        }

        public async Task<UserEntity?> GetUserByIdAsync(int id)
        {
            var result = await _context.Users.SingleOrDefaultAsync(user => user.Id == id);

            return result;
        }

        public async Task<bool> IsNicknameExistAsync(string nickname)
        {
            var result = await _context.Users.AnyAsync(user => user.Nickname == nickname);

            return result;
        }

        public async Task<bool> IsEmailExistAsync(string email)
        {
            var result = await _context.Users.AnyAsync(user => user.Email == email);

            return result;
        }
    }
}
