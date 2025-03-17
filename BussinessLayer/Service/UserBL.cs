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
        private readonly EmailService _emailService;

        public UserBL(IUserRL userRepository, JwtTokenGenerator jwtTokenGenerator, EmailService emailService)
        {
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _emailService = emailService;
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

        public async Task<bool> ForgotPassword(ForgotPasswordDTO forgotPasswordDTO)
        {
            var user = await _userRepository.GetUserByEmail(forgotPasswordDTO.Email);
            if (user == null) return false;

            user.ResetToken = Guid.NewGuid().ToString();
            user.ResetTokenExpiry = DateTime.UtcNow.AddHours(2);

            await _userRepository.UpdateUser(user);

            string resetLink = $"https://localhost:7135/api/auth/reset-password?token={user.ResetToken}";
            string emailBody = $"Click <a href='{resetLink}'>here</a> to reset your password.";

            await _emailService.SendEmailAsync(user.Email, "Password Reset", emailBody);

            return true;
        }

        public async Task<bool> ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            var user = await _userRepository.GetUserByEmail(resetPasswordDTO.Token);
            if (user == null || user.ResetTokenExpiry > DateTime.UtcNow) return false;

            user.PasswordHash = PasswordHasher.HashPassword(resetPasswordDTO.NewPassword);
            user.ResetToken = null;
            user.ResetTokenExpiry = null;

            await _userRepository.UpdateUser(user);

            return true;
        }
    }
}
