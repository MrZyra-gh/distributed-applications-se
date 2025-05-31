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
    public class CourseService : ICourseService
    {
        private readonly StudyBuddyDbContext _context;

        public CourseService(StudyBuddyDbContext context)
        {
            _context = context;
        }

        public async Task<List<Course>> GetAllCoursesAsync()
        {
            return await _context.Courses
                .Include(c => c.Instructor)
                .ToListAsync();
        }

        public async Task<Course?> GetCourseByIdAsync(int id)
        {
            return await _context.Courses
                .Include(c => c.Instructor)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> CreateCourseAsync(CourseDto dto)
        {
            try
            {
                var course = new Course
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    Capacity = dto.Capacity,
                    InstructorId = dto.InstructorId
                };

                await _context.Courses.AddAsync(course);
                await _context.SaveChangesAsync();

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> UpdateCourseAsync(int id, CourseDto dto)
        {
            try
            {
                var course = await _context.Courses.FindAsync(id);
                if (course == null)
                    return (false, "Course not found");

                course.Title = dto.Title;
                course.Description = dto.Description;
                course.StartDate = dto.StartDate;
                course.EndDate = dto.EndDate;
                course.Capacity = dto.Capacity;
                course.InstructorId = dto.InstructorId;

                _context.Courses.Update(course);
                await _context.SaveChangesAsync();

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> DeleteCourseAsync(int id)
        {
            try
            {
                var course = await _context.Courses.FindAsync(id);
                if (course == null)
                    return (false, "Course not found");

                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<List<Course>> GetCoursesByInstructorIdAsync(string instructorId)
        {
            return await _context.Courses
                .Include(c => c.Instructor)
                .Where(c => c.InstructorId == instructorId)
                .ToListAsync();
        }
    }
}