using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyBuddyAPI.DTOs;
using StudyBuddyAPI.Services;
using System.ComponentModel.DataAnnotations;

namespace StudyBuddyAPI.Controllers
{
    /// <summary>
    /// API Controller for managing courses
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        /// <summary>
        /// Gets all courses
        /// </summary>
        /// <returns>A list of all courses</returns>
        /// <response code="200">Returns the list of courses</response>
        /// <response code="401">If the user is not authorized</response>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCourses()
        {
            var courses = await _courseService.GetAllCoursesAsync();
            return Ok(courses);
        }

        /// <summary>
        /// Gets a specific course by id
        /// </summary>
        /// <param name="id">The course id</param>
        /// <returns>The requested course</returns>
        /// <response code="200">Returns the course</response>
        /// <response code="404">If the course is not found</response>
        /// <response code="401">If the user is not authorized</response>
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCourse([Required] int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null) return NotFound("Course not found.");
            return Ok(course);
        }

        /// <summary>
        /// Gets all courses for a specific instructor
        /// </summary>
        /// <param name="instructorId">The instructor's user id</param>
        /// <returns>A list of courses taught by the instructor</returns>
        /// <response code="200">Returns the list of courses</response>
        /// <response code="401">If the user is not authorized</response>
        [HttpGet("instructor/{instructorId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCoursesByInstructor([Required] string instructorId)
        {
            var courses = await _courseService.GetCoursesByInstructorIdAsync(instructorId);
            return Ok(courses);
        }

        /// <summary>
        /// Creates a new course
        /// </summary>
        /// <param name="dto">The course data</param>
        /// <returns>A success message</returns>
        /// <response code="200">If the course was created successfully</response>
        /// <response code="400">If the request data is invalid</response>
        /// <response code="401">If the user is not authorized</response>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateCourse([FromBody][Required] CourseDto dto)
        {
            var (isSuccess, error) = await _courseService.CreateCourseAsync(dto);
            if (!isSuccess)
                return BadRequest(error);
            return Ok("Course created successfully.");
        }

        /// <summary>
        /// Updates an existing course
        /// </summary>
        /// <param name="id">The course id</param>
        /// <param name="dto">The updated course data</param>
        /// <returns>A success message</returns>
        /// <response code="200">If the course was updated successfully</response>
        /// <response code="400">If the request data is invalid or course doesn't exist</response>
        /// <response code="401">If the user is not authorized</response>
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateCourse([Required] int id, [FromBody][Required] CourseDto dto)
        {
            var (isSuccess, error) = await _courseService.UpdateCourseAsync(id, dto);
            if (!isSuccess) return BadRequest(error);
            return Ok("Course updated successfully.");
        }

        /// <summary>
        /// Deletes a course
        /// </summary>
        /// <param name="id">The course id</param>
        /// <returns>A success message</returns>
        /// <response code="200">If the course was deleted successfully</response>
        /// <response code="400">If the course doesn't exist</response>
        /// <response code="401">If the user is not authorized</response>
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteCourse([Required] int id)
        {
            var (isSuccess, error) = await _courseService.DeleteCourseAsync(id);
            if (!isSuccess) return BadRequest(error);
            return Ok("Course deleted successfully.");
        }
    }
}