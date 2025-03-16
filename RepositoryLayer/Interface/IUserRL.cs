using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Model;

namespace RepositoryLayer.Interface
{
    public interface IUserRL
    {
        Task<UserEntity> RegisterUser(UserEntity user);
        Task<UserEntity> GetUserByEmail(string email);
    }
}
