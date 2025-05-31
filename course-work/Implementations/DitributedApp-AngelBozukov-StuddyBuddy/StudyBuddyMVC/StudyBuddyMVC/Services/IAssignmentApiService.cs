using StudyBuddyMVC.DTOs;

namespace StudyBuddyMVC.Services
{
    public interface IAssignmentApiService
    {
        Task<AssignmentDto?> GetAssignmentByIdAsync(int id);
        Task<(bool IsSuccess, string ErrorMessage)> CreateAssignmentAsync(AssignmentDto dto);
        Task<(bool IsSuccess, string ErrorMessage)> UpdateAssignmentAsync(int id, AssignmentDto dto);
        Task<(bool IsSuccess, string ErrorMessage)> DeleteAssignmentAsync(int id);
        Task<List<AssignmentDto>> GetAssignmentsByCourseIdAsync(int courseId);

        Task<List<AssignmentDto>> GetAssignmentsByUserIdAsync(string userId);
    }
}
