using Microsoft.AspNetCore.Mvc;
using StudyBuddyMVC.DTOs;

namespace StudyBuddyMVC.Services
{
    public interface ICourseApiService 
    {
        //enrollment stuff
        Task<List<CourseDto>> GetCoursesByStudentIdAsync(string userId);
        Task<(bool IsSuccess, string ErrorMessage)> EnrollStudentAsync(int courseId, string studentId);
        Task<List<UserDto>> GetEnrolledStudentsAsync(int courseId);
        Task<(bool IsSuccess, string ErrorMessage)> RemoveStudentAsync(int courseId, string studentId);

        Task<(bool IsSuccess, string ErrorMessage)> CreateCourseAsync(CourseDto dto);
        Task<(bool IsSuccess, string ErrorMessage)> UpdateCourseAsync(int id, CourseDto dto);
        Task<(bool IsSuccess, string ErrorMessage)> DeleteCourseAsync(int id);
        Task<List<CourseDto>> GetCoursesByInstructorIdAsync(string instructorId);
        Task<CourseDto?> GetCourseByIdAsync(int id);
        Task<List<CourseDto>> GetAllCoursesAsync();
    }
}
