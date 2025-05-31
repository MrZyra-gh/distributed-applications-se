using StudyBuddyMVC.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace StudyBuddyMVC.Services
{
    public class AssignmentApiService : IAssignmentApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AssignmentApiService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICourseApiService _courseApiService;


        public AssignmentApiService(IHttpClientFactory httpClientFactory, ILogger<AssignmentApiService> logger, ICourseApiService courseApiService, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClientFactory.CreateClient("StudyBuddyAPI");
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _courseApiService = courseApiService;

        }

        private void SetAuthorizationHeader()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("token");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<AssignmentDto?> GetAssignmentByIdAsync(int id)
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.GetAsync($"assignments/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<AssignmentDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }

                _logger.LogError("Failed to get assignment by ID {Id}. Status: {Status}", id, response.StatusCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving assignment by ID {Id}", id);
                return null;
            }
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> CreateAssignmentAsync(AssignmentDto dto)
        {
            try
            {
                SetAuthorizationHeader();

                var json = JsonSerializer.Serialize(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("assignments", content);

                if (response.IsSuccessStatusCode)
                {
                    return (true, string.Empty);
                }

                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to create assignment. Status: {Status}. Error: {Error}", response.StatusCode, error);
                return (false, $"Failed to create assignment: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating assignment");
                return (false, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> UpdateAssignmentAsync(int id, AssignmentDto dto)
        {
            try
            {
                SetAuthorizationHeader();

                var json = JsonSerializer.Serialize(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"assignments/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    return (true, string.Empty);
                }

                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to update assignment ID {Id}. Status: {Status}. Error: {Error}", id, response.StatusCode, error);
                return (false, $"Failed to update assignment: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating assignment ID {Id}", id);
                return (false, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> DeleteAssignmentAsync(int id)
        {
            try
            {
                SetAuthorizationHeader();

                var response = await _httpClient.DeleteAsync($"assignments/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return (true, string.Empty);
                }

                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to delete assignment ID {Id}. Status: {Status}. Error: {Error}", id, response.StatusCode, error);
                return (false, $"Failed to delete assignment: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting assignment ID {Id}", id);
                return (false, ex.Message);
            }
        }

        public async Task<List<AssignmentDto>> GetAssignmentsByCourseIdAsync(int courseId)
        {
            try
            {
                SetAuthorizationHeader();

                var response = await _httpClient.GetAsync($"assignments/course/{courseId}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var assignments = JsonSerializer.Deserialize<List<AssignmentDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return assignments ?? new List<AssignmentDto>();
                }

                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to get assignments by course ID {CourseId}. Status: {Status}. Error: {Error}", courseId, response.StatusCode, error);
                return new List<AssignmentDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting assignments by course ID {CourseId}", courseId);
                return new List<AssignmentDto>();
            }
        }

        public async Task<List<AssignmentDto>> GetAssignmentsByUserIdAsync(string userId)
        {
            var allAssignments = new List<AssignmentDto>();

            try
            {
                SetAuthorizationHeader();

                // Get courses (both as instructor and student)
                var instructorCourses = await _courseApiService.GetCoursesByInstructorIdAsync(userId);
                var studentCourses = await _courseApiService.GetCoursesByStudentIdAsync(userId);

                // Combine course lists without duplicates (assuming CourseDto.Id is unique)
                var allCourses = instructorCourses.Concat(studentCourses)
                                                  .GroupBy(c => c.Id)
                                                  .Select(g => g.First())
                                                  .ToList();

                // Loop through each course and get assignments
                foreach (var course in allCourses)
                {
                    var assignments = await GetAssignmentsByCourseIdAsync(course.Id);
                    allAssignments.AddRange(assignments);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving assignments for user ID {UserId}", userId);
            }

            return allAssignments;
        }

    }
}
