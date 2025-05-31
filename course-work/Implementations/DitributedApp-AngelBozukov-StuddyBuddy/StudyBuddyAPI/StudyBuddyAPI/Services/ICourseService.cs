using StudyBuddyAPI.Models;
using StudyBuddyAPI.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudyBuddyAPI.Services
{
    public interface ICourseService
    {
        Task<List<Course>> GetAllCoursesAsync();
        Task<Course?> GetCourseByIdAsync(int id);
        Task<(bool IsSuccess, string ErrorMessage)> CreateCourseAsync(CourseDto dto);
        Task<(bool IsSuccess, string ErrorMessage)> UpdateCourseAsync(int id, CourseDto dto);
        Task<(bool IsSuccess, string ErrorMessage)> DeleteCourseAsync(int id);
        Task<List<Course>> GetCoursesByInstructorIdAsync(string instructorId);
    }
}