using Microsoft.AspNetCore.Http;
using StudyBuddyMVC.DTOs;
using System.Text;
using System.Text.Json;

namespace StudyBuddyMVC.Services
{

    public class UserApiService : IUserApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserApiService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserApiService(IHttpClientFactory httpClientFactory, ILogger<UserApiService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClientFactory.CreateClient("StudyBuddyAPI");
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<bool> RegisterAsync(RegisterUserDto dto)
        {
            try
            {
                var json = JsonSerializer.Serialize(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Make sure this matches your API endpoint
                var response = await _httpClient.PostAsync("users/register", content);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                // Log the error for debugging
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Registration failed: {response.StatusCode} - {errorContent}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                return false;
            }
        }

        public async Task<TokenResponseDto> LoginAsync(LoginDto dto)
        {
            try
            {
                var json = JsonSerializer.Serialize(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("users/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    TokenResponseDto tokenResponse = JsonSerializer.Deserialize<TokenResponseDto>(
                        responseContent,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );
                    return (tokenResponse);
                }
                return (null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return (null);
            }
        }

        public async Task<UserDto?> GetUserByIdAsync(string Id)
        {
            try
            {
                var token = _httpContextAccessor.HttpContext?.Session.GetString("token");


                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }
                // Call the GetUser endpoint with the user ID
                var response = await _httpClient.GetAsync($"users/{Id}");
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var user = JsonSerializer.Deserialize<UserDto>(
                        responseContent,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );
                    return user;
                }

                _logger.LogWarning("Failed to get user with ID {UserId}. Status: {StatusCode}", Id, response.StatusCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during getting user ID {UserId}", Id);
                return null;
            }
        }

        public async Task<List<UserDto>> GetUsersByRoleAsync(string role)
        {
            try
            {
                var token = _httpContextAccessor.HttpContext?.Session.GetString("token");


                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }
                var response = await _httpClient.GetAsync("users");
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var students = JsonSerializer.Deserialize<List<UserDto>>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    var studentsOfRole = students
                    .Where(s => !(string.Equals(role, s.Role)))
                    .ToList();

                    return studentsOfRole ?? new List<UserDto>();
                }
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Failed to get students");
                return new List<UserDto>();


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting students");
                return new List<UserDto>();
            }
        }
    }
}