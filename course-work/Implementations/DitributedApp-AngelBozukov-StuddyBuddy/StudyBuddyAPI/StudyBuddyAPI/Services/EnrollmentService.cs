using Microsoft.EntityFrameworkCore;
using StudyBuddyAPI.Data;
using StudyBuddyAPI.DTOs;
using StudyBuddyAPI.Models;

namespace StudyBuddyAPI.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly StudyBuddyDbContext _context;

        public EnrollmentService(StudyBuddyDbContext context)
        {
            _context = context;
        }

        public async Task<(bool, string)> EnrollStudentAsync(EnrollmentDTO dto)
        {
            try
            {
                // Verify that the course exists
                var courseExists = await _context.Courses.AnyAsync(c => c.Id == dto.CourseId);
                if (!courseExists)
                    return (false, "Course not found");

                var userExists = await _context.Users.AnyAsync(c => c.Id == dto.StudentId);
                if (!courseExists)
                    return (false, "Student not found");

                var enrollment = new Enrollment
                {
                    UserId = dto.StudentId,
                    CourseId = dto.CourseId
                };

                await _context.Enrollments.AddAsync(enrollment);
                await _context.SaveChangesAsync();

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool, string)> UnenrollStudentAsync(EnrollmentDTO dto)
        {
            try
            {
                var enrollment = await _context.Enrollments
                    .FirstOrDefaultAsync(e => e.UserId == dto.StudentId && e.CourseId == dto.CourseId);

                if (enrollment == null)
                    return (false, "Enrollment not found.");

                _context.Enrollments.Remove(enrollment);
                await _context.SaveChangesAsync();
                return (true, string.Empty);

            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }


        }

        public async Task<List<User>> GetStudentsByCourseIdAsync(int courseId)
        {
            return await _context.Enrollments
                .Where(e => e.CourseId == courseId)
                .Include(e => e.User)
                .Select(e => e.User)
                .ToListAsync();
        }

        public async Task<List<Course>> GetCoursesByStudentIdAsync(string userId)
        {
            return await _context.Enrollments
                .Where(e => e.UserId == userId)
                .Include(e => e.Course)
                .Select(e => e.Course)
                .ToListAsync();
        }
    }

}