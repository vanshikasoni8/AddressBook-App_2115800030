using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ModelLayer.Model;
using RepositoryLayer.Context;
using RepositoryLayer.Interface;

namespace RepositoryLayer.Service
{
    public class UserRL : IUserRL
    {
        private readonly AddressBookContext _context;

        public UserRL(AddressBookContext context)
        {
            _context = context;
        }

        public async Task<UserEntity> RegisterUser(UserEntity user)
        {
            _context.User.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<UserEntity> GetUserByEmail(string email)
        {
            return await _context.User.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<UserEntity> GetUserByResetToken(string resetToken)
        {
            return await _context.User
                 .FirstOrDefaultAsync(u => u.ResetToken == resetToken);
        }

        public async Task UpdateUser(UserEntity user)
        {
            _context.User.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
