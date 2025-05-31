using StudyBuddyAPI.Controllers;
using StudyBuddyAPI.DTOs;
using StudyBuddyAPI.Models;
using StudyBuddyAPI.Services;

namespace StudyBuddyAPI.Services
{
    public interface IUserService
    {

        Task<(bool IsSuccess, string ErrorMessage)> CreateUserAsync(RegisterUserDto dto);
        Task<List<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(string id);
        Task<(bool IsSuccess, string ErrorMessage)> UpdateUserAsync(string id, RegisterUserDto dto);
        Task<(bool IsSuccess, string ErrorMessage)> DeleteUserAsync(string id);

        Task<(bool IsSuccess, string TokenOrError, string UserId)> LoginAsync(LoginDto dto);
    }
}
