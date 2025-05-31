using StudyBuddyAPI.DTOs;
using StudyBuddyAPI.Models;
using StudyBuddyAPI.Controllers;

namespace StudyBuddyAPI.Services
{
    public interface IEnrollmentService
    {

        Task<(bool IsSuccess, string ErrorMessage)> EnrollStudentAsync(EnrollmentDTO dto);
        Task<(bool IsSuccess, string ErrorMessage)> UnenrollStudentAsync(EnrollmentDTO dto);
        Task<List<User>> GetStudentsByCourseIdAsync(int courseId);
        Task<List<Course>> GetCoursesByStudentIdAsync(string userId);
    }
}
