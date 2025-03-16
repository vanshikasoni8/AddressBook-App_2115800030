using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.DTO;
using ModelLayer.Model;

namespace BussinessLayer.Interface
{
    public interface IUserBL
    {
        Task<UserEntity> Register(UserDTO userDTO);
        Task<string> Login(UserDTO userDTO);
    }
}
