using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyBuddyAPI.DTOs;
using StudyBuddyAPI.Services;
using System.ComponentModel.DataAnnotations;

namespace StudyBuddyAPI.Controllers
{
    /// <summary>
    /// API Controller for managing assignments
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentsController : ControllerBase
    {
        private readonly IAssignmentService _assignmentService;

        public AssignmentsController(IAssignmentService assignmentService)
        {
            _assignmentService = assignmentService;
        }

        /// <summary>
        /// Gets all assignments
        /// </summary>
        /// <returns>A list of all assignments</returns>
        /// <response code="200">Returns the list of assignments</response>
        /// <response code="401">If the user is not authorized</response>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAssignments()
        {
            var assignments = await _assignmentService.GetAllAssignmentsAsync();
            return Ok(assignments);
        }

        /// <summary>
        /// Gets a specific assignment by id
        /// </summary>
        /// <param name="id">The assignment id</param>
        /// <returns>The requested assignment</returns>
        /// <response code="200">Returns the assignment</response>
        /// <response code="404">If the assignment is not found</response>
        /// <response code="401">If the user is not authorized</response>
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAssignment([Required] int id)
        {
            var assignment = await _assignmentService.GetAssignmentByIdAsync(id);
            if (assignment == null) return NotFound("Assignment not found.");
            return Ok(assignment);
        }

        /// <summary>
        /// Gets all assignments for a specific course
        /// </summary>
        /// <param name="courseId">The course id</param>
        /// <returns>A list of assignments for the course</returns>
        /// <response code="200">Returns the list of assignments</response>
        /// <response code="401">If the user is not authorized</response>
        [HttpGet("course/{courseId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAssignmentsByCourse([Required] int courseId)
        {
            var assignments = await _assignmentService.GetAssignmentsByCourseIdAsync(courseId);
            return Ok(assignments);
        }

        /// <summary>
        /// Creates a new assignment
        /// </summary>
        /// <param name="dto">The assignment data</param>
        /// <returns>A success message</returns>
        /// <response code="200">If the assignment was created successfully</response>
        /// <response code="400">If the request data is invalid or the referenced course doesn't exist</response>
        /// <response code="401">If the user is not authorized</response>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateAssignment([FromBody][Required] AssignmentDto dto)
        {
            var (isSuccess, error) = await _assignmentService.CreateAssignmentAsync(dto);
            if (!isSuccess)
                return BadRequest(error);
            return Ok("Assignment created successfully.");
        }

        /// <summary>
        /// Updates an existing assignment
        /// </summary>
        /// <param name="id">The assignment id</param>
        /// <param name="dto">The updated assignment data</param>
        /// <returns>A success message</returns>
        /// <response code="200">If the assignment was updated successfully</response>
        /// <response code="400">If the request data is invalid, assignment doesn't exist, or referenced course doesn't exist</response>
        /// <response code="401">If the user is not authorized</response>
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateAssignment([Required] int id, [FromBody][Required] AssignmentDto dto)
        {
            var (isSuccess, error) = await _assignmentService.UpdateAssignmentAsync(id, dto);
            if (!isSuccess) return BadRequest(error);
            return Ok("Assignment updated successfully.");
        }

        /// <summary>
        /// Deletes an assignment
        /// </summary>
        /// <param name="id">The assignment id</param>
        /// <returns>A success message</returns>
        /// <response code="200">If the assignment was deleted successfully</response>
        /// <response code="400">If the assignment doesn't exist</response>
        /// <response code="401">If the user is not authorized</response>
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteAssignment([Required] int id)
        {
            var (isSuccess, error) = await _assignmentService.DeleteAssignmentAsync(id);
            if (!isSuccess) return BadRequest(error);
            return Ok("Assignment deleted successfully.");
        }
    }
}