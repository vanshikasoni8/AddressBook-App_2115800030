using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BussinessLayer.Helper;
using BussinessLayer.Interface;
using Microsoft.Extensions.Configuration;
using ModelLayer.DTO;
using ModelLayer.Model;
using RabbitMQ.Client;
using RepositoryLayer.Interface;
using StackExchange.Redis;

namespace BussinessLayer.Service
{
    public class UserBL:IUserBL
    {
        private readonly IUserRL _userRepository;
        private readonly JwtTokenGenerator _jwtTokenGenerator;
        private readonly EmailService _emailService;
        private readonly IDatabase _cache;
        private readonly TimeSpan _cacheExpiration;
        private readonly IConnection _rabbitMqConnection;

        public UserBL(IUserRL userRepository, JwtTokenGenerator jwtTokenGenerator, EmailService emailService, IConnectionMultiplexer redis, IConfiguration config, IConnection rabbitMqConnection)
        {
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _emailService = emailService;
            _cache = redis.GetDatabase();
            _cacheExpiration = TimeSpan.FromMinutes(int.Parse(config["Redis:CacheExpirationMinutes"] ?? "10"));
            _rabbitMqConnection = rabbitMqConnection;
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

            var registeredUser = await _userRepository.RegisterUser(user);

            // Cache user data
            await _cache.StringSetAsync($"user:{registeredUser.Email}", System.Text.Json.JsonSerializer.Serialize(registeredUser), _cacheExpiration);

            // Publish user registration event to RabbitMQ
            PublishMessage("UserRegisteredQueue", new { Email = registeredUser.Email, Name = registeredUser.Name });


            return registeredUser;


        }

        public async Task<string> Login(UserDTO userDTO)
        {
            string cacheKey = $"user:{userDTO.Email}";

            // Try getting the user from cache
            var cachedUser = await _cache.StringGetAsync(cacheKey);
            UserEntity user;

            if (!cachedUser.IsNullOrEmpty)
            {
                user = System.Text.Json.JsonSerializer.Deserialize<UserEntity>(cachedUser);
            }
            else
            {
                // Fetch from DB if not in cache
                user = await _userRepository.GetUserByEmail(userDTO.Email);
                if (user != null)
                {
                    await _cache.StringSetAsync(cacheKey, System.Text.Json.JsonSerializer.Serialize(user), _cacheExpiration);
                }
            }

            if (user == null || !PasswordHasher.VerifyPassword(userDTO.Password, user.PasswordHash))
            {
                return null; // Invalid credentials
            }

            return _jwtTokenGenerator.GenerateToken(user.Email);
        }

        public async Task<bool> ForgotPassword(ForgotPasswordDTO forgotPasswordDTO)
        {
            string cacheKey = $"user:{forgotPasswordDTO.Email}";

            // Try fetching from cache first
            var cachedUser = await _cache.StringGetAsync(cacheKey);
            UserEntity user;

            if (!cachedUser.IsNullOrEmpty)
            {
                user = System.Text.Json.JsonSerializer.Deserialize<UserEntity>(cachedUser);
            }
            else
            {
                user = await _userRepository.GetUserByEmail(forgotPasswordDTO.Email);
                if (user == null) return false;
            }

            user.ResetToken = Guid.NewGuid().ToString();
            user.ResetTokenExpiry = DateTime.UtcNow.AddHours(2);

            await _userRepository.UpdateUser(user);

            // Update cache with new reset token
            await _cache.StringSetAsync(cacheKey, System.Text.Json.JsonSerializer.Serialize(user), _cacheExpiration);

            string resetLink = $"https://localhost:7135/api/auth/reset-password?token={user.ResetToken}";
            string emailBody = $"Click <a href='{resetLink}'>here</a> to reset your password.";

            await _emailService.SendEmailAsync(user.Email, "Password Reset", emailBody);

            return true;
        }

        public async Task<bool> ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            var user = await _userRepository.GetUserByResetToken(resetPasswordDTO.Token);
            if (user == null || user.ResetTokenExpiry < DateTime.UtcNow) return false;

            user.PasswordHash = PasswordHasher.HashPassword(resetPasswordDTO.NewPassword);
            user.ResetToken = null;
            user.ResetTokenExpiry = null;

            await _userRepository.UpdateUser(user);

            // Invalidate cache after password reset
            await _cache.KeyDeleteAsync($"user:{user.Email}");

            return true;
        }

        //for publishing the message 

        private void PublishMessage<T>(string routingKey, T message)
        {
            using var channel = _rabbitMqConnection.CreateModel();
            channel.ExchangeDeclare("events", ExchangeType.Topic);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            channel.BasicPublish(exchange: "events", routingKey: routingKey, body: body);
        }
    }
}
