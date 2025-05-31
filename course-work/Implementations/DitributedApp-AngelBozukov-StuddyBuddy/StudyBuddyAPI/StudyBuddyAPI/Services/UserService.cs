using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StudyBuddyAPI.Controllers;
using StudyBuddyAPI.DTOs;
using StudyBuddyAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StudyBuddyAPI.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;

        public UserService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        // Login response model


        public async Task<(bool IsSuccess, string TokenOrError, string? UserId)> LoginAsync(LoginDto DTO)
        {
            var user = await _userManager.FindByEmailAsync(DTO.Email);
            if (user == null)
                return (false, "Invalid credentials", null);

            var result = await _signInManager.CheckPasswordSignInAsync(user, DTO.Password, false);
            if (!result.Succeeded)
                return (false, "Invalid credentials", null);

            // Create JWT token
            var claims = new[]
                    {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return (true, new JwtSecurityTokenHandler().WriteToken(token), user.Id);
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> CreateUserAsync(RegisterUserDto DTO)
        {
            try
            {
                var user = new User
                {
                    UserName = DTO.UserName,
                    FullName = DTO.FullName,
                    Email = DTO.Email,
                    Role = DTO.Role
                };

                var result = await _userManager.CreateAsync(user, DTO.Password);
                if (!result.Succeeded)
                {
                    return (false, string.Join(", ", result.Errors.Select(e => e.Description)));
                }

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await Task.FromResult(_userManager.Users.ToList());
        }

        public async Task<User?> GetUserByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> UpdateUserAsync(string id, RegisterUserDto DTO)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                    return (false, "User not found");

                user.UserName = DTO.UserName;
                user.FullName = DTO.FullName;
                user.Email = DTO.Email;
                user.Role = DTO.Role;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return (false, string.Join(", ", result.Errors.Select(e => e.Description)));
                }

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> DeleteUserAsync(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                    return (false, "User not found");

                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    return (false, string.Join(", ", result.Errors.Select(e => e.Description)));
                }

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
