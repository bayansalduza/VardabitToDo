using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Vardabit.Domain.Models;
using Vardabit.Infrastructure.UnitOfWork;
using Vardabit.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using Vardabit.Domain.DTOs;
using Microsoft.Extensions.Logging;

namespace Vardabit.Service.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserService> _logger;

        public UserService(IUnitOfWork unitOfWork, IConfiguration configuration, ILogger<UserService> logger)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task AddAsync(User user)
        {
            _logger.LogWarning("AddAsync started for new user with UserName {UserName}", user.UserName);
            user.PasswordHash = ComputeHash(user.PasswordHash);
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.CommitAsync();
                _logger.LogWarning("AddAsync completed. New user added with Id {UserId}", user.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in AddAsync for user with UserName {UserName}", user.UserName);
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateAsync(User user)
        {
            _logger.LogWarning("UpdateAsync started for user with Id {UserId}", user.Id);
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var existing = await _unitOfWork.Users.GetByIdAsync(user.Id);
                if (existing == null)
                {
                    _logger.LogWarning("UpdateAsync: User with Id {UserId} not found", user.Id);
                    throw new Exception("Kullanıcı bulunamadı!");
                }

                existing.Name = user.Name;
                existing.Surname = user.Surname;
                existing.UserName = user.UserName;
                existing.PasswordHash = ComputeHash(user.PasswordHash);

                _unitOfWork.Users.Update(existing);
                await _unitOfWork.CommitAsync();
                _logger.LogWarning("UpdateAsync completed for user with Id {UserId}", user.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in UpdateAsync for user with Id {UserId}", user.Id);
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<UserDto> GetByIdAsync(int id)
        {
            _logger.LogWarning("GetByIdAsync started for user with Id {UserId}", id);

            try
            {
                var users = await _unitOfWork.Users.GetAllAsync(query =>
                    query.Include(u => u.Baskets)
                         .ThenInclude(b => b.Product)
                );

                var user = users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                {
                    _logger.LogWarning("GetByIdAsync: User with Id {UserId} not found", id);
                    throw new Exception("Kullanıcı bulunamadı!");
                }

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Surname = user.Surname,
                    UserName = user.UserName,
                    Baskets = user.Baskets.Select(b => new BasketDto
                    {
                        Id = b.Id,
                        ProductId = b.ProductId,
                        Amount = b.Amount,
                        UserId = b.UserId,
                        ProductName = b.Product.Name,
                        ProductCode = b.Product.Code,
                        UserName = b.User.UserName
                    }).ToList()
                };

                _logger.LogWarning("GetByIdAsync completed for user with Id {UserId}", id);
                return userDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in GetByIdAsync for user with Id {UserId}", id);
                throw;
            }
        }

        public async Task<string> LoginAsync(string username, string password)
        {
            _logger.LogWarning("LoginAsync started for user with UserName {UserName}", username);

            try
            {
                string hashedPassword = ComputeHash(password);
                var allUsers = await _unitOfWork.Users.GetAllAsync();

                var user = allUsers.FirstOrDefault(u =>
                    u.UserName.Equals(username, StringComparison.OrdinalIgnoreCase) &&
                    u.PasswordHash == hashedPassword);

                if (user == null)
                {
                    _logger.LogWarning("LoginAsync: Login failed for user with UserName {UserName}", username);
                    return null;
                }

                string token = GenerateJwtToken(user);
                _logger.LogWarning("LoginAsync completed. Login successful for user with Id {UserId}", user.Id);
                return token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in LoginAsync for user with UserName {UserName}", username);
                throw;
            }
        }

        private string GenerateJwtToken(User user)
        {
            var secretKey = _configuration["JwtSettings:SecretKey"];
            var issuer = _configuration["JwtSettings:Issuer"];
            var audience = _configuration["JwtSettings:Audience"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var token = new JwtSecurityToken(issuer, audience, claims, expires: DateTime.UtcNow.AddHours(2), signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string ComputeHash(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hashBytes = sha256.ComputeHash(bytes);
                var sb = new StringBuilder();

                foreach (var b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}
