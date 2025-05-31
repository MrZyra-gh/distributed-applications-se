using StudyBuddyMVC.DTOs;

namespace StudyBuddyMVC.Services
{
    public interface IUserApiService
    {
        Task<bool> RegisterAsync(RegisterUserDto dto);
        Task<TokenResponseDto> LoginAsync(LoginDto dto);
        Task<List<UserDto>> GetUsersByRoleAsync(string role);
        Task<UserDto> GetUserByIdAsync(string Id);
        
    }
}
