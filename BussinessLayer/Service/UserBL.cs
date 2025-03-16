using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BussinessLayer.Helper;
using BussinessLayer.Interface;
using ModelLayer.DTO;
using ModelLayer.Model;
using RepositoryLayer.Interface;

namespace BussinessLayer.Service
{
    public class UserBL:IUserBL
    {
        private readonly IUserRL _userRepository;
        private readonly JwtTokenGenerator _jwtTokenGenerator;

        public UserBL(IUserRL userRepository, JwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<UserEntity> Register(UserDTO userDTO)
        {
            var hashedPassword = PasswordHasher.HashPassword(userDTO.Password);
            var user = new UserEntity
            {
                Email = userDTO.Email,
                PasswordHash = hashedPassword,
                Name = userDTO.Name
            };

            return await _userRepository.RegisterUser(user);
        }

        public async Task<string> Login(UserDTO userDTO)
        {
            var user = await _userRepository.GetUserByEmail(userDTO.Email);
            if (user == null || !PasswordHasher.VerifyPassword(userDTO.Password, user.PasswordHash))
            {
                return null; // Invalid credentials
            }

            return _jwtTokenGenerator.GenerateToken(user.Email);
        }
    }
}
