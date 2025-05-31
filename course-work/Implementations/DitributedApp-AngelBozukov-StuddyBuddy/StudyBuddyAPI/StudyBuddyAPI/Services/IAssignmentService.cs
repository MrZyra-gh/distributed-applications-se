using StudyBuddyAPI.Models;
using StudyBuddyAPI.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudyBuddyAPI.Services
{
    public interface IAssignmentService
    {
        Task<List<Assignment>> GetAllAssignmentsAsync();
        Task<Assignment?> GetAssignmentByIdAsync(int id);
        Task<(bool IsSuccess, string ErrorMessage)> CreateAssignmentAsync(AssignmentDto dto);
        Task<(bool IsSuccess, string ErrorMessage)> UpdateAssignmentAsync(int id, AssignmentDto dto);
        Task<(bool IsSuccess, string ErrorMessage)> DeleteAssignmentAsync(int id);
        Task<List<Assignment>> GetAssignmentsByCourseIdAsync(int courseId);
    }
}