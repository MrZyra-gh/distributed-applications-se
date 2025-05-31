using Microsoft.AspNetCore.Mvc;
using StudyBuddyMVC.DTOs;
using System.Text;
using System.Text.Json;

namespace StudyBuddyMVC.Services
{
    public class CourseApiService : ICourseApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CourseApiService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CourseApiService(IHttpClientFactory httpClientFactory, ILogger<CourseApiService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClientFactory.CreateClient("StudyBuddyAPI");
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
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

        public async Task<(bool IsSuccess, string ErrorMessage)> CreateCourseAsync(CourseDto dto)
        {
            try
            {
                SetAuthorizationHeader();

                var json = JsonSerializer.Serialize(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("courses", content);

                if (response.IsSuccessStatusCode)
                {
                    return (true, string.Empty);
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Course creation failed: {response.StatusCode} - {errorContent}");
                return (false, $"Failed to create course: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during course creation");
                return (false, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> DeleteCourseAsync(int id)
        {
            try
            {
                SetAuthorizationHeader();

                var response = await _httpClient.DeleteAsync($"courses/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return (true, string.Empty);
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Course deletion failed: {response.StatusCode} - {errorContent}");
                return (false, $"Failed to delete course: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during course deletion");
                return (false, ex.Message);
            }
        }

        public async Task<List<CourseDto>> GetCoursesByInstructorIdAsync(string instructorId)
        {
            try
            {
                SetAuthorizationHeader();

                var response = await _httpClient.GetAsync($"courses/instructor/{instructorId}");

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var courses = JsonSerializer.Deserialize<List<CourseDto>>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return courses ?? new List<CourseDto>();
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Failed to get courses by instructor: {response.StatusCode} - {errorContent}");
                return new List<CourseDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting courses by instructor");
                return new List<CourseDto>();
            }
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> EnrollStudentAsync(int courseId, string studentId)
        {
            try
            {
                SetAuthorizationHeader();

                var enrollmentData = new
                {
                    CourseId = courseId,
                    StudentId = studentId
                };

                var json = JsonSerializer.Serialize(enrollmentData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("enrollment/enroll", content);

                if (response.IsSuccessStatusCode)
                {
                    return (true, string.Empty);
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Enrollment failed: {response.StatusCode} - {errorContent}");
                return (false, $"Failed to enroll student: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during student enrollment");
                return (false, ex.Message);
            }
        }

        public async Task<List<UserDto>> GetEnrolledStudentsAsync(int courseId)
        {
            try
            {
                SetAuthorizationHeader();

                var response = await _httpClient.GetAsync($"enrollment/course/{courseId}/students");

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var students = JsonSerializer.Deserialize<List<UserDto>>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return students ?? new List<UserDto>();
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Failed to get enrolled students: {response.StatusCode} - {errorContent}");
                return new List<UserDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting enrolled students");
                return new List<UserDto>();
            }
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> RemoveStudentAsync(int courseId, string studentId)
        {
            try
            {
                SetAuthorizationHeader();

                var unenrollData = new { CourseId = courseId, StudentId = studentId };
                var json = JsonSerializer.Serialize(unenrollData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(_httpClient.BaseAddress, "enrollment/unenroll"),
                    Content = content
                };

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    return (true, string.Empty);
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Failed to remove student: {response.StatusCode} - {errorContent}");
                return (false, $"Failed to remove student: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing student from course");
                return (false, ex.Message);
            }
        }


        public async Task<List<CourseDto>> GetCoursesByStudentIdAsync(string userId)
        {
            try
            {
                SetAuthorizationHeader();
                
                var response = await _httpClient.GetAsync($"enrollments/student/{userId}");

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var courses = JsonSerializer.Deserialize<List<CourseDto>>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return courses ?? new List<CourseDto>();
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Failed to get courses by student: {response.StatusCode} - {errorContent}");
                return new List<CourseDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting courses by student");
                return new List<CourseDto>();
            }
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> UpdateCourseAsync(int id, CourseDto dto)
        {
            try
            {
                SetAuthorizationHeader();

                var json = JsonSerializer.Serialize(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"courses/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    return (true, string.Empty);
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Course update failed: {response.StatusCode} - {errorContent}");
                return (false, $"Failed to update course: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during course update");
                return (false, ex.Message);
            }
        }       

        public async Task<List<CourseDto>> GetAllCoursesAsync()
        {
            try
            {
                SetAuthorizationHeader();

                var response = await _httpClient.GetAsync("courses");

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var courses = JsonSerializer.Deserialize<List<CourseDto>>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return courses ?? new List<CourseDto>();
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Failed to get all courses: {response.StatusCode} - {errorContent}");
                return new List<CourseDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all courses");
                return new List<CourseDto>();
            }
        }

        public async Task<CourseDto?> GetCourseByIdAsync(int id)
        {
            try
            {
                SetAuthorizationHeader();

                var response = await _httpClient.GetAsync($"courses/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var course = JsonSerializer.Deserialize<CourseDto>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return course;
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Failed to get course by id: {response.StatusCode} - {errorContent}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting course by id");
                return null;
            }
        }
    }
}