using Microsoft.EntityFrameworkCore;
using StudyBuddyAPI.Data;
using StudyBuddyAPI.DTOs;
using StudyBuddyAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudyBuddyAPI.Services
{
    public class AssignmentService : IAssignmentService
    {
        private readonly StudyBuddyDbContext _context;

        public AssignmentService(StudyBuddyDbContext context)
        {
            _context = context;
        }

        public async Task<List<Assignment>> GetAllAssignmentsAsync()
        {
            return await _context.Assignments
                .Include(a => a.Course)
                .ToListAsync();
        }

        public async Task<Assignment?> GetAssignmentByIdAsync(int id)
        {
            return await _context.Assignments
                .Include(a => a.Course)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> CreateAssignmentAsync(AssignmentDto dto)
        {
            try
            {
                // Verify that the course exists
                var courseExists = await _context.Courses.AnyAsync(c => c.Id == dto.CourseId);
                if (!courseExists)
                    return (false, "Course not found");

                var assignment = new Assignment
                {
                    Title = dto.Title,
                    Instructions = dto.Instructions,
                    DueDate = dto.DueDate,
                    MaxScore = dto.MaxScore,
                    CourseId = dto.CourseId
                };

                await _context.Assignments.AddAsync(assignment);
                await _context.SaveChangesAsync();

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> UpdateAssignmentAsync(int id, AssignmentDto dto)
        {
            try
            {
                var assignment = await _context.Assignments.FindAsync(id);
                if (assignment == null)
                    return (false, "Assignment not found");

                // Verify that the course exists
                var courseExists = await _context.Courses.AnyAsync(c => c.Id == dto.CourseId);
                if (!courseExists)
                    return (false, "Course not found");

                assignment.Title = dto.Title;
                assignment.Instructions = dto.Instructions;
                assignment.DueDate = dto.DueDate;
                assignment.MaxScore = dto.MaxScore;
                assignment.CourseId = dto.CourseId;

                _context.Assignments.Update(assignment);
                await _context.SaveChangesAsync();

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> DeleteAssignmentAsync(int id)
        {
            try
            {
                var assignment = await _context.Assignments.FindAsync(id);
                if (assignment == null)
                    return (false, "Assignment not found");

                _context.Assignments.Remove(assignment);
                await _context.SaveChangesAsync();

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<List<Assignment>> GetAssignmentsByCourseIdAsync(int courseId)
        {
            return await _context.Assignments
                .Include(a => a.Course)
                .Where(a => a.CourseId == courseId)
                .ToListAsync();
        }
    }
}