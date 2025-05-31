using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyBuddyAPI.DTOs;
using StudyBuddyAPI.Services;
using System.ComponentModel.DataAnnotations;

namespace StudyBuddyAPI.Controllers
{
    /// <summary>
    /// API Controller for managing student enrollments in courses
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        /// <summary>
        /// Enrolls a student in a course
        /// </summary>
        /// <param name="dto">The enrollment data containing student and course information</param>
        /// <returns>A success message</returns>
        /// <response code="200">If the student was enrolled successfully</response>
        /// <response code="400">If the request data is invalid, course not found, or student not found</response>
        /// <response code="401">If the user is not authorized</response>
        [HttpPost("enroll")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> EnrollStudent([FromBody][Required] EnrollmentDTO dto)
        {
            var (isSuccess, error) = await _enrollmentService.EnrollStudentAsync(dto);
            if (!isSuccess)
                return BadRequest(error);
            return Ok("Student enrolled successfully.");
        }

        /// <summary>
        /// Unenrolls a student from a course
        /// </summary>
        /// <param name="dto">The enrollment data containing student and course information</param>
        /// <returns>A success message</returns>
        /// <response code="200">If the student was unenrolled successfully</response>
        /// <response code="400">If the enrollment is not found</response>
        /// <response code="401">If the user is not authorized</response>
        [HttpDelete("unenroll")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UnenrollStudent([FromBody][Required] EnrollmentDTO dto)
        {
            var (isSuccess, error) = await _enrollmentService.UnenrollStudentAsync(dto);
            if (!isSuccess)
                return BadRequest(error);
            return Ok("Student unenrolled successfully.");
        }

        /// <summary>
        /// Gets all students enrolled in a specific course
        /// </summary>
        /// <param name="courseId">The course id</param>
        /// <returns>A list of students enrolled in the course</returns>
        /// <response code="200">Returns the list of students</response>
        /// <response code="401">If the user is not authorized</response>
        [HttpGet("course/{courseId}/students")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetStudentsByCourse([Required] int courseId)
        {
            var students = await _enrollmentService.GetStudentsByCourseIdAsync(courseId);
            return Ok(students);
        }

        /// <summary>
        /// Gets all courses a specific student is enrolled in
        /// </summary>
        /// <param name="studentId">The student's user id</param>
        /// <returns>A list of courses the student is enrolled in</returns>
        /// <response code="200">Returns the list of courses</response>
        /// <response code="401">If the user is not authorized</response>
        [HttpGet("student/{studentId}/courses")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCoursesByStudent([Required] string studentId)
        {
            var courses = await _enrollmentService.GetCoursesByStudentIdAsync(studentId);
            return Ok(courses);
        }
    }
}